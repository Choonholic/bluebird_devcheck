using Bluebird;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemSamPage : Page
    {
        public ItemSamPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TestInfoSet.Sam.Supported = true;
            TestInfoSet.Sam.Status = TestStatus.NotTested;

            StartTestButton.IsEnabled = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private async void StartTest()
        {
            string samResult;

            StartTestButton.IsEnabled = false;

            TestInfoSet.Sam.Supported = true;
            TestInfoSet.Sam.Status = TestStatus.Testing;
            TestInfoSet.Sam.StartTime = DateTime.Now;

            Output.Text = "Getting Firmware Version from SAM... ";

            samResult = await MainPage.Current._device.InvokeCommand(CommandSet.SAM_GET_FW);

            Output.Text += samResult + "\n";
            Output.Text += "Turning SAM on... ";

            samResult = await MainPage.Current._device.InvokeCommand(CommandSet.SAM_POWER);

            Output.Text += "\n"+"ATR : " + samResult + "\n";
            Output.Text += "Turning SAM off... ";

            samResult = await MainPage.Current._device.InvokeCommand(CommandSet.SAM_STOP);

            Output.Text += samResult + "\n";

            TestInfoSet.Sam.Status = TestStatus.Tested;
            TestInfoSet.Sam.FinishTime = DateTime.Now;
        }

        private void Success()
        {
            TestInfoSet.Sam.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Sam.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
