using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemSdCardPage : Page
    {
        public ItemSdCardPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            StorageFolder externalDevices = KnownFolders.RemovableDevices;
            StorageFolder sdCard = (await externalDevices.GetFoldersAsync()).FirstOrDefault();

            TestInfoSet.SdCard.Status = TestStatus.Tested;

            if (sdCard != null)
            {
                TestInfoSet.SdCard.Supported = true;
                TestInfoSet.SdCard.StartTime = DateTime.Now;
                TestInfoSet.SdCard.FinishTime = DateTime.Now;

                Description.Text = string.Empty;

                Description.Text += resourceLoader.GetString("SdCardName") + " " + sdCard.DisplayName + "\n";
                Description.Text += resourceLoader.GetString("SdCardId") + " " + sdCard.FolderRelativeId + "\n";
                Description.Text += resourceLoader.GetString("SdCardProvider") + " " + sdCard.Provider + "\n";
                Description.Text += resourceLoader.GetString("SdCardPath") + " " + sdCard.Path + "\n";
            }
            else
            {
                TestInfoSet.SdCard.Supported = false;
                TestInfoSet.SdCard.StartTime = DateTime.Now;
                TestInfoSet.SdCard.FinishTime = DateTime.Now;

                Description.Text = resourceLoader.GetString("SdCardNotSupported");
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void Success()
        {
            TestInfoSet.SdCard.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.SdCard.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
