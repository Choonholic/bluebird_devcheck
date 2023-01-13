using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemTouchPage : Page
    {
        public ItemTouchPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TestInfoSet.Touch.Supported = true;
            TestInfoSet.Touch.Status = TestStatus.NotTested;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void StartTest()
        {
            TestInfoSet.Touch.Status = TestStatus.Testing;
            TestInfoSet.Touch.StartTime = DateTime.Now;

            MainPage.Current.NavigateToPage(typeof(ItemTouchSubPage));
        }

        private void Success()
        {
            TestInfoSet.Touch.FinishTime = DateTime.Now;
            TestInfoSet.Touch.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Touch.FinishTime = DateTime.Now;
            TestInfoSet.Touch.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
