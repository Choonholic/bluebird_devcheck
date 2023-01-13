using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCheck
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Configuration _configuration = new Configuration();

            _configuration.Load();
            _configuration.Apply();

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

                Window.Current.Activate();
            }
        }

        internal static async void ShowMessage(string content, string title)
        {
            var messageDialog = new MessageDialog(content, title);

            await messageDialog.ShowAsync();
        }

        private async void AskExit()
        {
            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var exitDialog = new MessageDialog(resourceLoader.GetString("ExitMessage"), resourceLoader.GetString("ExitTitle"));

            exitDialog.Commands.Add(new UICommand(resourceLoader.GetString("Yes"), (command) =>
            {
                Current.Exit();
            }));

            exitDialog.Commands.Add(new UICommand(resourceLoader.GetString("No"), (command) => 
            {
            }));

            exitDialog.DefaultCommandIndex = 1;

            await exitDialog.ShowAsync();
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                return;
            }

            e.Handled = true;

            if (rootFrame.SourcePageType == typeof(MainPage))
            {
                e.Handled = true;

                AskExit();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }
    }
}
