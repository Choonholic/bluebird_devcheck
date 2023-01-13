using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Bluebird
{
    public enum CommandSet
    {
        DPP_REG_SET = 0x30,
        DPP_GET_BB_SN = 0x31,
        DPP_GET_QCOM_BT = 0x32,
        DPP_GET_QCOM_WLAN = 0x33,
        DPP_GET_QCOM_WLAN_CLPC = 0x34,
        DPP_GET_REG_DOMAIN = 0x35,
        DPP_GET_BOARD_VERSION = 0x36,
        SAM_GET_FW = 0x50,
        SAM_GET_FW_STATE = 0x51,
        SAM_POWER = 0x52,
        SAM_STOP = 0x53,
        SAM_FW_UPDATE = 0x54,
        LED_TEST = 0x60
    }

    public enum InitializeState
    {
        NotInitiated,
        Initiated,
        Initialized
    }

    public sealed class Device
    {
        private static StreamSocket _socket;
        private HostName _hostName;

        private const string _hostIPAddress = "127.0.0.1";
        private const string _hostPort = "9123";

        private string _data = string.Empty;
        private string _regdata = string.Empty;

        private byte[] _bytes = { 0, };

        private static InitializeState _initializeState = InitializeState.NotInitiated;
        private static bool _registerState = false;

        private Object socketLock = new Object();

        public Device()
        {
            Initialize();
            Register();
        }

        ~Device()
        {
            Cleanup();
        }

        public async void Initialize()
        {
            Debug.WriteLine("Bluebird.Device.Initialize()");

            if (_initializeState == InitializeState.NotInitiated)
            {
                _initializeState = InitializeState.Initiated;

                try
                {
                    if (_socket == null)
                    {
                        _socket = new StreamSocket();
                        _hostName = new HostName(_hostIPAddress);

                        await _socket.ConnectAsync(_hostName, _hostPort);

                        await Task.Delay(TimeSpan.FromMilliseconds(1000));

                        _initializeState = InitializeState.Initialized;

                        Debug.WriteLine("Bluebird.Device.Initialize() - Initialized");
                    }
                }
                catch (Exception ex)
                {
                    _initializeState = InitializeState.NotInitiated;

                    if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                    {
                        throw;
                    }
                }
            }
        }

        public async void Register()
        {
            Debug.WriteLine("Bluebird.Device.Register()");

            while (_initializeState == InitializeState.Initiated)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                Debug.WriteLine("Bluebird.Device.Register() - " + _initializeState.ToString());
            }

            await _InvokeCommand(CommandSet.DPP_REG_SET);

            _registerState = true;

            Debug.WriteLine("Bluebird.Device.Register() - Registered");
        }

        public void Cleanup()
        {
            Debug.WriteLine("Bluebird.Device.Cleanup()");

            WaitForReady();

            if (_initializeState == InitializeState.Initialized)
            {
                try
                {
                    _socket.Dispose();

                    _socket = null;

                    _initializeState = InitializeState.NotInitiated;
                }
                catch (Exception ex)
                {
                    if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                    {
                        throw;
                    }
                }
            }
        }

        public bool IsInitialized()
        {
            return (bool)(_initializeState == InitializeState.Initialized);
        }

        public bool IsRegistered()
        {
            return _registerState;
        }

        public async void WaitForReady()
        {
            while (_initializeState == InitializeState.Initiated)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                Debug.WriteLine("Bluebird.Device.WaitForReady() - " + _initializeState.ToString());
            }

            while (_registerState == false)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(2500));

                Debug.WriteLine("Bluebird.Device.WaitForReady() - " + _registerState.ToString());
            }
        }

        public IAsyncOperation<string> InvokeCommand(CommandSet command)
        {
            return Task.Run<string>(async () =>
            {
                Debug.WriteLine("Bluebird.Device.InvokeCommand(" + command + ")");

                WaitForReady();

                return await _InvokeCommand(command);
            }).AsAsyncOperation();
        }

        public IAsyncOperation<string> _InvokeCommand(CommandSet command)
        {
            return Task.Run<string>(async () =>
            {
                Debug.WriteLine("Bluebird.Device._InvokeCommand(" + command + ")");

                if (_socket != null)
                {
                    try
                    {
                        byte[] message = { 0, };

                        message[0] = (byte)((byte)command & 0xFF);

                        await Send(message);

                        _data = await Receive();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Bluebird.Device._InvokeCommand(" + command + ") - " + SocketError.GetStatus(ex.HResult).ToString());
                        throw;
                    }
                }

                Debug.WriteLine("Bluebird.Device._InvokeCommand(" + command + ") - " + _data + " [" + _data.Length + "]");

                return _data;
            }).AsAsyncOperation();
        }

        private async Task Send(byte[] message)
        {
            DataWriter writer;

            using (writer = new DataWriter(_socket.OutputStream))
            {
                try
                {
                    writer.WriteBytes(message);

                    await writer.StoreAsync();
                    await writer.FlushAsync();

                    writer.DetachStream();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Bluebird.Device.Send() - " + ex.ToString());
                }
            }
        }

        private async Task<String> Receive()
        {
            DataReader reader;
            StringBuilder builder;

            byte[] buffer1 = new byte[256];
            byte[] buffer2 = new byte[256];

            using (reader = new DataReader(_socket.InputStream))
            {
                builder = new StringBuilder();

                reader.InputStreamOptions = InputStreamOptions.Partial;
                reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                reader.ByteOrder = ByteOrder.LittleEndian;

                await reader.LoadAsync(256);

                do
                {
                    if (reader.UnconsumedBufferLength <= 0)
                    {
                        break;
                    }

                    try
                    {
                        builder.Append(reader.ReadString(reader.UnconsumedBufferLength));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Debug.WriteLine("Bluebird.Device.Receive() - " + e.ToString());

                        buffer1 = new byte[reader.UnconsumedBufferLength];

                        reader.ReadBytes(buffer1);

                        buffer2 = new byte[buffer1.Length >> 2];

                        for (int i = 0; i < (buffer1.Length >> 2); i++)
                        {
                            buffer2[i] = buffer1[i << 2];
                        }

                        builder.Append(BitConverter.ToString(buffer2));
                    }
                } while (true);

                reader.DetachStream();

                return builder.ToString();
            }
        }
    }
}
