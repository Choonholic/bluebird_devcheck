using DevCheck.Model;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class TestResultsPage : Page
    {
        private ObservableCollection<ResultItem> ResultItems;

        public TestResultsPage()
        {
            this.InitializeComponent();

            ResultItems = ResultItem.RetrieveResults();

            ResultListView.ItemsSource = ResultItems;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void PerformTests()
        {
            MainPage.Current.NavigateScenario(1);
        }
    }
}
