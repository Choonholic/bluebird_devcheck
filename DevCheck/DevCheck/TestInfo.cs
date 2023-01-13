using System;

namespace DevCheck
{
    public enum TestStatus
    {
        NotTested,
        Testing,
        Tested,
        Succeeded,
        Failed
    };

    public class TestInfo
    {
        private string title;

        public string Title
        {
            get
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                return resourceLoader.GetString(title);
            }

            set
            {
                title = value;
            }
        }

        public Type TestType { get; set; }
        public TestStatus Status { get; set; }
        public bool Supported { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        private string resultText;

        public string ResultText
        {
            get
            {
                return resultText;
            }

            set
            {
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                if (Title != string.Empty)
                {
                    resultText = "[" + Title + "] / " + resourceLoader.GetString(Supported ? "Supported" : "Not Supported") + " / " + Status.ToString() + " / " + StartTime.ToString() + " / " + FinishTime.ToString();
                }
                else
                {
                    resultText = string.Empty; 
                }
            }
        }

        public TestInfo()
        {
            title = string.Empty;
            TestType = null;
            Status = TestStatus.NotTested;
            Supported = false;
            StartTime = DateTime.MinValue;
            FinishTime = DateTime.MinValue;
            resultText = string.Empty;
        }

        public void Reset()
        {
            Status = TestStatus.NotTested;
            Supported = false;
            StartTime = DateTime.MinValue;
            FinishTime = DateTime.MinValue;
            resultText = string.Empty;
        }
    }

    public static class TestInfoSet
    {
        public static int TestItemCount = 20;
        public static int CurrentTest = 0;

        public static Type[] TestSet = new Type[TestItemCount + 1];
        public static bool[] TestQueue = new bool[TestItemCount + 1];

        public static TestInfo Lcd = new TestInfo() { Title = "Lcd", TestType = typeof(ItemLcdPage) };
        public static TestInfo Battery = new TestInfo() { Title = "Battery", TestType = typeof(ItemBatteryPage) };
        public static TestInfo Touch = new TestInfo() { Title = "Touch", TestType = typeof(ItemTouchPage) };
        public static TestInfo Accelerometer = new TestInfo() { Title = "Accelerometer", TestType = typeof(ItemAccelerometerPage) };
        public static TestInfo Proximity = new TestInfo() { Title = "Proximity", TestType = typeof(ItemProximityPage) };
        public static TestInfo Location = new TestInfo() { Title = "Location", TestType = typeof(ItemLocationPage) };
        public static TestInfo Compass = new TestInfo() { Title = "Compass", TestType = typeof(ItemCompassPage) };
        public static TestInfo Light = new TestInfo() { Title = "Light", TestType = typeof(ItemLightPage) };
        public static TestInfo SdCard = new TestInfo() { Title = "SdCard", TestType = typeof(ItemSdCardPage) };
        public static TestInfo Camera = new TestInfo() { Title = "Camera", TestType = typeof(ItemCameraPage) };
        public static TestInfo WiFi = new TestInfo() { Title = "WiFi", TestType = typeof(ItemWiFiPage) };
        public static TestInfo Bluetooth = new TestInfo() { Title = "Bluetooth", TestType = typeof(ItemBluetoothPage) };
        public static TestInfo Barcode = new TestInfo() { Title = "Barcode", TestType = typeof(ItemBarcodePage) };
        public static TestInfo LedAndVibrator = new TestInfo() { Title = "LEDAndVibrator", TestType = typeof(ItemLedAndVibratorPage) };
        public static TestInfo Sam = new TestInfo() { Title = "Sam", TestType = typeof(ItemSamPage) };
        public static TestInfo MagneticStripeReader = new TestInfo() { Title = "MagneticStripeReader", TestType = typeof(ItemMagneticStripeReaderPage) };

        private static void SetSentinel()
        {
            TestQueue[TestItemCount] = true;
            TestSet[TestItemCount] = typeof(TestResultsPage);
        }

        public static void Initialize()
        {
            SetSentinel();

            TestSet[0] = Lcd.TestType;
            TestSet[1] = Battery.TestType;
            TestSet[2] = Touch.TestType;
            TestSet[3] = Accelerometer.TestType;
            TestSet[4] = Proximity.TestType;
            TestSet[5] = Location.TestType;
            TestSet[6] = Compass.TestType;
            TestSet[7] = Light.TestType;
            TestSet[8] = SdCard.TestType;
            TestSet[9] = Camera.TestType;
            TestSet[10] = WiFi.TestType;
            TestSet[11] = Bluetooth.TestType;
            TestSet[12] = Barcode.TestType;
            TestSet[13] = LedAndVibrator.TestType;
            TestSet[14] = Sam.TestType;

#if MSR_ENABLED
            TestSet[19] = MagneticStripeReader.TestType;
#endif
        }

        public static void ResetTestResults()
        {
            Lcd.Reset();
            Battery.Reset();
            Touch.Reset();
            Accelerometer.Reset();
            Proximity.Reset();
            Location.Reset();
            Compass.Reset();
            Light.Reset();
            SdCard.Reset();
            Camera.Reset();
            WiFi.Reset();
            Bluetooth.Reset();
            Barcode.Reset();
            LedAndVibrator.Reset();
            Sam.Reset();
            MagneticStripeReader.Reset();
        }

        public static void ResetTestQueue()
        {
            for (int i = 0; i < TestItemCount; i++)
            {
                TestQueue[i] = false;
            }
        }

        public static int FirstTest()
        {
            CurrentTest = 0;

            while (TestQueue[CurrentTest] == false)
            {
                CurrentTest++;

                if (CurrentTest > TestItemCount)
                {
                    CurrentTest = TestItemCount;
                }
            }

            return CurrentTest;
        }

        public static int PreviousTest()
        {
            do
            {
                CurrentTest--;

                if (CurrentTest < 0)
                {
                    CurrentTest = 0;
                }
            } while (TestQueue[CurrentTest] == false);

            return CurrentTest;
        }

        public static int NextTest()
        {
            do
            {
                CurrentTest++;

                if (CurrentTest > TestItemCount)
                {
                    CurrentTest = TestItemCount;
                }
            } while (TestQueue[CurrentTest] == false);

            return CurrentTest;
        }

        public static int LastTest()
        {
            CurrentTest = TestItemCount;

            return CurrentTest;
        }
    }
}
