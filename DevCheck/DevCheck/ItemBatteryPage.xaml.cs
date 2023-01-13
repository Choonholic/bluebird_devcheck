using System;
using Windows.Devices.Power;
using Windows.System.Power;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    public sealed partial class ItemBatteryPage : Page
    {
        bool reportRequested = false;

        public ItemBatteryPage()
        {
            this.InitializeComponent();

            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;

            GetBatteryReport();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TestInfoSet.Battery.Status = TestStatus.Tested;
            TestInfoSet.Battery.Supported = true;
            TestInfoSet.Battery.StartTime = DateTime.Now;
            TestInfoSet.Battery.FinishTime = DateTime.Now;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void GetBatteryReport()
        {
            Output.Children.Clear();
            RequestAggregateBatteryReport();

            reportRequested = true;
        }

        private void RequestAggregateBatteryReport()
        {
            var aggBattery = Battery.AggregateBattery;
            var report = aggBattery.GetReport();

            if (report.Status == Windows.System.Power.BatteryStatus.NotPresent)
            {
                TestInfoSet.Battery.Supported = false;
            }

            AddReportUI(Output, report, aggBattery.DeviceId);
        }

        private string getBatteryStatus(BatteryReport report)
        {
            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            string status = string.Empty;

            switch (report.Status)
            {
                case BatteryStatus.NotPresent:
                    {
                        status = resourceLoader.GetString("BatteryNotPresent");
                    }
                    break;
                case BatteryStatus.Discharging:
                    {
                        status = resourceLoader.GetString("BatteryDischarging");
                    }
                    break;
                case BatteryStatus.Idle:
                    {
                        status = resourceLoader.GetString("BatteryIdle");
                    }
                    break;
                case BatteryStatus.Charging:
                    {
                        status = resourceLoader.GetString("BatteryCharging");
                    }
                    break;
            }

            return status;
        }

        private void AddReportUI(StackPanel sp, BatteryReport report, string DeviceID)
        {
            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            TextBlock txt1 = new TextBlock { Text = resourceLoader.GetString("BatteryDeviceId") + DeviceID };
            txt1.FontSize = 15;
            txt1.Margin = new Thickness(0, 15, 0, 0);
            txt1.TextWrapping = TextWrapping.WrapWholeWords;

            TextBlock txt2 = new TextBlock { Text = resourceLoader.GetString("BatteryStatus") + getBatteryStatus(report) };
            txt2.Margin = new Thickness(0, 0, 0, 15);

            TextBlock txt3 = new TextBlock { Text = resourceLoader.GetString("BatteryChargeRate") + report.ChargeRateInMilliwatts.ToString() };
            TextBlock txt4 = new TextBlock { Text = resourceLoader.GetString("BatteryDesignCapacity") + report.DesignCapacityInMilliwattHours.ToString() };
            TextBlock txt5 = new TextBlock { Text = resourceLoader.GetString("BatteryFullChargeCapacity") + report.FullChargeCapacityInMilliwattHours.ToString() };
            TextBlock txt6 = new TextBlock { Text = resourceLoader.GetString("BatteryRemainingCapacity") + report.RemainingCapacityInMilliwattHours.ToString() };

            TextBlock pbLabel = new TextBlock { Text = resourceLoader.GetString("BatteryPercent") };
            pbLabel.Margin = new Thickness(0, 10, 0, 5);
            pbLabel.FontFamily = new FontFamily("Segoe UI");
            pbLabel.FontSize = 11;

            ProgressBar pb = new ProgressBar();
            pb.Margin = new Thickness(0, 5, 0, 0);
            pb.Width = 200;
            pb.Height = 10;
            pb.IsIndeterminate = false;
            pb.HorizontalAlignment = HorizontalAlignment.Left;

            TextBlock pbPercent = new TextBlock();
            pbPercent.Margin = new Thickness(0, 5, 0, 10);
            pbPercent.FontFamily = new FontFamily("Segoe UI");
            pbLabel.FontSize = 11;

            if ((report.FullChargeCapacityInMilliwattHours == null) || (report.RemainingCapacityInMilliwattHours == null))
            {
                pb.IsEnabled = false;
                pbPercent.Text = resourceLoader.GetString("BatteryNotApplicable");
            }
            else
            {
                pb.IsEnabled = true;
                pb.Maximum = Convert.ToDouble(report.FullChargeCapacityInMilliwattHours);
                pb.Value = Convert.ToDouble(report.RemainingCapacityInMilliwattHours);
                pbPercent.Text = ((pb.Value / pb.Maximum) * 100).ToString("F2") + "%";
            }

            sp.Children.Add(txt1);
            sp.Children.Add(txt2);
            sp.Children.Add(txt3);
            sp.Children.Add(txt4);
            sp.Children.Add(txt5);
            sp.Children.Add(txt6);
            sp.Children.Add(pbLabel);
            sp.Children.Add(pb);
            sp.Children.Add(pbPercent);
        }

        async private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            if (reportRequested)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Output.Children.Clear();
                    RequestAggregateBatteryReport();
                });
            }
        }

        private void Success()
        {
            TestInfoSet.Battery.FinishTime = DateTime.Now;
            TestInfoSet.Battery.Status = TestStatus.Succeeded;

            MainPage.Current.NavigateNextTest();
        }

        private void Failure()
        {
            TestInfoSet.Battery.FinishTime = DateTime.Now;
            TestInfoSet.Battery.Status = TestStatus.Failed;

            MainPage.Current.NavigateNextTest();
        }
    }
}
