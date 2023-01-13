using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemBarcodePage : Page
    {
        BarcodeScanner scanner = null;
        ClaimedBarcodeScanner claimedScanner = null;

        public ItemBarcodePage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ResetTheScenarioState();

            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            Output.Text = resourceLoader.GetString("CreatingBarcodeScannerObject");

            if (await CreateDefaultScannerObject())
            {
                if (await ClaimScanner())
                {
                    claimedScanner.ReleaseDeviceRequested += claimedScanner_ReleaseDeviceRequested;
                    claimedScanner.DataReceived += claimedScanner_DataReceived;
                    claimedScanner.IsDecodeDataEnabled = true;

                    await claimedScanner.EnableAsync();

                    Output.Text = resourceLoader.GetString("ReadyToBarcodeScan") + claimedScanner.DeviceId;
                }
            }

            if (TestInfoSet.Barcode.Supported)
            {
                TestInfoSet.Barcode.Status = TestStatus.Testing;
                TestInfoSet.Barcode.StartTime = DateTime.Now;
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ResetTheScenarioState();

            base.OnNavigatedFrom(e);
        }

        private async Task<bool> CreateDefaultScannerObject()
        {
            if (scanner == null)
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                Output.Text = resourceLoader.GetString("CreatingBarcodeScannerObject");

                DeviceInformationCollection deviceCollection = await DeviceInformation.FindAllAsync(BarcodeScanner.GetDeviceSelector());

                if (deviceCollection != null && deviceCollection.Count > 0)
                {
                    scanner = await BarcodeScanner.FromIdAsync(deviceCollection[0].Id);

                    if (scanner == null)
                    {
                        Output.Text = resourceLoader.GetString("FailedToCreateBarcodeScannerObject");

                        TestInfoSet.Barcode.Supported = false;
                        TestInfoSet.Barcode.StartTime = DateTime.Now;
                        TestInfoSet.Barcode.FinishTime = DateTime.Now;

                        return false;
                    }
                }
                else
                {
                    Output.Text = resourceLoader.GetString("FailedToCreateBarcodeScannerObject");

                    TestInfoSet.Barcode.Supported = false;
                    TestInfoSet.Barcode.StartTime = DateTime.Now;
                    TestInfoSet.Barcode.FinishTime = DateTime.Now;

                    return false;
                }
            }

            TestInfoSet.Barcode.Supported = true;

            return true;
        }

        private async Task<bool> ClaimScanner()
        {
            if (claimedScanner == null)
            {
                claimedScanner = await scanner.ClaimScannerAsync();

                if (claimedScanner == null)
                {
                    var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                    Output.Text = resourceLoader.GetString("ClaimBarcodeScannerFailed");

                    TestInfoSet.Barcode.Supported = false;
                    TestInfoSet.Barcode.StartTime = DateTime.Now;
                    TestInfoSet.Barcode.FinishTime = DateTime.Now;

                    return false;
                }
            }

            TestInfoSet.Barcode.Supported = true;
            return true;
        }

        private async void claimedScanner_ReleaseDeviceRequested(object sender, ClaimedBarcodeScanner e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                e.RetainDevice();

                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                Output.Text = resourceLoader.GetString("RetainingBarcodeScanner");
            });
        }

        private void ResetTheScenarioState()
        {
            if (claimedScanner != null)
            {
                claimedScanner.DataReceived -= claimedScanner_DataReceived;
                claimedScanner.ReleaseDeviceRequested -= claimedScanner_ReleaseDeviceRequested;

                claimedScanner.Dispose();

                claimedScanner = null;
            }

            scanner = null;

            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            Output.Text = resourceLoader.GetString("Idle");

            this.OutputScanData.Text = resourceLoader.GetString("NoData");
            this.OutputScanDataLabel.Text = resourceLoader.GetString("NoData");
            this.OutputScanDataType.Text = resourceLoader.GetString("NoData");
        }

        private string GetDataString(IBuffer data)
        {
            StringBuilder result = new StringBuilder();

            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            if (data == null)
            {
                result.Append(resourceLoader.GetString("NoData"));
            }
            else
            {
                const uint MAX_BYTES_TO_PRINT = 20;
                uint bytesToPrint = Math.Min(data.Length, MAX_BYTES_TO_PRINT);

                DataReader reader = DataReader.FromBuffer(data);
                byte[] dataBytes = new byte[bytesToPrint];

                reader.ReadBytes(dataBytes);

                for (uint byteIndex = 0; byteIndex < bytesToPrint; ++byteIndex)
                {
                    result.AppendFormat("{0:X2} ", dataBytes[byteIndex]);
                }

                if (bytesToPrint < data.Length)
                {
                    result.Append("...");
                }
            }

            return result.ToString();
        }

        private string GetDataLabelString(IBuffer data, uint scanDataType)
        {
            string result = null;

            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            if (data == null)
            {
                result = resourceLoader.GetString("NoData");
            }
            else
            {
                switch (BarcodeSymbologies.GetName(scanDataType))
                {
                    case "Upca":
                    case "UpcaAdd2":
                    case "UpcaAdd5":
                    case "Upce":
                    case "UpceAdd2":
                    case "UpceAdd5":
                    case "Ean8":
                    case "TfStd":
                        {
                            DataReader reader = DataReader.FromBuffer(data);
                            result = reader.ReadString(data.Length);
                        }
                        break;
                    default:
                        {
                            result = string.Format(resourceLoader.GetString("RawLabelData"), GetDataString(data));
                        }
                        break;
                }
            }

            return result;
        }

        private async void claimedScanner_DataReceived(ClaimedBarcodeScanner sender, BarcodeScannerDataReceivedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                OutputScanDataLabel.Text = GetDataLabelString(args.Report.ScanDataLabel, args.Report.ScanDataType);
                OutputScanData.Text = GetDataString(args.Report.ScanData);
                OutputScanDataType.Text = BarcodeSymbologies.GetName(args.Report.ScanDataType);
            });
        }

        private void Success()
        {
            TestInfoSet.Barcode.FinishTime = DateTime.Now;
            TestInfoSet.Barcode.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Barcode.FinishTime = DateTime.Now;
            TestInfoSet.Barcode.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
