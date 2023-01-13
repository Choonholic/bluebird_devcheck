using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace DevCheck
{
    public partial class MainPage : Page
    {
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "DeviceInformation", ClassType = typeof(DeviceInformationPage) },
            new Scenario() { Title = "PerformTests", ClassType = typeof(PerformTestsPage) },
            new Scenario() { Title = "TestResults", ClassType = typeof(TestResultsPage) },
            new Scenario() { Title = "Settings", ClassType = typeof(SettingsPage) }
        };
    }

    public class Scenario
    {
        private string title;

        public string Title
        {
            get
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                return resourceLoader.GetString(title);
            }

            set
            {
                title = value;
            }
        }

        public Type ClassType { get; set; }
    }

    public class LanguageSelectorInfo
    {
        public string Name { get; set; }
        public string Culture { get; set; }
        public int Index { get; set; }

        public LanguageSelectorInfo()
        {
            Name = string.Empty;
            Culture = string.Empty;
            Index = (-1);
        }
    }

    public static class LanguageSelectorChoices
    {
        public static List<LanguageSelectorInfo> LanguageSelectors
        {
            get
            {
                List<LanguageSelectorInfo> selectors = new List<LanguageSelectorInfo>();

                selectors.Add(new LanguageSelectorInfo() { Name = "English (U.S.)", Culture = "en-US", Index = selectors.Count });
                selectors.Add(new LanguageSelectorInfo() { Name = "한국어", Culture = "ko-KR", Index = selectors.Count });

                return selectors;
            }
        }
    }

    public class WiFiNetworkDisplay : INotifyPropertyChanged
    {
        private WiFiAdapter adapter;

        public WiFiNetworkDisplay(WiFiAvailableNetwork availableNetwork, WiFiAdapter adapter)
        {
            AvailableNetwork = availableNetwork;
            this.adapter = adapter;

            UpdateWiFiImage();
        }

        private void UpdateWiFiImage()
        {
            string imageFileNamePrefix = "secure";

            if (AvailableNetwork.SecuritySettings.NetworkAuthenticationType == NetworkAuthenticationType.Open80211)
            {
                imageFileNamePrefix = "open";
            }

            string imageFileName = string.Format("ms-appx:/Assets/{0}_{1}bar.png", imageFileNamePrefix, AvailableNetwork.SignalBars);

            WiFiImage = new BitmapImage(new Uri(imageFileName));

            OnPropertyChanged("WiFiImage");
        }

        public async Task UpdateConnectivityLevel()
        {
            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            string connectivityLevel = resourceLoader.GetString("WiFiNone");
            string connectedSsid = null;

            var connectedProfile = await adapter.NetworkAdapter.GetConnectedProfileAsync();

            if (connectedProfile != null && connectedProfile.IsWlanConnectionProfile && connectedProfile.WlanConnectionProfileDetails != null)
            {
                connectedSsid = connectedProfile.WlanConnectionProfileDetails.GetConnectedSsid();
            }

            if (!string.IsNullOrEmpty(connectedSsid))
            {
                if (connectedSsid.Equals(AvailableNetwork.Ssid))
                {

                    switch (connectedProfile.GetNetworkConnectivityLevel())
                    {
                        case NetworkConnectivityLevel.None:
                            {
                                connectivityLevel = resourceLoader.GetString("WiFiNone");
                            }
                            break;
                        case NetworkConnectivityLevel.LocalAccess:
                            {
                                connectivityLevel = resourceLoader.GetString("WiFiLocalAccess");
                            }
                            break;
                        case NetworkConnectivityLevel.ConstrainedInternetAccess:
                            {
                                connectivityLevel = resourceLoader.GetString("WiFiConstrainedInternetAccess");
                            }
                            break;
                        case NetworkConnectivityLevel.InternetAccess:
                            {
                                connectivityLevel = resourceLoader.GetString("WiFiInternetAccess");
                            }
                            break;
                    }
                }
            }

            ConnectivityLevel = connectivityLevel;

            OnPropertyChanged("ConnectivityLevel");
        }

        public String Ssid
        {
            get
            {
                return availableNetwork.Ssid;
            }
        }

        public String Bssid
        {
            get
            {
                return availableNetwork.Bssid;

            }
        }

        public String ChannelCenterFrequency
        {
            get
            {
                return string.Format("{0}kHz", availableNetwork.ChannelCenterFrequencyInKilohertz);
            }
        }

        public String Rssi
        {
            get
            {
                return string.Format("{0}dBm", availableNetwork.NetworkRssiInDecibelMilliwatts);
            }
        }

        public String SecuritySettings
        {
            get
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                return string.Format(resourceLoader.GetString("WiFiStatus"), availableNetwork.SecuritySettings.NetworkAuthenticationType, availableNetwork.SecuritySettings.NetworkEncryptionType);
            }
        }

        public String ConnectivityLevel
        {
            get;
            private set;
        }

        public BitmapImage WiFiImage
        {
            get;
            private set;
        }

        private WiFiAvailableNetwork availableNetwork;

        public WiFiAvailableNetwork AvailableNetwork
        {
            get
            {
                return availableNetwork;
            }

            private set
            {
                availableNetwork = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async void OnPropertyChanged(string name)
        {
            var rootPage = MainPage.Current;

            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChangedEventHandler handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            });
        }
    }

    public class DeviceInformationDisplay : INotifyPropertyChanged
    {
        private DeviceInformation deviceInfo;

        public DeviceInformationDisplay(DeviceInformation deviceInfoIn)
        {
            deviceInfo = deviceInfoIn;

            UpdateGlyphBitmapImage();
        }

        public DeviceInformationKind Kind
        {
            get
            {
                return deviceInfo.Kind;
            }
        }

        public string Id
        {
            get
            {
                return deviceInfo.Id;
            }
        }

        public string Name
        {
            get
            {
                return deviceInfo.Name;
            }
        }

        public BitmapImage GlyphBitmapImage { get; private set; }

        public bool CanPair
        {
            get
            {
                return deviceInfo.Pairing.CanPair;
            }
        }

        public bool IsPaired
        {
            get
            {
                return deviceInfo.Pairing.IsPaired;
            }
        }

        public IReadOnlyDictionary<string, object> Properties
        {
            get
            {
                return deviceInfo.Properties;
            }
        }

        public DeviceInformation DeviceInformation
        {
            get
            {
                return deviceInfo;
            }

            private set
            {
                deviceInfo = value;
            }
        }

        public void Update(DeviceInformationUpdate deviceInfoUpdate)
        {
            deviceInfo.Update(deviceInfoUpdate);

            OnPropertyChanged("Kind");
            OnPropertyChanged("Id");
            OnPropertyChanged("Name");
            OnPropertyChanged("DeviceInformation");
            OnPropertyChanged("CanPair");
            OnPropertyChanged("IsPaired");

            UpdateGlyphBitmapImage();
        }

        private async void UpdateGlyphBitmapImage()
        {
            DeviceThumbnail deviceThumbnail = await deviceInfo.GetGlyphThumbnailAsync();
            BitmapImage glyphBitmapImage = new BitmapImage();

            await glyphBitmapImage.SetSourceAsync(deviceThumbnail);

            GlyphBitmapImage = glyphBitmapImage;

            OnPropertyChanged("GlyphBitmapImage");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class GeneralPropertyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            object property = null;

            if (value is IReadOnlyDictionary<string, object> && parameter is string && !String.IsNullOrEmpty((string)parameter))
            {
                IReadOnlyDictionary<string, object> properties = value as IReadOnlyDictionary<string, object>;
                string propertyName = parameter as string;

                property = properties[propertyName];
            }

            return property;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
