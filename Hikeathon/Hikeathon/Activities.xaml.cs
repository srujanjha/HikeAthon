using Hikeathon.Common;
using Hikeathon.Data;
using Lumia.Sense;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Lumia.Sense.Testing;
using System.Diagnostics;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Hikeathon
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Activities : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Activities()
        {
            this.InitializeComponent();
            DataContext = MyData.Instance();

            Loaded += MainPage_Loaded;

            //Using this method to detect if the application runs in the emulator or on a real device. Later the *Simulator API is used to read fake sense data on emulator. 
            //In production code you do not need this and in fact you should ensure that you do not include the Lumia.Sense.Test reference in your project.
            EasClientDeviceInformation x = new EasClientDeviceInformation();
            if (x.SystemProductName.StartsWith("Virtual"))
            {
                _runningInEmulator = true;
            }
            this.navigationHelper = new NavigationHelper(this);
        }
        #region Private members
        /// <summary>
        /// Activity monitor instance
        /// </summary>
        private IActivityMonitor _activityMonitor = null;
        private bool _runningInEmulator = false;
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        
        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_runningInEmulator && !await ActivityMonitor.IsSupportedAsync())
            {
                // nothing to do if we cannot use the API
                // In a real app please do make an effort to create a better user experience.
                // e.g. if access to Activity Monitor is not mission critical, let the app run without showing any error message
                // simply hide the features that depend on it
                MessageDialog md = new MessageDialog(this.resourceLoader.GetString("FeatureNotSupported/Message"), this.resourceLoader.GetString("FeatureNotSupported/Title"));
                await md.ShowAsync();
            }

            Initialize();
        }

        /// <summary>
        /// Initializes activity monitor
        /// </summary>
        private async void Initialize()
        {
            if (_activityMonitor == null)
            {
                if (_runningInEmulator)
                {
                    await CallSensorCoreApiAsync(async () => { _activityMonitor = await ActivityMonitorSimulator.GetDefaultAsync(); });
                }
                else
                {
                    await CallSensorCoreApiAsync(async () => { _activityMonitor = await ActivityMonitor.GetDefaultAsync(); });
                }

                if (_activityMonitor != null)
                {
                    // Set activity observer
                    _activityMonitor.ReadingChanged += activityMonitor_ReadingChanged;
                    _activityMonitor.Enabled = true;

                    // read current activity
                    ActivityMonitorReading reading = null;
                    if (await CallSensorCoreApiAsync(async () => { reading = await _activityMonitor.GetCurrentReadingAsync(); }))
                    {
                        if (reading != null)
                        {
                            MyData.Instance().ActivityEnum = reading.Mode;
                        }
                    }

                    // read logged data
                    PollHistory();
                }
                else
                {
                    // nothing to do if we cannot use the API
                    // in a real app do make an effort to make the user experience better
                }

                // Must call DeactivateAsync() when the application goes to background
                Window.Current.VisibilityChanged += async (sender, args) =>
                {
                    if (_activityMonitor != null)
                    {
                        await CallSensorCoreApiAsync(async () =>
                        {
                            if (!args.Visible)
                            {
                                await _activityMonitor.DeactivateAsync();
                            }
                            else
                            {
                                await _activityMonitor.ActivateAsync();
                            }
                        });
                    }
                };
            }

        }

        /// <summary>
        /// Performs asynchronous Sense SDK operation and handles any exceptions
        /// </summary>
        /// <param name="action"></param>
        /// <returns><c>true</c> if call was successful, <c>false</c> otherwise</returns>
        private async Task<bool> CallSensorCoreApiAsync(Func<Task> action)
        {
            Exception failure = null;
            try
            {
                await action();
            }
            catch (Exception e)
            {
                failure = e;
            }

            if (failure != null)
            {
                try
                {
                    MessageDialog dialog = null;
                    switch (SenseHelper.GetSenseError(failure.HResult))
                    {
                        case SenseError.LocationDisabled:
                            dialog = new MessageDialog(this.resourceLoader.GetString("FeatureDisabled/Location"), this.resourceLoader.GetString("FeatureDisabled/Title"));
                            dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(async (cmd) => await SenseHelper.LaunchLocationSettingsAsync())));
                            dialog.Commands.Add(new UICommand("No"));
                            await dialog.ShowAsync();
                            new System.Threading.ManualResetEvent(false).WaitOne(500);
                            return false;

                        case SenseError.SenseDisabled:
                            dialog = new MessageDialog(this.resourceLoader.GetString("FeatureDisabled/MotionData"), this.resourceLoader.GetString("FeatureDisabled/InTitle"));
                            dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(async (cmd) => await SenseHelper.LaunchSenseSettingsAsync())));
                            dialog.Commands.Add(new UICommand("No"));
                            await dialog.ShowAsync();
                            new System.Threading.ManualResetEvent(false).WaitOne(500);
                            return false;

                        default:
                            dialog = new MessageDialog("Failure: " + SenseHelper.GetSenseError(failure.HResult), "");
                            await dialog.ShowAsync();
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Called when navigating to this page
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_activityMonitor != null)
            {
                _activityMonitor.ReadingChanged += activityMonitor_ReadingChanged;
            }
        }

        /// <summary>
        /// Called when navigating from this page
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_activityMonitor != null)
            {
                _activityMonitor.ReadingChanged -= activityMonitor_ReadingChanged;
            }
        }

        /// <summary>
        /// Called when activity changes
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="args">Event arguments</param>
        private async void activityMonitor_ReadingChanged(IActivityMonitor sender, ActivityMonitorReading args)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MyData.Instance().ActivityEnum = args.Mode;
            });
        }

        /// <summary>
        /// Poll history, read the data for the past day
        /// </summary>
        private async void PollHistory()
        {
            if (_activityMonitor != null)
            {
                if (!await CallSensorCoreApiAsync(async () =>
                {
                    // get the data for the current 24h time window
                    MyData.Instance().History = await _activityMonitor.GetActivityHistoryAsync(DateTime.Now.Date.AddDays(MyData.Instance().TimeWindow), new TimeSpan(24, 0, 0));
                }))
                {
                   
                }
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PollHistory();
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            // move the time window 24 to the past
            MyData.Instance().PreviousDay();
            nextButton.IsEnabled = true;
            prevButton.IsEnabled = MyData.Instance().TimeWindow > -10;
            refreshButton.IsEnabled = false;
            PollHistory();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            // move the time window 24h to the present
            MyData.Instance().NextDay();
            nextButton.IsEnabled = MyData.Instance().TimeWindow < 0;
            prevButton.IsEnabled = true;
            refreshButton.IsEnabled = MyData.Instance().TimeWindow == 0;
            PollHistory();
        }

    }
}

