using System.Collections.ObjectModel;

namespace DevCheck.Model
{
    public class TestItem
    {
        #region Properties
        public string Title { get; set; }
        public int TestIndex { get; set; }
        #endregion

        public TestItem()
        {
            Title = string.Empty;
            TestIndex = (-1);
        }

        #region Public Methods
        public static ObservableCollection<TestItem> InitializeTests()
        {
            ObservableCollection<TestItem> testItems = new ObservableCollection<TestItem>();

            testItems.Add(new TestItem() { Title = TestInfoSet.Lcd.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Battery.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Touch.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Accelerometer.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Proximity.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Location.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Compass.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Light.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.SdCard.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Camera.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.WiFi.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Bluetooth.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Barcode.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.LedAndVibrator.Title, TestIndex = testItems.Count });
            testItems.Add(new TestItem() { Title = TestInfoSet.Sam.Title, TestIndex = testItems.Count });

#if MSR_ENABLED
            testItems.Add(new TestItem() { Title = TestInfoSet.MagneticStripeReader.Title, TestIndex = testItems.Count });
#endif

            return testItems;
        }

        public override string ToString()
        {
            return "[" + TestIndex + "] " + Title;
        }
#endregion
    }
}
