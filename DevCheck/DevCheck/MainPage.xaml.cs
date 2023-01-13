using Bluebird;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        public DisplayInformation displayInfo;
        public Device _device = new Device();

        public MainPage()
        {
            this.InitializeComponent();

            Current = this;
            NavigationCacheMode = NavigationCacheMode.Disabled;

            TestInfoSet.Initialize();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            PackageVersion packageVersion = Package.Current.Id.Version;

            BuildNumber.Text = string.Format("{0} {1}.{2}.{3}.{4}", resourceLoader.GetString("Build"), packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);

            displayInfo = DisplayInformation.GetForCurrentView();

            displayInfo.OrientationChanged += OnOrientationChanged;

            ScenarioControl.ItemsSource = scenarios;
            ScenarioControl.SelectedIndex = 0;

            SetPaneOpen(false);

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void OnOrientationChanged(DisplayInformation sender, object e)
        {
            SetPaneOpen(Splitter.IsPaneOpen);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void SetPaneOpen(bool isPaneOpen)
        {
            if (IsMobileApp())
            {
                Splitter.IsPaneOpen = isPaneOpen;

                if (isPaneOpen == true)
                {
                    ScenarioFrame.Opacity = 0.1;
                }
                else
                {
                    ScenarioFrame.Opacity = 1.0;
                }
            }
            else
            {
                if (Window.Current.Bounds.Width >= 640)
                {
                    isPaneOpen = true;
                }

                Splitter.IsPaneOpen = isPaneOpen;
            }

            Thickness margin = FooterPanel.Margin;

            margin.Bottom = 0;

            if (Splitter.IsPaneOpen)
            {
                Type PageType = ScenarioFrame.SourcePageType;

                if ((PageType == typeof(SettingsPage)) || (PageType == typeof(PerformTestsPage)))
                {
                    margin.Bottom = 56;
                }
            }

            FooterPanel.Margin = margin;
        }

        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox scenarioListBox = sender as ListBox;
            Scenario s = scenarioListBox.SelectedItem as Scenario;

            if (s != null)
            {
                ScenarioFrame.Navigate(s.ClassType);
                SetPaneOpen(false);
            }
        }

        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }

        private void HamburgerClick(object sender, RoutedEventArgs e)
        {
            SetPaneOpen(!Splitter.IsPaneOpen);
        }

        public void EnableHeaderPanel(bool enable)
        {
            HeaderPanel.Visibility = (enable ? Visibility.Visible : Visibility.Collapsed);
        }

        public async void EnableStatusBar(bool enable)
        {
            if (IsMobileApp())
            {
                var statusBar = StatusBar.GetForCurrentView();

                if (enable)
                {
                    await statusBar.ShowAsync();
                }
                else
                {
                    await statusBar.HideAsync();
                }
            }
        }

        public void NavigateScenario(int index)
        {
            ScenarioControl.SelectedIndex = index;

            switch (index)
            {
                case 0:
                    {
                        ScenarioFrame.Navigate(typeof(DeviceInformationPage));
                    }
                    break;
                case 1:
                    {
                        ScenarioFrame.Navigate(typeof(PerformTestsPage));
                    }
                    break;
                case 2:
                    {
                        ScenarioFrame.Navigate(typeof(TestResultsPage));
                    }
                    break;
                case 3:
                    {
                        ScenarioFrame.Navigate(typeof(SettingsPage));
                    }
                    break;
            }
        }

        public void NavigateToPage(Type pageType)
        {
            ScenarioFrame.Navigate(pageType);
        }

        public void NavigateFirstTest()
        {
            Type TestType = TestInfoSet.TestSet[TestInfoSet.FirstTest()];

            if (TestType == typeof(TestResultsPage))
            {
                ScenarioControl.SelectedIndex = 2;
            }

            ScenarioFrame.Navigate(TestType);
        }

        public void NavigatePreviousTest()
        {
            Type TestType = TestInfoSet.TestSet[TestInfoSet.PreviousTest()];

            if (TestType == typeof(TestResultsPage))
            {
                ScenarioControl.SelectedIndex = 2;
            }

            ScenarioFrame.Navigate(TestType);
        }

        public void NavigateNextTest()
        {
            Type TestType = TestInfoSet.TestSet[TestInfoSet.NextTest()];

            if (TestType == typeof(TestResultsPage))
            {
                ScenarioControl.SelectedIndex = 2;
            }

            ScenarioFrame.Navigate(TestType);
        }

        public void NavigateLastTest()
        {
            Type TestType = TestInfoSet.TestSet[TestInfoSet.LastTest()];

            if (TestType == typeof(TestResultsPage))
            {
                ScenarioControl.SelectedIndex = 2;
            }

            ScenarioFrame.Navigate(TestType);
        }

        public bool IsMobileApp()
        {
            return (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1));
        }
    }

    public class ScenarioBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Scenario s = value as Scenario;
            return s.Title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}
