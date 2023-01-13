using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemMagneticStripeReaderPage : Page
    {
        MagneticStripeReader _reader = null;
        ClaimedMagneticStripeReader _claimedReader = null;

        public ItemMagneticStripeReaderPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            StartTestButton.IsEnabled = true;
            FinishTestButton.IsEnabled = false;

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (FinishTestButton.IsEnabled == true)
            {
                FinishTest();
                Failure();
            }

            base.OnNavigatedFrom(e);
        }

        private async Task<bool> CreateDefaultMagneticStripeReaderObject()
        {
            if (_reader == null)
            {
                _reader = await MagneticStripeReader.GetDefaultAsync();

                if (_reader == null)
                {
                    Output.Text = "Magnetic Stripe Reader not found. Please connect a Magnetic Stripe Reader.";

                    return false;
                }
            }

            return true;
        }

        private async Task<bool> ClaimReader()
        {
            if (_claimedReader == null)
            {
                _claimedReader = await _reader.ClaimReaderAsync();

                if (_claimedReader == null)
                {
                    Output.Text = "Claim Magnetic Stripe Reader failed.";

                    return false;
                }
            }

            return true;
        }

        private string TrimASCIIZ(string String)
        {
            return String.Substring(0, String.IndexOf('\0'));
        }

        private async void OnVendorSpecificDataReceived(ClaimedMagneticStripeReader sender, MagneticStripeReaderVendorSpecificCardDataReceivedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    var track1 = Windows.Storage.Streams.DataReader.FromBuffer(args.Report.Track1.Data);

                    OutputTrack1.Text = TrimASCIIZ(track1.ReadString(args.Report.Track1.Data.Length));
                    bool readTrack1 = (bool)(OutputTrack1.Text.Length > 0);

                    var track2 = Windows.Storage.Streams.DataReader.FromBuffer(args.Report.Track2.Data);

                    OutputTrack2.Text = TrimASCIIZ(track2.ReadString(args.Report.Track2.Data.Length));
                    bool readTrack2 = (bool)(OutputTrack2.Text.Length > 0);

                    var track3 = Windows.Storage.Streams.DataReader.FromBuffer(args.Report.Track3.Data);

                    OutputTrack3.Text = TrimASCIIZ(track3.ReadString(args.Report.Track3.Data.Length));
                    bool readTrack3 = (bool)(OutputTrack3.Text.Length > 0);

                    if (readTrack1 == true || readTrack2 == true || readTrack3 == true)
                    {
                        Output.Text = "Got track data.";
                    }
                    else
                    {
                        Output.Text = "Failed to get track data.";
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to get track data: " + ex.ToString());

                    Output.Text = "Failed to get track data.";
                }
            });
        }

        private void OnReleaseDeviceRequested(object sender, ClaimedMagneticStripeReader args)
        {
            args.RetainDevice();
        }

        private async void StartTest()
        {
            TestInfoSet.MagneticStripeReader.Supported = false;
            TestInfoSet.MagneticStripeReader.Status = TestStatus.Tested;
            TestInfoSet.MagneticStripeReader.StartTime = DateTime.Now;
            TestInfoSet.MagneticStripeReader.FinishTime = DateTime.Now;

            if (await CreateDefaultMagneticStripeReaderObject())
            {
                if (_reader != null)
                {
                    if (await ClaimReader())
                    {
                        if (_claimedReader != null)
                        {
                            _claimedReader.ReleaseDeviceRequested += OnReleaseDeviceRequested;
                            _claimedReader.VendorSpecificDataReceived += OnVendorSpecificDataReceived;
                            _claimedReader.IsDecodeDataEnabled = true;

                            await _claimedReader.EnableAsync();

                            Output.Text = "Ready to swipe.";

                            StartTestButton.IsEnabled = false;
                            FinishTestButton.IsEnabled = true;

                            TestInfoSet.MagneticStripeReader.Supported = true;
                            TestInfoSet.MagneticStripeReader.Status = TestStatus.Testing;
                            TestInfoSet.MagneticStripeReader.StartTime = DateTime.Now;
                        }
                    }
                }
            }
        }

        private void FinishTest()
        {
            if (_claimedReader != null)
            {
                _claimedReader.VendorSpecificDataReceived -= OnVendorSpecificDataReceived;
                _claimedReader.ReleaseDeviceRequested -= OnReleaseDeviceRequested;

                _claimedReader.Dispose();

                _claimedReader = null;
            }

            if (_reader != null)
            {
                _reader = null;
            }

            StartTestButton.IsEnabled = true;
            FinishTestButton.IsEnabled = false;

            TestInfoSet.MagneticStripeReader.Status = TestStatus.Tested;
            TestInfoSet.MagneticStripeReader.FinishTime = DateTime.Now;
        }

        private void Success()
        {
            if (FinishTestButton.IsEnabled == true)
            {
                FinishTest();
            }

            TestInfoSet.MagneticStripeReader.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            if (FinishTestButton.IsEnabled == true)
            {
                FinishTest();
            }

            TestInfoSet.MagneticStripeReader.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
