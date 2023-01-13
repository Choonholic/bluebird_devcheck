using System;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemCameraPage : Page
    {
        public ItemCameraPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TestInfoSet.Camera.Status = TestStatus.NotTested;
            TestInfoSet.Camera.Supported = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private async void OpenCamera()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

            TestInfoSet.Camera.Status = TestStatus.Tested;
            TestInfoSet.Camera.StartTime = DateTime.Now;

            await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            TestInfoSet.Camera.FinishTime = DateTime.Now;
        }

        private void Success()
        {
            TestInfoSet.Camera.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Camera.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
