using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Maps;
using Microsoft.Phone.Shell;
using FootSteps.Resources;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using FootSteps.DataModels;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Threading;

namespace FootSteps
{
    public partial class ConnectPage : PhoneApplicationPage
    {
        private bool tracking = false;
        string groupId;        
        double lat=0;
        double lon=0;
        Location destinationLocation = null;
        Geolocator geolocator = null;
        GeoCoordinate watcher2;
        List<Person> allPeople = null;
        async void connect_PositionChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();            
            dict.Add("groupId",groupId);
            JToken token = await App.serviceClient.InvokeApiAsync("getPeopleInGroup", System.Net.Http.HttpMethod.Get, dict);
            allPeople=JsonConvert.DeserializeObject<List<Person>>(token.ToString());

                // watcher2 = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);               
                while (this.map.Children.Count != 0)
                {
                    var pushpin = map.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && ((Pushpin)p).Tag == "locationPushpin"));

                    if (pushpin != null)
                    {
                        this.map.Children.Remove(pushpin);
                    }
                }

                

                foreach (Person person in allPeople)
                {

                    if ((person.latitude <= 180 && person.latitude >= -180) && (person.longitude <= 180 && person.latitude >= -180))
                    {
                        watcher2 = new GeoCoordinate(person.latitude, person.longitude);
                        Pushpin locationPushpin = new Pushpin();
                        locationPushpin.Tag = "locationPushpin";
                        locationPushpin.Location = watcher2;
                        locationPushpin.Content = person.name;

                        this.map.Children.Add(locationPushpin);
                        this.map.SetView(watcher2, 1.0);
                    }                    
                }
                if (destinationLocation!=null && App.isCoordinatesValid(destinationLocation.locationLat,destinationLocation.locationLong))
                {
                    watcher2 = new GeoCoordinate(destinationLocation.locationLat, destinationLocation.locationLong);
                    Pushpin locationPushpin = new Pushpin();
                    locationPushpin.Tag = "locationPushpin";
                    locationPushpin.Location = watcher2;
                    locationPushpin.Content = destinationLocation.locationName;

                    this.map.Children.Add(locationPushpin);
                    this.map.SetView(watcher2, 1.0);
                }                          

        }

        private void TrackLocation_Click()
        {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                return;
            }

            if (!tracking)
            {
                geolocator = new Geolocator();
                geolocator.DesiredAccuracy = PositionAccuracy.High;
                geolocator.MovementThreshold = 1; // The units are meters.

                geolocator.PositionChanged += geolocator_PositionChanged;

                tracking = true;
            }
            else
            {
                geolocator.PositionChanged -= geolocator_PositionChanged;
                geolocator = null;

                tracking = false;
            }
        }

        async void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            //  this.map.Center = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

                lat = args.Position.Coordinate.Latitude;
                lon = args.Position.Coordinate.Longitude;
                if (App.client != null)
                {
                    App.client.latitude = lat;
                    App.client.longitude = lon;
                    Dictionary<string, string> dict2 = new Dictionary<string, string>();
                    dict2.Add("personStr", JsonConvert.SerializeObject(App.client));
                    await App.serviceClient.InvokeApiAsync("updateLocation", System.Net.Http.HttpMethod.Get, dict2);
                }                                
        }

        void updater()
        {
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(connect_PositionChanged);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (PhoneApplicationService.Current.State.ContainsKey("groupId"))
            {
                groupId=(string)PhoneApplicationService.Current.State["groupId"];
            }
            updater();
            await configureAppBarButtonsAndHandlers();

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("groupId", groupId);
            JToken destLatToken = await App.serviceClient.InvokeApiAsync("getDestinationForGroup", System.Net.Http.HttpMethod.Get, dict);
            dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(destLatToken.ToString());
            destinationLocation = new Location();
            destinationLocation.locationLat = double.Parse(dict["destinationLatitude"]);
            destinationLocation.locationLong = double.Parse(dict["destinationLongitude"]);
            destinationLocation.locationName = dict["destinationName"];
            TrackLocation_Click();
        }

        
       async void join_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("groupId", groupId);
            dict.Add("personId", App.client.Id);
            dict.Add("addMyself", JsonConvert.SerializeObject(true));
            await App.serviceClient.InvokeApiAsync("addConnect2", System.Net.Http.HttpMethod.Get, dict);
            await configureAppBarButtonsAndHandlers();
        }
       async void meet_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("groupId", groupId);
            dict.Add("personAdminId", App.client.Id);
            dict.Add("destinationLat", destinationLocation.locationLat.ToString());
            dict.Add("destinationLong", destinationLocation.locationLong.ToString());
            await App.serviceClient.InvokeApiAsync("addConnectForWholeGroup", System.Net.Http.HttpMethod.Get, dict);
            await configureAppBarButtonsAndHandlers();


           // Navigate to the suggestion page
            dict.Clear();
            dict.Add("groupId", groupId);
            JToken token = await App.serviceClient.InvokeApiAsync("getPeopleInGroup", System.Net.Http.HttpMethod.Get, dict);
            allPeople = JsonConvert.DeserializeObject<List<Person>>(token.ToString());
            PhoneApplicationService.Current.State["allPeople"] = allPeople;
            PhoneApplicationService.Current.State["groupId"] = groupId;
            NavigationService.Navigate(new Uri("/MainPages/SuggestionPage.xaml", UriKind.Relative));

        }
        async void quitConnect_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();                        
            dict.Add("personId", App.client.Id);
            dict.Add("groupId", groupId);
            await App.serviceClient.InvokeApiAsync("deleteConnectUsingGroupAndPerson", System.Net.Http.HttpMethod.Get, dict);
            await configureAppBarButtonsAndHandlers();
        }
        async void endConnect_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("groupId", groupId);
            await App.serviceClient.InvokeApiAsync("deleteConnectUsingGroupId", System.Net.Http.HttpMethod.Get, dict);
            await configureAppBarButtonsAndHandlers();            
        }

        async Task configureAppBarButtonsAndHandlers()
        {

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Click -= join_Click;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Click -= meet_Click;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Click -= quitConnect_Click;

            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Click -= endConnect_Click;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("groupId", groupId);
            dict.Add("personId", App.client.Id);
            JToken stateToken = await App.serviceClient.InvokeApiAsync("getMeeetingStateUsingPersonAndGroup", System.Net.Http.HttpMethod.Get, dict);
            State participantsState = JsonConvert.DeserializeObject<State>(stateToken.ToString());

            if (!participantsState.personInMeeting)
            {
                if (participantsState.groupInMeeting)
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Click += join_Click;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = "Join";
                }
                else if (!participantsState.groupInMeeting)
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Click += meet_Click;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = "Meet";
                }
            }
            else
            {
                if (participantsState.personAndGroupInSameMeeting)
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Click += quitConnect_Click;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Click += endConnect_Click;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = "Quit Connect";
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = "End Connect";
                }
                else if (participantsState.groupInMeeting)
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Click += join_Click;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = "Join";
                }
                else if (!participantsState.groupInMeeting)
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Click += meet_Click;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = "Meet";
                }
            }

        }
        public ConnectPage()
        {
            InitializeComponent();            
        }

        private void create_click(object sender, EventArgs e) {

        }      
     
    }
    


}