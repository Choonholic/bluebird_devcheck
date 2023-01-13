using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace DevCheck.Model
{
    class ResultItem
    {
        #region Properties
        public string Title { get; set; }

        private TestStatus status;

        public TestStatus Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
                result = string.Empty;
                background = null;
            }
        }

        private string result;

        public string Result
        {
            get
            {
                if (result == string.Empty)
                {
                    var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader(); 

                    switch (Status)
                    {
                        case TestStatus.NotTested:
                            {
                                result = resourceLoader.GetString("NotTested");
                            }
                            break;
                        case TestStatus.Testing:
                            {
                                result = resourceLoader.GetString("Testing");
                            }
                            break;
                        case TestStatus.Tested:
                            {
                                result = resourceLoader.GetString("Tested");
                            }
                            break;
                        case TestStatus.Succeeded:
                            {
                                result = resourceLoader.GetString("Succeeded");
                            }
                            break;
                        case TestStatus.Failed:
                            {
                                result = resourceLoader.GetString("Failed");
                            }
                            break;
                    }
                }

                return result;
            }
        }

        private Brush background;

        public Brush Background
        {
            get
            {
                if (background == null)
                {
                    switch (Status)
                    {
                        case TestStatus.NotTested:
                            {
                                background = new SolidColorBrush(Colors.DarkGray);
                            }
                            break;
                        case TestStatus.Testing:
                            {
                                background = new SolidColorBrush(Colors.Green);
                            }
                            break;
                        case TestStatus.Tested:
                            {
                                background = new SolidColorBrush(Colors.DarkGreen);
                            }
                            break;
                        case TestStatus.Succeeded:
                            {
                                background = new SolidColorBrush(Colors.Blue);
                            }
                            break;
                        case TestStatus.Failed:
                            {
                                background = new SolidColorBrush(Colors.Red);
                            }
                            break;
                    }
                }

                return background;
            }
        }
        #endregion

        public ResultItem()
        {
            Title = string.Empty;
            status = TestStatus.NotTested;
            result = string.Empty;
            background = null;
        }

        #region Public Methods
        public static ObservableCollection<ResultItem> RetrieveResults()
        {
            ObservableCollection<ResultItem> resultItems = new ObservableCollection<ResultItem>();

            resultItems.Add(new ResultItem() { Title = TestInfoSet.Lcd.Title, Status = TestInfoSet.Lcd.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Battery.Title, Status = TestInfoSet.Battery.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Touch.Title, Status = TestInfoSet.Touch.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Accelerometer.Title, Status = TestInfoSet.Accelerometer.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Proximity.Title, Status = TestInfoSet.Proximity.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Location.Title, Status = TestInfoSet.Location.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Compass.Title, Status = TestInfoSet.Compass.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Light.Title, Status = TestInfoSet.Light.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.SdCard.Title, Status = TestInfoSet.SdCard.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Camera.Title, Status = TestInfoSet.Camera.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.WiFi.Title, Status = TestInfoSet.WiFi.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Bluetooth.Title, Status = TestInfoSet.Bluetooth.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Barcode.Title, Status = TestInfoSet.Barcode.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.LedAndVibrator.Title, Status = TestInfoSet.LedAndVibrator.Status });
            resultItems.Add(new ResultItem() { Title = TestInfoSet.Sam.Title, Status = TestInfoSet.Sam.Status });

#if MSR_ENABLED
            resultItems.Add(new ResultItem() { Title = TestInfoSet.MagneticStripeReader.Title, Status = TestInfoSet.MagneticStripeReader.Status });
#endif

            return resultItems;
        }

        public override string ToString()
        {
            return Title + " [" + Result + "]";
        }
#endregion
    }
}
