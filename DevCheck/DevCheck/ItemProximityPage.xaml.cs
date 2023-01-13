using System;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemProximityPage : Page
    {
        private ProximitySensor sensor;
        private ProximitySensorDisplayOnOffController displayController;
        private DeviceWatcher watcher;

        public ItemProximityPage()
        {
            this.InitializeComponent();

            watcher = DeviceInformation.CreateWatcher(ProximitySensor.GetDeviceSelector());
            watcher.Added += OnProximitySensorAdded;

            watcher.Start();
        }

        private async void OnProximitySensorAdded(DeviceWatcher sender, DeviceInformation device)
        {
            if (null == sensor)
            {
                ProximitySensor foundSensor = ProximitySensor.FromId(device.Id);

                if (null != foundSensor)
                {
                    sensor = foundSensor;

                    TestInfoSet.Proximity.Supported = true;
                    TestInfoSet.Proximity.Status = TestStatus.NotTested;
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                        Description.Text = resourceLoader.GetString("ProximityNotSupported");

                        TestInfoSet.Proximity.Supported = false;
                        TestInfoSet.Proximity.Status = TestStatus.Tested;
                    });
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (null != sensor)
            {
                displayController = sensor.CreateDisplayOnOffController();

                TestInfoSet.Proximity.Supported = true;
                TestInfoSet.Proximity.Status = TestStatus.Testing;
                TestInfoSet.Proximity.StartTime = DateTime.Now;
            }
            else
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                Description.Text = resourceLoader.GetString("ProximityNotSupported");

                TestInfoSet.Proximity.Supported = false;
                TestInfoSet.Proximity.Status = TestStatus.Tested;
                TestInfoSet.Proximity.StartTime = DateTime.Now;
                TestInfoSet.Proximity.FinishTime = DateTime.Now;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (null != displayController)
            {
                displayController.Dispose();
                displayController = null;
            }

            base.OnNavigatingFrom(e);
        }

        private void Success()
        {
            TestInfoSet.Proximity.FinishTime = DateTime.Now;
            TestInfoSet.Proximity.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Proximity.FinishTime = DateTime.Now;
            TestInfoSet.Proximity.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
