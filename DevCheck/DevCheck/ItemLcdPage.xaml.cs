using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemLcdPage : Page
    {
        private int index = 0;

        public ItemLcdPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            StartTestButton.IsEnabled = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void StartTest()
        {
            TestArea.Width = Window.Current.Bounds.Width * 2;
            TestArea.Height = Window.Current.Bounds.Height * 2;
            TestArea.Visibility = Visibility.Visible;
            Actions.Visibility = Visibility.Collapsed;

            MainPage.Current.EnableStatusBar(false);
            MainPage.Current.EnableHeaderPanel(false);

            TestInfoSet.Lcd.Status = TestStatus.Testing;
            TestInfoSet.Lcd.Supported = true;
            TestInfoSet.Lcd.StartTime = DateTime.Now;
            TestInfoSet.Lcd.ResultText = string.Empty;

            index = 0;

            StartTestButton.IsEnabled = false;

            TestArea_Tapped(null, null);
        }

        private void TestArea_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Brush brush;

            switch (index)
            {
                case 0:
                    {
                        brush = new SolidColorBrush(Colors.White);
                        TestArea.Fill = brush;
                        index++;
                    }
                    break;
                case 1:
                    {
                        brush = new SolidColorBrush(Colors.Black);
                        TestArea.Fill = brush;
                        index++;
                    }
                    break;
                case 2:
                    {
                        brush = new SolidColorBrush(Colors.Red);
                        TestArea.Fill = brush;
                        index++;
                    }
                    break;
                case 3:
                    {
                        brush = new SolidColorBrush(Colors.Green);
                        TestArea.Fill = brush;
                        index++;
                    }
                    break;
                case 4:
                    {
                        brush = new SolidColorBrush(Colors.Blue);
                        TestArea.Fill = brush;
                        index++;
                    }
                    break;
                case 5:
                    {
                        brush = new SolidColorBrush(Colors.Yellow);
                        TestArea.Fill = brush;
                        index++;
                    }
                    break;
                default:
                    {
                        var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                        StartTestButton.Content = resourceLoader.GetString("RestartTest");
                        TestArea.Visibility = Visibility.Collapsed;
                        Actions.Visibility = Visibility.Visible;

                        StartTestButton.IsEnabled = true;

                        index = 0;

                        TestInfoSet.Lcd.Status = TestStatus.Tested;
                        TestInfoSet.Lcd.FinishTime = DateTime.Now;

                        MainPage.Current.EnableHeaderPanel(true);
                        MainPage.Current.EnableStatusBar(true);
                    }
                    break;
            }
        }

        private void Success()
        {
            TestInfoSet.Lcd.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Lcd.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
