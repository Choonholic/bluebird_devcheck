using Bluebird;
using System;
using Windows.Phone.Devices.Notification;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemLedAndVibratorPage : Page
    {
        VibrationDevice _vibrator = VibrationDevice.GetDefault();

        public ItemLedAndVibratorPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_vibrator != null)
            {
                TestInfoSet.LedAndVibrator.Supported = true;
                TestInfoSet.LedAndVibrator.Status = TestStatus.NotTested;

                StartTestButton.IsEnabled = true;
            }
            else
            {
                TestInfoSet.LedAndVibrator.Supported = false;
                TestInfoSet.LedAndVibrator.Status = TestStatus.Tested;
                TestInfoSet.LedAndVibrator.StartTime = DateTime.Now;
                TestInfoSet.LedAndVibrator.FinishTime = DateTime.Now;

                StartTestButton.IsEnabled = false;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private async void StartTest()
        {
            if (_vibrator != null)
            {
                StartTestButton.IsEnabled = false;

                TestInfoSet.LedAndVibrator.Supported = true;
                TestInfoSet.LedAndVibrator.Status = TestStatus.Testing;
                TestInfoSet.LedAndVibrator.StartTime = DateTime.Now;

                Output.Text = "Red, Green and Blue LED will be turned on continuously.\n";

                await MainPage.Current._device.InvokeCommand(CommandSet.LED_TEST);

                Output.Text += "Vibrator will be turned on.";

                _vibrator.Vibrate(TimeSpan.FromSeconds(2));
            }
            else
            {
                StartTestButton.IsEnabled = false;

                TestInfoSet.LedAndVibrator.Supported = false;
                TestInfoSet.LedAndVibrator.Status = TestStatus.Tested;
                TestInfoSet.LedAndVibrator.StartTime = DateTime.Now;
                TestInfoSet.LedAndVibrator.FinishTime = DateTime.Now;
            }
        }

        private void Success()
        {
            TestInfoSet.LedAndVibrator.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.LedAndVibrator.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
