/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Footstepsv2
{
    public partial class MapPage : PhoneApplicationPage
    {
        public MapPage()
        {
            InitializeComponent();
        }
    }
}

*/

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
using Footstepsv3.Resources;

using System.Threading.Tasks;
using Windows.Devices.Geolocation;

using System.IO.IsolatedStorage;
using Footstepsv3.dataModels;
using Microsoft.WindowsAzure.MobileServices;

/*
 
 * 
 * using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;

 */

namespace Footstepsv3
{
    public partial class MapPage : PhoneApplicationPage
    {

        Geolocator geolocator = null;
        bool tracking = false;
        GeoCoordinate watcher;
        string phoneNo;
        double lat;
        double lon;

        // Constructor
        public MapPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {


            base.OnNavigatedTo(e);
            string msg = "";

            if (NavigationContext.QueryString.TryGetValue("msg", out msg))

                phoneNo = msg;

            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                return;
            }
            else
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
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        private void TrackLocation_Click(object sender, RoutedEventArgs e)
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
                geolocator.MovementThreshold = 100; // The units are meters.

                geolocator.StatusChanged += geolocator_StatusChanged;
                geolocator.PositionChanged += geolocator_PositionChanged;

                tracking = true;
                TrackLocationButton.Content = "stop tracking";
            }
            else
            {
                geolocator.PositionChanged -= geolocator_PositionChanged;
                geolocator.StatusChanged -= geolocator_StatusChanged;
                geolocator = null;

                tracking = false;
                TrackLocationButton.Content = "track location";
                StatusTextBlock.Text = "stopped";
            }
        }

        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status = "";

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    // the application does not have the right capability or the location master switch is off
                    status = "location is disabled in phone settings";
                    break;
                case PositionStatus.Initializing:
                    // the geolocator started the tracking operation
                    status = "initializing";
                    break;
                case PositionStatus.NoData:
                    // the location service was not able to acquire the location
                    status = "no data";
                    break;
                case PositionStatus.Ready:
                    // the location service is generating geopositions as specified by the tracking parameters
                    status = "ready";
                    break;
                case PositionStatus.NotAvailable:
                    status = "not available";
                    // not used in WindowsPhone, Windows desktop uses this value to signal that there is no hardware capable to acquire location information
                    break;
                case PositionStatus.NotInitialized:
                    // the initial state of the geolocator, once the tracking operation is stopped by the user the geolocator moves back to this state

                    break;
            }

            Dispatcher.BeginInvoke(() =>
            {
                StatusTextBlock.Text = status;
            });
        }

        /*public async void newFunction(PositionChangedEventArgs args)
             {

                LatitudeTextBlock.Text = args.Position.Coordinate.Latitude.ToString("0.00");
                LongitudeTextBlock.Text = args.Position.Coordinate.Longitude.ToString("0.00");

                this.map.Center = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

                watcher= new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

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
                locationPushpin.Location = watcher;

                await App.serviceClient.InvokeApiAsync<Person,object>("updatelocation",new Person(){

                    Id=phoneNo,
                    phoneNo=phoneNo,
                    latitude=lat,
                    longitude=lon

                });

                List<Person> allPeople= await App.serviceClient.GetTable<Person>().ToListAsync();
                foreach (var  person in allPeople){
                    
                }
                

                this.map.Children.Add(locationPushpin);
                this.map.SetView(watcher, 18.0);


            }
         
       */


        async void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            //  this.map.Center = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

            Dispatcher.BeginInvoke(() =>
            {

                LatitudeTextBlock.Text = args.Position.Coordinate.Latitude.ToString("0.00");
                LongitudeTextBlock.Text = args.Position.Coordinate.Longitude.ToString("0.00");

                this.map.Center = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

                watcher = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

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
                locationPushpin.Location = watcher;
                locationPushpin.Content = "You";

                lat = args.Position.Coordinate.Latitude;
                lon = args.Position.Coordinate.Longitude;


                this.map.Children.Add(locationPushpin);
                this.map.SetView(watcher, 18.0);


            });

            if(App.client!=null){
                App.client.latitude=lat;
                App.client.longitude=lon;
                await App.serviceClient.InvokeApiAsync<Person, object>("updateLocation", App.client);
            }           
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ConnectPage.xaml", UriKind.Relative));
        }

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

    }
}