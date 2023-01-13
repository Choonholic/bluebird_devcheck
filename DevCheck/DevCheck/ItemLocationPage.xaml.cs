using System;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemLocationPage : Page
    {
        private Geolocator _geolocator = null;

        public ItemLocationPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    {
                        var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                        _geolocator = new Geolocator { ReportInterval = 2000 };

                        _geolocator.PositionChanged += OnPositionChanged;
                        _geolocator.StatusChanged += OnStatusChanged;

                        ShowData(true);

                        OutputStatus.Text = resourceLoader.GetString("Unknown");
                        OutputLatitude.Text = resourceLoader.GetString("NoData");
                        OutputLongitude.Text = resourceLoader.GetString("NoData");
                        OutputAccuracy.Text = resourceLoader.GetString("NoData");

                        TestInfoSet.Location.Supported = true;
                        TestInfoSet.Location.Status = TestStatus.Testing;
                        TestInfoSet.Location.StartTime = DateTime.Now;
                    }
                    break;
                case GeolocationAccessStatus.Denied:
                    {
                        ShowData(false);

                        TestInfoSet.Location.Supported = false;
                        TestInfoSet.Location.Status = TestStatus.Tested;
                        TestInfoSet.Location.StartTime = DateTime.Now;
                        TestInfoSet.Location.FinishTime = DateTime.Now;
                    }
                    break;
                case GeolocationAccessStatus.Unspecified:
                    {
                        ShowData(true);

                        TestInfoSet.Location.Supported = false;
                        TestInfoSet.Location.Status = TestStatus.Tested;
                        TestInfoSet.Location.StartTime = DateTime.Now;
                        TestInfoSet.Location.FinishTime = DateTime.Now;
                    }
                    break;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (TestInfoSet.Location.Supported == true)
            {
                _geolocator.PositionChanged -= OnPositionChanged;
                _geolocator.StatusChanged -= OnStatusChanged;
            }

            _geolocator = null;

            base.OnNavigatingFrom(e);
        }

        private async void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateLocationData(e.Position);
            });
        }

        private async void OnStatusChanged(Geolocator sender, StatusChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ShowData(true);

                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                switch (e.Status)
                {
                    case PositionStatus.Ready:
                        {
                            OutputStatus.Text = resourceLoader.GetString("Ready");
                        }
                        break;
                    case PositionStatus.Initializing:
                        {
                            OutputStatus.Text = resourceLoader.GetString("Initializing");
                        }
                        break;
                    case PositionStatus.NoData:
                        {
                            OutputStatus.Text = resourceLoader.GetString("NoData");
                        }
                        break;
                    case PositionStatus.Disabled:
                        {
                            OutputStatus.Text = resourceLoader.GetString("Disabled");

                            ShowData(false);
                            UpdateLocationData(null);
                        }
                        break;
                    case PositionStatus.NotInitialized:
                        {
                            OutputStatus.Text = resourceLoader.GetString("NotInitialized");
                        }
                        break;
                    case PositionStatus.NotAvailable:
                        {
                            OutputStatus.Text = resourceLoader.GetString("NotAvailable");
                        }
                        break;
                    default:
                        {
                            OutputStatus.Text = resourceLoader.GetString("Unknown");
                        }
                        break;
                }
            });
        }

        private void UpdateLocationData(Geoposition position)
        {
            if (position == null)
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                OutputLatitude.Text = resourceLoader.GetString("NoData");
                OutputLongitude.Text = resourceLoader.GetString("NoData");
                OutputAccuracy.Text = resourceLoader.GetString("NoData");
            }
            else
            {
                OutputLatitude.Text = position.Coordinate.Point.Position.Latitude.ToString();
                OutputLongitude.Text = position.Coordinate.Point.Position.Longitude.ToString();
                OutputAccuracy.Text = position.Coordinate.Accuracy.ToString();
            }
        }

        private void ShowData(bool show)
        {
            if (show)
            {
                LocationMessageBody.Children.Clear();

                LocationData.Visibility = Visibility.Visible;
                LocationMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
                TextBlock textBlock = new TextBlock();
                Hyperlink hyperlink = new Hyperlink();
                Run runPrefix = new Run();
                Run runHyperlink = new Run();
                Run runPostfix = new Run();

                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                runPrefix.Text = resourceLoader.GetString("LocationFailedPrefix");
                runHyperlink.Text = resourceLoader.GetString("LocationFailedHyperlink");
                runPostfix.Text = resourceLoader.GetString("LocationFailedPostfix");

                hyperlink.NavigateUri = new Uri("ms-settings:privacy-location");

                hyperlink.Inlines.Add(runHyperlink);

                textBlock.Inlines.Add(runPrefix);
                textBlock.Inlines.Add(hyperlink);
                textBlock.Inlines.Add(runPostfix);

                textBlock.TextWrapping = TextWrapping.WrapWholeWords;

                LocationMessageBody.Children.Add(textBlock);

                LocationData.Visibility = Visibility.Collapsed;
                LocationMessage.Visibility = Visibility.Visible;
            }
        }

        private void Success()
        {
            TestInfoSet.Location.FinishTime = DateTime.Now;
            TestInfoSet.Location.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Location.FinishTime = DateTime.Now;
            TestInfoSet.Location.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
