using Hikeathon.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Security.Authentication.Web;
using Windows.UI.Popups;
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
    public sealed partial class InfoFb : Page, IWebAuthenticationContinuable
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public static string acc = "";
        public static string text = "";
        public InfoFb()
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
            if (acc.Equals("") || text.Equals(""))
                Launch();
            else { prg.IsActive = false; Web.Text = text; }
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        private void Launch()
        {

            
            try
            {
                String FacebookURL = "https://www.facebook.com/dialog/oauth?client_id=413037865516033&redirect_uri=http://localhost&scope=read_stream&display=popup&response_type=token";
                System.Uri StartUri = new Uri(FacebookURL);
                System.Uri EndUri = new Uri("http://localhost");
#if WINDOWS_PHONE_APP
                WebAuthenticationBroker.AuthenticateAndContinue(StartUri, EndUri, null, WebAuthenticationOptions.None);
#else
                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        StartUri,
                                                        EndUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    OutputToken(WebAuthenticationResult.ResponseData.ToString());
                    await GetFacebookUserNameAsync(WebAuthenticationResult.ResponseData.ToString());
                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    OutputToken("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                }
                else
                {
                    OutputToken("Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString());
                }
#endif
            }
            catch (Exception Error)
            {
                
            }
        }
        public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            WebAuthenticationResult result = args.WebAuthenticationResult;


            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                await GetFacebookUserNameAsync(result.ResponseData.ToString());
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                MessageDialog md = new MessageDialog("HTTP Error returned by AuthenticateAsync() : " + result.ResponseErrorDetail.ToString(),"Error");
                await md.ShowAsync();
            }
            else
            {
                MessageDialog md = new MessageDialog("Error returned by AuthenticateAsync() : " + result.ResponseStatus.ToString(), "Error");
                await md.ShowAsync();
            }
        }


        /// <summary>
        /// This function extracts access_token from the response returned from web authentication broker
        /// and uses that token to get user information using facebook graph api. 
        /// </summary>
        /// <param name="webAuthResultResponseData">responseData returned from AuthenticateAsync result.</param>
        private async Task GetFacebookUserNameAsync(string webAuthResultResponseData)
        {
            //Get Access Token first
            string responseData = webAuthResultResponseData.Substring(webAuthResultResponseData.IndexOf("access_token"));
            String[] keyValPairs = responseData.Split('&');
            string access_token = null;
            string expires_in = null;
            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "access_token":
                        access_token = splits[1];
                        break;
                    case "expires_in":
                        expires_in = splits[1];
                        break;
                }
            }
            acc = access_token;
            HttpClient httpClient = new HttpClient();
            string response = await httpClient.GetStringAsync(new Uri("https://graph.facebook.com/me?access_token=" + access_token));
            JsonObject value = JsonValue.Parse(response).GetObject();
            string response1 = await httpClient.GetStringAsync(new Uri("https://graph.facebook.com/me?fields=age_range,friends&access_token=" + access_token));
            JsonObject value1 = JsonValue.Parse(response1).GetObject();
            response=BuildInfo(value,value1);
            text = response;
            Web.Text = response;
            prg.IsActive = false;
        }
        private string BuildInfo(JsonObject ob,JsonObject ob1)
        {
            string str = "";
            try {
                str += "Name: " + ob.GetNamedString("name");
                try {str += Environment.NewLine + "Birthday: " + ob.GetNamedString("birthday"); } catch (Exception) { }
                try { str += Environment.NewLine + "Gender: " + ob.GetNamedString("gender"); } catch (Exception) { }
                try { str += Environment.NewLine + "Age: " + ob1.GetNamedObject("age_range").GetNamedNumber("min"); } catch (Exception) { }
                try { str += Environment.NewLine + "Number of Fb Friends: " + ob1.GetNamedObject("friends").GetNamedObject("summary").GetNamedNumber("total_count");} catch (Exception) { }
                try { str += Environment.NewLine + "Email: " + ob.GetNamedString("email");}catch (Exception) { }
                try {str += Environment.NewLine + "Relationship Status: " + ob.GetNamedString("relationship_status");} catch (Exception) { }
                try {str += Environment.NewLine + "Hometown: " + ob.GetNamedObject("hometown").GetNamedString("name"); } catch (Exception) { }
                try {str += Environment.NewLine + "Languages: " + getInfo(ob.GetNamedArray("languages"));} catch (Exception) { }
                try {str += Environment.NewLine + "Religion: " + ob.GetNamedString("religion");} catch (Exception) { }
                try {str += Environment.NewLine + "Location: " + ob.GetNamedObject("location").GetNamedString("name");} catch (Exception) { }
                try {str += Environment.NewLine + "Bio: " + ob.GetNamedString("bio");} catch (Exception) { }
                try {str += Environment.NewLine + "Quotes: " + ob.GetNamedString("quotes");} catch (Exception) { }
                try {str += Environment.NewLine + "Sports: " + getInfo(ob.GetNamedArray("sports"));} catch (Exception) { }
                try {str += Environment.NewLine + "Favourite Teams: " + getInfo(ob.GetNamedArray("favorite_teams")); } catch (Exception) { }
                try {str += Environment.NewLine + "Favourite Athletes: " + getInfo(ob.GetNamedArray("favorite_athletes"));} catch (Exception) { }
                try {str += Environment.NewLine + "Favourite Teams: " + getInfo(ob.GetNamedArray("favorite_teams"));} catch (Exception) { }
                try {str += Environment.NewLine + "Inspirational People: " + getInfo(ob.GetNamedArray("inspirational_people"));} catch (Exception) { }
                try { str += Environment.NewLine + "Education: " + removeInfo(ob.GetNamedArray("education")); } catch (Exception) { }
                try { str += Environment.NewLine + "Work: " + removeInfo(ob.GetNamedArray("work")); } catch (Exception) { }
            }
            catch (Exception e)
            {
                str += "Name: " + ob.GetNamedString("name");
                str += Environment.NewLine+ob.Stringify();
            }
            return str;
        }
        private string getInfo(JsonArray ob)
        {
            string s = "";
            for(uint i=0;i<ob.Count();i++)
            {
                s += ob.GetObjectAt(i).GetNamedString("name")+", ";
            }
            return s;
        }
        private string removeInfo(JsonArray ob)
        {
            string s = ob.Stringify();
            while(s.Contains("\"id\""))
            {
                string f=s.Substring(0,s.IndexOf("\"id\""));
                s=s.Substring(s.IndexOf("\"id\""));
                string l = s.Substring(24);
                s = f + l;
            }
            s = s.Replace(",", "");
            s = s.Replace("\"", "");
            s = s.Replace("}", Environment.NewLine);
            s=s.Replace("{", "");
            s=s.Replace("[", "");
            s=s.Replace("]", "");

            return s;
        }
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }
    }
}
