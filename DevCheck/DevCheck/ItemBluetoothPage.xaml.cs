using System;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemBluetoothPage : Page
    {
        private DeviceWatcher deviceWatcher = null;
        private TypedEventHandler<DeviceWatcher, DeviceInformation> handlerAdded = null;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerUpdated = null;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerRemoved = null;
        private TypedEventHandler<DeviceWatcher, Object> handlerEnumCompleted = null;
        private TypedEventHandler<DeviceWatcher, Object> handlerStopped = null;

        public ObservableCollection<DeviceInformationDisplay> BluetoothCollection { get; private set; }

        public ItemBluetoothPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BluetoothCollection = new ObservableCollection<DeviceInformationDisplay>();

            DataContext = this;

            TestInfoSet.Bluetooth.Supported = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (null != deviceWatcher)
            {
                deviceWatcher.Added -= handlerAdded;
                deviceWatcher.Updated -= handlerUpdated;
                deviceWatcher.Removed -= handlerRemoved;
                deviceWatcher.EnumerationCompleted -= handlerEnumCompleted;

                if (DeviceWatcherStatus.Started == deviceWatcher.Status || DeviceWatcherStatus.EnumerationCompleted == deviceWatcher.Status)
                {
                    deviceWatcher.Stop();
                }

                deviceWatcher = null;
            }

            base.OnNavigatingFrom(e);
        }

        private void Scan()
        {
            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            ScanButton.IsEnabled = false;

            if (null != deviceWatcher)
            {
                deviceWatcher.Added -= handlerAdded;
                deviceWatcher.Updated -= handlerUpdated;
                deviceWatcher.Removed -= handlerRemoved;
                deviceWatcher.EnumerationCompleted -= handlerEnumCompleted;

                if (DeviceWatcherStatus.Started == deviceWatcher.Status || DeviceWatcherStatus.EnumerationCompleted == deviceWatcher.Status)
                {
                    deviceWatcher.Stop();
                }

                deviceWatcher = null;
            }

            BluetoothCollection.Clear();

            deviceWatcher = DeviceInformation.CreateWatcher("System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\"", null, DeviceInformationKind.AssociationEndpoint);

            handlerAdded = new TypedEventHandler<DeviceWatcher, DeviceInformation>(async (watcher, deviceInfo) =>
            {
                await MainPage.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    BluetoothCollection.Add(new DeviceInformationDisplay(deviceInfo));

                    DeviceMessage.Text = string.Format(resourceLoader.GetString("BluetoothDevicesFound"), BluetoothCollection.Count, resourceLoader.GetString((BluetoothCollection.Count < 2) ? "Device" : "Devices"));
                });
            });

            deviceWatcher.Added += handlerAdded;

            handlerUpdated = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                await MainPage.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    foreach (DeviceInformationDisplay deviceInfoDisp in BluetoothCollection)
                    {
                        if (deviceInfoDisp.Id == deviceInfoUpdate.Id)
                        {
                            deviceInfoDisp.Update(deviceInfoUpdate);
                            break;
                        }
                    }
                });
            });

            deviceWatcher.Updated += handlerUpdated;

            handlerRemoved = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                await MainPage.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    foreach (DeviceInformationDisplay deviceInfoDisp in BluetoothCollection)
                    {
                        if (deviceInfoDisp.Id == deviceInfoUpdate.Id)
                        {
                            BluetoothCollection.Remove(deviceInfoDisp);
                            break;
                        }

                        DeviceMessage.Text = string.Format(resourceLoader.GetString("BluetoothDevicesFound"), BluetoothCollection.Count, resourceLoader.GetString((BluetoothCollection.Count < 2) ? "Device" : "Devices"));
                    }
                });
            });

            deviceWatcher.Removed += handlerRemoved;

            handlerEnumCompleted = new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await MainPage.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    DeviceMessage.Text = string.Format(resourceLoader.GetString("BluetoothEnumerationCompleted"), BluetoothCollection.Count, resourceLoader.GetString((BluetoothCollection.Count < 2) ? "Device" : "Devices"));
                });
            });

            deviceWatcher.EnumerationCompleted += handlerEnumCompleted;

            handlerStopped = new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await MainPage.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    DeviceMessage.Text = string.Format(resourceLoader.GetString("BluetoothWatcherStopped"), BluetoothCollection.Count, resourceLoader.GetString((BluetoothCollection.Count < 2) ? "Device" : "Devices"), resourceLoader.GetString((DeviceWatcherStatus.Aborted == watcher.Status) ? "WatcherAborted" : "WatcherStopped"));
                });
            });

            deviceWatcher.Stopped += handlerStopped;

            DeviceMessage.Text = resourceLoader.GetString("BluetoothWatcherStarting");

            deviceWatcher.Start();

            TestInfoSet.Bluetooth.Status = TestStatus.Testing;
            TestInfoSet.Bluetooth.StartTime = DateTime.Now;

            ScanButton.IsEnabled = true;
        }

        private async void OpenBluetooth()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
        }

        private void Success()
        {
            TestInfoSet.Bluetooth.FinishTime = DateTime.Now;
            TestInfoSet.Bluetooth.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Bluetooth.FinishTime = DateTime.Now;
            TestInfoSet.Bluetooth.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
