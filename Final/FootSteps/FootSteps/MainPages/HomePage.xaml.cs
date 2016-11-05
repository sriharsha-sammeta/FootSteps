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

using System.Threading.Tasks;
using Windows.Devices.Geolocation;

using System.IO.IsolatedStorage;
using Microsoft.WindowsAzure.MobileServices;
using FootSteps.DataModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FootSteps.MainPages {
    public partial class HomePage : PhoneApplicationPage {
        Geolocator geolocator = null;
        bool tracking = false;
        GeoCoordinate watcher;
        string phoneNo;
        double lat;
        double lon;
        
        public HomePage() {
            InitializeComponent();
        }

        /////
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            string msg = "";

            if (NavigationContext.QueryString.TryGetValue("msg", out msg))

                phoneNo = msg;

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                    return;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            TrackLocation_Click();
        }

        private void TrackLocation_Click() {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true) {
                // The user has opted out of Location.
                return;
            }

            if (!tracking) {
                geolocator = new Geolocator();
                geolocator.DesiredAccuracy = PositionAccuracy.High;
                geolocator.MovementThreshold = 1; // The units are meters.
         
                geolocator.PositionChanged += geolocator_PositionChanged;

                tracking = true;                
            } else {
                geolocator.PositionChanged -= geolocator_PositionChanged;
                geolocator = null;

                tracking = false;                                
            }
        }        

        async void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args) {
            //  this.map.Center = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

            Dispatcher.BeginInvoke(() =>{
                
                this.map.Center = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

                watcher = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

                if (this.map.Children.Count != 0) {
                    var pushpin = map.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && ((Pushpin)p).Tag == "locationPushpin"));

                    if (pushpin != null) {
                        this.map.Children.Remove(pushpin);
                    }
                }

                Pushpin locationPushpin = new Pushpin();
                locationPushpin.Tag = "locationPushpin";
                locationPushpin.Location = watcher;
                locationPushpin.Content = "You";

                lat = args.Position.Coordinate.Latitude;
                lon = args.Position.Coordinate.Longitude;


                if (App.client != null)
                {
                    App.client.latitude = lat;
                    App.client.longitude = lon;
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("personStr", JsonConvert.SerializeObject(App.client));
                    App.serviceClient.InvokeApiAsync("updateLocation", System.Net.Http.HttpMethod.Get, dict);
                }
                this.map.Children.Add(locationPushpin);
                this.map.SetView(watcher, 18.0);


            });

           

        }


        //************Appbar functionalities
        private void contacts_clickHandler(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/MainPages/ContactsPage.xaml", UriKind.Relative));
        }

        private void groups_clickHandler(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/MainPages/GroupsPage.xaml", UriKind.Relative));
        }

        private void settings_clickHandler(object sender, EventArgs e) {
            //NavigationService.Navigate(new Uri("/RegistrationPages/PasscodePage.xaml", UriKind.Relative));
        }

        private void logout_clickHandler(object sender, EventArgs e) {
            //NavigationService.Navigate(new Uri("/RegistrationPages/PasscodePage.xaml", UriKind.Relative));
            LocalDB.deleteLocalPerson();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
         
        }
        //************************
        /*   void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
           {
               if (e.Position.Location.IsUnknown)
               {
                   MessageBox.Show("Please wait while your prosition is determined....");
                   return;
               }

               this.map.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

               if (this.map.Children.Count != 0)
               {
                   var pushpin = map.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && ((Pushpin)p).Tag == "locationPushpin"));

                   if (pushpin != null)
                   {
                       this.map.Children.Remove(pushpin);
                   }
               }

               Pushpin locationPushpin = new Pushpin();
               locationPushpin.Tag = "locationPushpin";
               locationPushpin.Location = geolocator.Position.Location;
               this.map.Children.Add(locationPushpin);
               this.map.SetView(geolocator.Position.Location, 18.0);

           }*/
        /////
    }
}