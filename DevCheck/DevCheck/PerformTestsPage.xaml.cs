using DevCheck.Model;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class PerformTestsPage : Page
    {
        private ObservableCollection<TestItem> TestItems;

        public PerformTestsPage()
        {
            this.InitializeComponent();

            TestItems = TestItem.InitializeTests();

            TestListView.ItemsSource = TestItems;

            TestListView.SelectAll();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void StartTests(object sender, RoutedEventArgs e)
        {
            TestInfoSet.ResetTestResults();
            TestInfoSet.ResetTestQueue();

            if (TestListView.SelectedItems.Count > 0)
            {
                foreach (TestItem testItem in TestListView.SelectedItems)
                {
                    TestInfoSet.TestQueue[testItem.TestIndex] = true;
                }

                MainPage.Current.NavigateFirstTest();
            }
            else
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                App.ShowMessage(resourceLoader.GetString("NeedToSelect"), resourceLoader.GetString("Error"));
            }
        }

        private void SelectAllItems(object sender, RoutedEventArgs e)
        {
            TestListView.SelectAll();
        }

        private void CancelSelection(object sender, RoutedEventArgs e)
        {
            TestListView.SelectedItems.Clear();
        }
    }
}
