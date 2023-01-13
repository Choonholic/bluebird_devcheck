using System;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemCompassPage : Page
    {
        private Compass _compass;
        private uint _desiredReportInterval;

        public ItemCompassPage()
        {
            this.InitializeComponent();

            _compass = Compass.GetDefault();

            if (_compass != null)
            {
                CompassData.Visibility = Visibility.Visible;
                CompassDisabled.Visibility = Visibility.Collapsed;

                uint minReportInterval = _compass.MinimumReportInterval;
                _desiredReportInterval = ((minReportInterval > 16) ? minReportInterval : 16);

                TestInfoSet.Compass.Supported = true;
                TestInfoSet.Compass.Status = TestStatus.NotTested;
            }
            else
            {
                CompassData.Visibility = Visibility.Collapsed;
                CompassDisabled.Visibility = Visibility.Visible;

                TestInfoSet.Compass.Supported = false;
                TestInfoSet.Compass.Status = TestStatus.Tested;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_compass != null)
            {
                _compass.ReportInterval = _desiredReportInterval;

                _compass.ReadingChanged += new TypedEventHandler<Compass, CompassReadingChangedEventArgs>(ReadingChanged);

                CompassData.Visibility = Visibility.Visible;
                CompassDisabled.Visibility = Visibility.Collapsed;

                TestInfoSet.Compass.Supported = true;
                TestInfoSet.Compass.Status = TestStatus.Testing;
                TestInfoSet.Compass.StartTime = DateTime.Now;
            }
            else
            {
                CompassData.Visibility = Visibility.Collapsed;
                CompassDisabled.Visibility = Visibility.Visible;

                TestInfoSet.Compass.Supported = false;
                TestInfoSet.Compass.Status = TestStatus.Tested;
                TestInfoSet.Compass.StartTime = DateTime.Now;
                TestInfoSet.Compass.FinishTime = DateTime.Now;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_compass != null)
            {
                _compass.ReadingChanged -= new TypedEventHandler<Compass, CompassReadingChangedEventArgs>(ReadingChanged);
                _compass.ReportInterval = 0;
            }

            base.OnNavigatingFrom(e);
        }

        async private void ReadingChanged(object sender, CompassReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                CompassReading reading = e.Reading;
                OutputMagneticNorth.Text = String.Format("{0,5:0.00}", reading.HeadingMagneticNorth);

                if (reading.HeadingTrueNorth != null)
                {
                    OutputTrueNorth.Text = String.Format("{0,5:0.00}", reading.HeadingTrueNorth);
                }
                else
                {
                    OutputTrueNorth.Text = resourceLoader.GetString("NoData");
                }

                switch (reading.HeadingAccuracy)
                {
                    case MagnetometerAccuracy.Unknown:
                        {
                            OutputHeadingAccuracy.Text = resourceLoader.GetString("Unknown");
                        }
                        break;
                    case MagnetometerAccuracy.Unreliable:
                        {
                            OutputHeadingAccuracy.Text = resourceLoader.GetString("Unreliable");
                        }
                        break;
                    case MagnetometerAccuracy.Approximate:
                        {
                            OutputHeadingAccuracy.Text = resourceLoader.GetString("Approximate");
                        }
                        break;
                    case MagnetometerAccuracy.High:
                        {
                            OutputHeadingAccuracy.Text = resourceLoader.GetString("High");
                        }
                        break;
                    default:
                        {
                            OutputHeadingAccuracy.Text = resourceLoader.GetString("NoData");
                        }
                        break;
                }
            });
        }

        private void Success()
        {
            TestInfoSet.Compass.FinishTime = DateTime.Now;
            TestInfoSet.Compass.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Compass.FinishTime = DateTime.Now;
            TestInfoSet.Compass.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
