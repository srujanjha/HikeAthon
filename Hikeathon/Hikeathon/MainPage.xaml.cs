using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Hikeathon
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        bool _checkInternet = true;
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        private void checkInternet()
        {
            _checkInternet = true;
            string connectionProfileInfo = string.Empty;
            try
            {
                ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

                if (InternetConnectionProfile == null)
                {
                    _checkInternet = false;
                    NotifyUser("Not connected to Internet\n");
                }
                else
                {
                    connectionProfileInfo = GetConnectionProfile(InternetConnectionProfile);
                    NotifyUser("Internet connection profile = " + connectionProfileInfo);
                }
            }
            catch (Exception ex)
            {
                NotifyUser("Unexpected exception occurred: " + ex.ToString());
            }
        }
        string GetConnectionProfile(ConnectionProfile connectionProfile)
        {
            string connectionProfileInfo = string.Empty;
            if (connectionProfile != null)
            {
                connectionProfileInfo = "Profile Name : " + connectionProfile.ProfileName + "\n";

                switch (connectionProfile.GetNetworkConnectivityLevel())
                {
                    case NetworkConnectivityLevel.None:
                        connectionProfileInfo += "Connectivity Level : None\n";
                        break;
                    case NetworkConnectivityLevel.LocalAccess:
                        connectionProfileInfo += "Connectivity Level : Local Access\n";
                        break;
                    case NetworkConnectivityLevel.ConstrainedInternetAccess:
                        connectionProfileInfo += "Connectivity Level : Constrained Internet Access\n";
                        break;
                    case NetworkConnectivityLevel.InternetAccess:
                        connectionProfileInfo += "Connectivity Level : Internet Access\n";
                        break;
                }

                switch (connectionProfile.GetDomainConnectivityLevel())
                {
                    case DomainConnectivityLevel.None:
                        connectionProfileInfo += "Domain Connectivity Level : None\n";
                        break;
                    case DomainConnectivityLevel.Unauthenticated:
                        connectionProfileInfo += "Domain Connectivity Level : Unauthenticated\n";
                        break;
                    case DomainConnectivityLevel.Authenticated:
                        connectionProfileInfo += "Domain Connectivity Level : Authenticated\n";
                        break;
                }

                //Get Connection Cost information
                ConnectionCost connectionCost = connectionProfile.GetConnectionCost();
                connectionProfileInfo += GetConnectionCostInfo(connectionCost);

                //Get Dataplan Status information
                DataPlanStatus dataPlanStatus = connectionProfile.GetDataPlanStatus();
                connectionProfileInfo += GetDataPlanStatusInfo(dataPlanStatus);

            }
            return connectionProfileInfo;
        }
        string GetConnectionCostInfo(ConnectionCost connectionCost)
        {
            string cost = string.Empty;
            cost += "Connection Cost Information: \n";
            cost += "====================\n";

            if (connectionCost == null)
            {
                cost += "Connection Cost not available\n";
                return cost;
            }

            switch (connectionCost.NetworkCostType)
            {
                case NetworkCostType.Unrestricted:
                    cost += "Cost: Unrestricted";
                    break;
                case NetworkCostType.Fixed:
                    cost += "Cost: Fixed";
                    break;
                case NetworkCostType.Variable:
                    cost += "Cost: Variable";
                    break;
                case NetworkCostType.Unknown:
                    cost += "Cost: Unknown";
                    break;
                default:
                    cost += "Cost: Error";
                    break;
            }
            cost += "\n";
            cost += "Roaming: " + connectionCost.Roaming + "\n";
            cost += "Over Data Limit: " + connectionCost.OverDataLimit + "\n";
            cost += "Approaching Data Limit : " + connectionCost.ApproachingDataLimit + "\n";

            return cost;
        }

        // Get DataPlanStatus information for a profile
        string GetDataPlanStatusInfo(DataPlanStatus dataPlan)
        {
            string dataplanStatusInfo = string.Empty;
            dataplanStatusInfo = "Dataplan Status Information:\n";
            dataplanStatusInfo += "====================\n";

            if (dataPlan == null)
            {
                dataplanStatusInfo += "Dataplan Status not available\n";
                return dataplanStatusInfo;
            }

            if (dataPlan.DataPlanUsage != null)
            {
                dataplanStatusInfo += "Usage In Megabytes : " + dataPlan.DataPlanUsage.MegabytesUsed + "\n";
                dataplanStatusInfo += "Last Sync Time : " + dataPlan.DataPlanUsage.LastSyncTime + "\n";
            }
            else
            {
                dataplanStatusInfo += "Usage In Megabytes : Not Defined\n";
            }

            ulong? inboundBandwidth = dataPlan.InboundBitsPerSecond;
            if (inboundBandwidth.HasValue)
            {
                dataplanStatusInfo += "InboundBitsPerSecond : " + inboundBandwidth + "\n";
            }
            else
            {
                dataplanStatusInfo += "InboundBitsPerSecond : Not Defined\n";
            }

            ulong? outboundBandwidth = dataPlan.OutboundBitsPerSecond;
            if (outboundBandwidth.HasValue)
            {
                dataplanStatusInfo += "OutboundBitsPerSecond : " + outboundBandwidth + "\n";
            }
            else
            {
                dataplanStatusInfo += "OutboundBitsPerSecond : Not Defined\n";
            }

            uint? dataLimit = dataPlan.DataLimitInMegabytes;
            if (dataLimit.HasValue)
            {
                dataplanStatusInfo += "DataLimitInMegabytes : " + dataLimit + "\n";
            }
            else
            {
                dataplanStatusInfo += "DataLimitInMegabytes : Not Defined\n";
            }

            System.DateTimeOffset? nextBillingCycle = dataPlan.NextBillingCycle;
            if (nextBillingCycle.HasValue)
            {
                dataplanStatusInfo += "NextBillingCycle : " + nextBillingCycle + "\n";
            }
            else
            {
                dataplanStatusInfo += "NextBillingCycle : Not Defined\n";
            }

            uint? maxTransferSize = dataPlan.MaxTransferSizeInMegabytes;
            if (maxTransferSize.HasValue)
            {
                dataplanStatusInfo += "MaxTransferSizeInMegabytes : " + maxTransferSize + "\n";
            }
            else
            {
                dataplanStatusInfo += "MaxTransferSizeInMegabytes : Not Defined\n";
            }
            return dataplanStatusInfo;
        }

        private void NotifyUser(string v)
        {
            Notify.Text = v;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                string rb = Windows.Phone.Devices.Power.Battery.GetDefault().RemainingChargePercent.ToString();
                TimeSpan dta = Windows.Phone.Devices.Power.Battery.GetDefault().RemainingDischargeTime;
                string dt =dta.TotalMinutes + "minutes";
                Battery.Text = "Battery Status: " + Environment.NewLine + "Charge Percent: " + rb +"%"+ Environment.NewLine + "Remaining Discharge Time: " + dt;
            }
            catch (Exception) { }
            checkInternet();
        }

        private async void Fb_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_checkInternet)
                Frame.Navigate(typeof(InfoFb));
            else
            {
                MessageDialog ob = new MessageDialog("You are not coonected to internet.", "Error");
                await ob.ShowAsync();
                checkInternet();
            }
        }

        private void Activities_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Activities));
        }
        static bool bat = false;
        private void Battery_Button_Click(object sender, RoutedEventArgs e)
        {
            if (bat) { Battery.Visibility = Visibility.Collapsed; bat = false; }
            else { Battery.Visibility = Visibility.Visible; bat = true; }
        }
        static bool net = false;
        private void Network_Button_Click(object sender, RoutedEventArgs e)
        {
            if (net) { Notify.Visibility = Visibility.Collapsed;net = false; }
            else { Notify.Visibility = Visibility.Visible;net = true; }
        }
        private void Places_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MapPage));
        }
        private void Call(object sender, RoutedEventArgs e)
        {
            AppBarButton ell = sender as AppBarButton;
            int i = Convert.ToInt16(ell.Tag.ToString());
            switch (i)
            {
                case 11: Action_Mail("Srujan Jha"); break;
                case 12: Action_Message("Srujan Jha", "+919010718698"); break;
                case 13: Action_Fb("srujanjha.jha"); break;
            }
        }
        private async void Action_Mail(String Name)
        {
            await Launcher.LaunchUriAsync(new Uri("mailto:srujanjha.jha@gmail.com"));
        }
        private async void Action_Fb(String Profile)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.facebook.com/" + Profile));
        }
        private async void Action_Message(String Name, String Number)
        {
            Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
            msg.Body = "Hi " + Name + ",";
            msg.Recipients.Add(Number);
            await Windows.ApplicationModel.Chat.ChatMessageManager
                     .ShowComposeSmsMessageAsync(msg);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }
    }
}
