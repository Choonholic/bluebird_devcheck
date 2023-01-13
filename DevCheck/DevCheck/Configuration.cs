using System.Globalization;
using Windows.Globalization;
using Windows.Graphics.Display;
using Windows.Storage;

namespace DevCheck
{
    public class Configuration
    {
        private ApplicationDataContainer localSettings = null;

        public string Culture { get; set; }
        public bool RotationLock { get; set; }

        public Configuration()
        {
            localSettings = ApplicationData.Current.LocalSettings;
        }

        public void Apply()
        {
            CultureInfo _culture = new CultureInfo(Culture);

            ApplicationLanguages.PrimaryLanguageOverride = Culture;

            CultureInfo.DefaultThreadCurrentCulture = _culture;
            CultureInfo.DefaultThreadCurrentUICulture = _culture;

            DisplayInformation.AutoRotationPreferences = ((RotationLock == true) ? DisplayOrientations.Portrait : DisplayOrientations.Portrait | DisplayOrientations.Landscape);
        }

        public void Load()
        {
            if (localSettings.Values.ContainsKey("Culture"))
            {
                Culture = (string)localSettings.Values["Culture"];
            }
            else
            {
                Culture = "ko-KR";
            }

            if (localSettings.Values.ContainsKey("RotationLock"))
            {
                RotationLock = (bool)localSettings.Values["RotationLock"];
            }
            else
            {
                RotationLock = true;
            }
        }

        public void Save()
        {
            localSettings.Values["Culture"] = Culture;
            localSettings.Values["RotationLock"] = RotationLock;
        }
    }
}
