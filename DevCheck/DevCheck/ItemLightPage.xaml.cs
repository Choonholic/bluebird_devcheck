using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemLightPage : Page
    {
        private LightSensor _sensor;
        private uint _desiredReportInterval;

        public ItemLightPage()
        {
            this.InitializeComponent();

            _sensor = LightSensor.GetDefault();

            if (_sensor != null)
            {
                uint minReportInterval = _sensor.MinimumReportInterval;
                _desiredReportInterval = minReportInterval > 100 ? minReportInterval : 100;

                TestInfoSet.Light.Supported = true;
                TestInfoSet.Light.Status = TestStatus.NotTested;
            }
            else
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                Output.Text = resourceLoader.GetString("LightNotSupported");

                TestInfoSet.Light.Supported = false;
                TestInfoSet.Light.Status = TestStatus.Tested;
                TestInfoSet.Light.StartTime = DateTime.Now;
                TestInfoSet.Light.FinishTime = DateTime.Now;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_sensor != null)
            {
                _sensor.ReportInterval = _desiredReportInterval;

                _sensor.ReadingChanged += new TypedEventHandler<LightSensor, LightSensorReadingChangedEventArgs>(ReadingChanged);

                TestInfoSet.Light.Status = TestStatus.Testing;
                TestInfoSet.Light.StartTime = DateTime.Now;
            }
            else
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                Output.Text = resourceLoader.GetString("LightNotSupported");

                TestInfoSet.Light.Supported = false;
                TestInfoSet.Light.Status = TestStatus.Tested;
                TestInfoSet.Light.StartTime = DateTime.Now;
                TestInfoSet.Light.FinishTime = DateTime.Now;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_sensor != null)
            {
                _sensor.ReadingChanged -= new TypedEventHandler<LightSensor, LightSensorReadingChangedEventArgs>(ReadingChanged);
                _sensor.ReportInterval = 0;
            }

            base.OnNavigatingFrom(e);
        }

        async private void ReadingChanged(object sender, LightSensorReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();
                LightSensorReading reading = e.Reading;

                Output.Text = String.Format(resourceLoader.GetString("Lux"), reading.IlluminanceInLux);
            });
        }

        private void Success()
        {
            TestInfoSet.Light.FinishTime = DateTime.Now;
            TestInfoSet.Light.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Light.FinishTime = DateTime.Now;
            TestInfoSet.Light.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
