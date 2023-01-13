using System;
using System.Collections.ObjectModel;
using Windows.Devices.WiFi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemWiFiPage : Page
    {
        private WiFiAdapter firstAdapter;
        public ObservableCollection<WiFiNetworkDisplay> ResultCollection { get; private set; }

        public ItemWiFiPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            ResultCollection = new ObservableCollection<WiFiNetworkDisplay>();

            var access = await WiFiAdapter.RequestAccessAsync();

            System.Diagnostics.Debug.WriteLine(access.ToString());

            if (access != WiFiAccessStatus.Allowed)
            {
                Results.Visibility = Visibility.Collapsed;
                Output.Visibility = Visibility.Visible;
                Output.Text = resourceLoader.GetString("WiFiAccessDenied");

                TestInfoSet.WiFi.Supported = false;
                TestInfoSet.WiFi.Status = TestStatus.Tested;
                TestInfoSet.WiFi.StartTime = DateTime.Now;
                TestInfoSet.WiFi.FinishTime = DateTime.Now;

                ScanButton.IsEnabled = false;
            }
            else
            {
                DataContext = this;

                var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());

                if (result.Count > 0)
                {
                    firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);
                    ScanButton.IsEnabled = true;

                    TestInfoSet.WiFi.Supported = true;
                    TestInfoSet.WiFi.Status = TestStatus.NotTested;
                }
                else
                {
                    Results.Visibility = Visibility.Collapsed;
                    Output.Visibility = Visibility.Visible;
                    Output.Text = resourceLoader.GetString("WiFiNotSupported");

                    TestInfoSet.WiFi.Supported = false;
                    TestInfoSet.WiFi.Status = TestStatus.Tested;
                    TestInfoSet.WiFi.StartTime = DateTime.Now;
                    TestInfoSet.WiFi.FinishTime = DateTime.Now;
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private async void Scan()
        {
            TestInfoSet.WiFi.Status = TestStatus.Testing;
            TestInfoSet.WiFi.StartTime = DateTime.Now;

            ScanButton.IsEnabled = false;

            await firstAdapter.ScanAsync();

            ScanButton.IsEnabled = true;

            TestInfoSet.WiFi.Status = TestStatus.Tested;
            TestInfoSet.WiFi.FinishTime = DateTime.Now;

            DisplayNetworkReport(firstAdapter.NetworkReport);
        }

        private async void DisplayNetworkReport(WiFiNetworkReport report)
        {
            ResultCollection.Clear();

            foreach (var network in report.AvailableNetworks)
            {
                var networkDisplay = new WiFiNetworkDisplay(network, firstAdapter);

                await networkDisplay.UpdateConnectivityLevel();
                ResultCollection.Add(networkDisplay);
            }
        }

        private void Success()
        {
            TestInfoSet.WiFi.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.WiFi.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
