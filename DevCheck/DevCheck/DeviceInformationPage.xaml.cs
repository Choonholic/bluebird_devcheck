using Bluebird;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class DeviceInformationPage : Page
    {
        public DeviceInformationPage()
        {
            this.InitializeComponent();

            InfoGrid.Visibility = Visibility.Visible;
            InfoFailed.Visibility = Visibility.Collapsed;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            try
            {
                AnalyticsVersionInfo analyticsVersionInfo = AnalyticsInfo.VersionInfo;

                ulong version = ulong.Parse(analyticsVersionInfo.DeviceFamilyVersion);
                ulong major = (version & 0xFFFF000000000000L) >> 48;
                ulong minor = (version & 0x0000FFFF00000000L) >> 32;
                ulong build = (version & 0x00000000FFFF0000L) >> 16;
                ulong revision = (version & 0x000000000000FFFFL);
                               
                EasClientDeviceInformation CurrentDeviceInfo = new EasClientDeviceInformation();

                DeviceID.Text = resourceLoader.GetString("RetrievingInformation");
                SystemManufacturer.Text = resourceLoader.GetString("RetrievingInformation");
                SystemProductName.Text = resourceLoader.GetString("RetrievingInformation");
                SerialNumber.Text = resourceLoader.GetString("RetrievingInformation");
                WiFiMacAddress.Text = resourceLoader.GetString("RetrievingInformation");
                BluetoothMacAddress.Text = resourceLoader.GetString("RetrievingInformation");
                WiFiClpcProvisioningSize.Text = resourceLoader.GetString("RetrievingInformation");
                BoardVersion.Text = resourceLoader.GetString("RetrievingInformation");
                SystemSku.Text = resourceLoader.GetString("RetrievingInformation");
                OperatingSystem.Text = resourceLoader.GetString("RetrievingInformation");
                DeviceFamily.Text = resourceLoader.GetString("RetrievingInformation");
                DeviceVersion.Text = resourceLoader.GetString("RetrievingInformation");
                Architecture.Text = resourceLoader.GetString("RetrievingInformation");
                FriendlyName.Text = resourceLoader.GetString("RetrievingInformation");

                await Task.Delay(TimeSpan.FromSeconds(2));

                DeviceID.Text = CurrentDeviceInfo.Id.ToString();
                SystemManufacturer.Text = CurrentDeviceInfo.SystemManufacturer;
                SystemProductName.Text = CurrentDeviceInfo.SystemProductName;
                SerialNumber.Text = await MainPage.Current._device.InvokeCommand(CommandSet.DPP_GET_BB_SN);
                WiFiMacAddress.Text = await MainPage.Current._device.InvokeCommand(CommandSet.DPP_GET_QCOM_WLAN);
                BluetoothMacAddress.Text = await MainPage.Current._device.InvokeCommand(CommandSet.DPP_GET_QCOM_BT);
                WiFiClpcProvisioningSize.Text = await MainPage.Current._device.InvokeCommand(CommandSet.DPP_GET_QCOM_WLAN_CLPC);
                BoardVersion.Text = await MainPage.Current._device.InvokeCommand(CommandSet.DPP_GET_BOARD_VERSION);
                SystemSku.Text = CurrentDeviceInfo.SystemSku;
                OperatingSystem.Text = CurrentDeviceInfo.OperatingSystem;
                DeviceFamily.Text = analyticsVersionInfo.DeviceFamily;
                DeviceVersion.Text = string.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
                Architecture.Text = Package.Current.Id.Architecture.ToString();
                FriendlyName.Text = CurrentDeviceInfo.FriendlyName;
            }
            catch (Exception ex)
            {
                InfoGrid.Visibility = Visibility.Collapsed;
                InfoFailed.Visibility = Visibility.Visible;
                InfoFailed.Text = string.Format(resourceLoader.GetString("RetrieveInformationFailed"), ex.ToString());
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void StartDeviceTest()
        {
            MainPage.Current.NavigateScenario(1);
        }
    }
}
