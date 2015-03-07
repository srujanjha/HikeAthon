using Hikeathon.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Hikeathon
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public AboutPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string fb = "Facebook Graph API:" + Environment.NewLine + "No SDK has been provided yet for Windows Phone 8.1. So, I used normal Graph Search using WebAuthenticationBroker API of WP-8.1."+Environment.NewLine+"The Graph API is the primary way to get data in and out of Facebook's social graph. It's a low - level HTTP - based API that you can use to query data, post new stories, upload photos and a variety of other tasks that an app might need to do.This guide will teach you how to accomplish all these things in the Graph API.";
            string ac = "Activity Monitor API:" + Environment.NewLine + "Activity Monitor API provides information about changes in user’s physical activity; for example, when user starts or stops walking.In order to filter out noise there is always a small delay associated with detecting changes in user’s activity.A typical delay is about 5 - 10 seconds.Activity information is most accurate when the device follows user’s movement closely, such as when the device is in user’s pocket.There are some challenging cases for Activity Monitor, such as when the user is in a vehicle that is generating strong vibrations." + Environment.NewLine +
"Activities are classified as follows:" + Environment.NewLine +
  "Idle(the phone is laying still, for example, on a table)." + Environment.NewLine +
 "Stationary(the phone is being handled / is in active use but the user is not moving around, or there is only some minor movement while the phone is, for example, in the user’s pocket)." + Environment.NewLine +
"Walking(the user is walking with the phone)." + Environment.NewLine +
"Running(the user is running with the phone)." + Environment.NewLine +
"Moving(the user is moving but Activity Monitor cannot identify the exact type of motion)." + Environment.NewLine +
"Activity Monitor is able to detect walking and running.If user is moving in such a way that Activity Monitor is unable to classify the movement as either walking or running, it will classify it as moving." + Environment.NewLine +
"Activity Monitor API provides real time information, foreground change notifications, and a ten day history of activity changes." + Environment.NewLine;
            string pl= "Place Monitor API:" + Environment.NewLine + "The Place Monitor API provides a collection of places where the user has spent time: be it jogging, working, shopping, enjoying a coffee, or being stuck in a traffic jam.The Place Monitor API provides geo-coordinates of places the user visits; including ‘home’ and ‘work’." + Environment.NewLine +
                "Place Monitor API is designed to provide this data with minimum battery consumption and it works passively in the background. This means that the logic behind the API is not actively using GPS to find the location of the device, instead it uses mainly cell towers and Wi - Fi hotspots to acquire location information.Because the Place Monitor is a passive information collector by nature, it will not provide real - time data or real - time location information. For real-time tracking scenarios, apps should employ GPS tracker or set geo - fences around the places which are recognised by the Place Monitor." + Environment.NewLine +
"Place Monitor API is able to recognize the following types of places:" + Environment.NewLine +
"Known" + Environment.NewLine +
            "Frequent" + Environment.NewLine +
    "Home" + Environment.NewLine +
    "Work" + Environment.NewLine;
            fbt.Text = fb;
            act.Text = ac;
            plt.Text = pl;
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
