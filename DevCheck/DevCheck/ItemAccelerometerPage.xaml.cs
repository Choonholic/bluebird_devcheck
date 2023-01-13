using System;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemAccelerometerPage : Page
    {
        private Accelerometer _accelerometer;

        public ItemAccelerometerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _accelerometer = Accelerometer.GetDefault();

            if (_accelerometer != null)
            {
                _accelerometer.ReadingChanged += ReadingChanged;
                _accelerometer.ReportInterval = Math.Max(_accelerometer.MinimumReportInterval, 16);

                TestInfoSet.Accelerometer.Supported = true;
                TestInfoSet.Accelerometer.Status = TestStatus.Testing;
                TestInfoSet.Accelerometer.StartTime = DateTime.Now;
            }
            else
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                Output.Text = resourceLoader.GetString("NoAccelerometerFound");

                TestInfoSet.Accelerometer.Supported = false;
                TestInfoSet.Accelerometer.Status = TestStatus.Tested;
                TestInfoSet.Accelerometer.StartTime = DateTime.Now;
                TestInfoSet.Accelerometer.FinishTime = DateTime.Now;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _accelerometer.ReadingChanged -= ReadingChanged;
            _accelerometer.ReportInterval = 0;

            base.OnNavigatingFrom(e);
        }

        async private void ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                Output.Text = string.Format(resourceLoader.GetString("AccelerometerData"), e.Reading.AccelerationX, e.Reading.AccelerationY, e.Reading.AccelerationZ);
            });
        }

        private void Success()
        {
            TestInfoSet.Accelerometer.FinishTime = DateTime.Now;
            TestInfoSet.Accelerometer.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Accelerometer.FinishTime = DateTime.Now;
            TestInfoSet.Accelerometer.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
