using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class SettingsPage : Page
    {
        Configuration _configuration = new Configuration();

        private string _currentCulture = string.Empty;
        private bool _currentRotationLock = false;

        public SettingsPage()
        {
            this.InitializeComponent();

            _configuration = new Configuration();

            LanguageSelector.ItemsSource = LanguageSelectorChoices.LanguageSelectors;
            LanguageSelector.SelectedIndex = 0;

            DisplayRotationLock.IsOn = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _configuration.Load();

            _currentCulture = _configuration.Culture;
            _currentRotationLock = _configuration.RotationLock;

            LanguageSelector.SelectedIndex = 0;

            foreach (LanguageSelectorInfo info in LanguageSelectorChoices.LanguageSelectors)
            {
                if (info.Culture == _configuration.Culture)
                {
                    LanguageSelector.SelectedIndex = info.Index;
                    break;
                }
            }

            DisplayRotationLock.IsOn = _configuration.RotationLock;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            foreach (LanguageSelectorInfo info in LanguageSelectorChoices.LanguageSelectors)
            {
                if (info.Index == LanguageSelector.SelectedIndex)
                {
                    _configuration.Culture = info.Culture;
                    break;
                }
            }

            if (_configuration.Culture != _currentCulture)
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                App.ShowMessage(resourceLoader.GetString("NeedToRestartDueToChangingLanguage"), resourceLoader.GetString("Notification"));
            }

            _configuration.RotationLock = DisplayRotationLock.IsOn;

            _configuration.Save();
            _configuration.Apply();

            MainPage.Current.Frame.Navigate(typeof(UpdatingPage));
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            MainPage.Current.NavigateScenario(1);
        }
    }
}
