using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FootSteps.Resources;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Maps;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using FootSteps.DataModels;
using Microsoft.Phone.Controls.Maps.Platform;

//using Microsoft.Maps.MapControl;



namespace FootSteps
{

    public partial class SuggestionPage : PhoneApplicationPage
    {
        string groupId = null;
        public SuggestionPage()
        {
            InitializeComponent();
            RunExampleQueries();

            watcher = new GeoCoordinate(SearchLatitude, SearchLongitude);
            this.map.Center = new GeoCoordinate(SearchLatitude, SearchLongitude);

            Microsoft.Phone.Controls.Maps.Pushpin locationPushpin = new Microsoft.Phone.Controls.Maps.Pushpin();
            locationPushpin.Tag = "locationPushpin";
            locationPushpin.Location = watcher;
            locationPushpin.Content = "Me";

            this.map.Children.Add(locationPushpin);
            this.map.SetView(watcher, 18.0);
        }
    }
    public partial class SuggestionPage
    {
        #region Run the Queries

        string BingMapsKey = "AgF_7d2Nl8hjufE6AU0ot1VhoSQ1vIAoUnEg9pHkpk7WfqjEIcXR2dwNQ7N1DJ4_";

        string DataSourceID = "9ba0e549407b48f6ae6dfda54846711c";

        public double SearchLatitude;

        public double SearchLongitude;

        public string lat;

        public GeoCoordinate watcher;

        public GeoCoordinate watcher2;

        public GeoCoordinate watcher3;

        List<Person> allPeople = null;

        public List<FootSteps.DataModels.Location> answer = new List<FootSteps.DataModels.Location>();

        ApplicationBarIconButton button1 = new ApplicationBarIconButton();
        ApplicationBarIconButton button2 = new ApplicationBarIconButton();
        ApplicationBarIconButton button3 = new ApplicationBarIconButton();
        ApplicationBarIconButton button4 = new ApplicationBarIconButton();

        List<FootSteps.DataModels.Location> d = new List<FootSteps.DataModels.Location>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            groupId = (string)PhoneApplicationService.Current.State["groupId"];
        }


        public void RunExampleQueries()
        {
            // Pushpin locationPushpin = new Pushpin();
            ExampleFindByAreaRadius();
        }



        #endregion


        #region Query By Area
        public void ExampleFindByAreaRadius()
        {


            string dataSourceName = "Footsteps";

            string dataEntityName = "LatLongShops";
            string accessId = DataSourceID;

            string bingMapsKey = BingMapsKey;

            SearchLatitude = 17.431980;
            SearchLongitude = 78.342981;



            double Radius = 11;
            //http://dev.virtualearth.net/REST/v1/Locations/47.64054,-122.12934?includeEntityTypes=countryRegion&o=xml&key=BingMapsKey
            string requestUrl = string.Format("http://spatial.virtualearth.net/REST/v1/data/{0}/{1}/{2}" +
              "?spatialFilter=nearby({3},{4},{5})&key={6}", accessId, dataSourceName,
              dataEntityName, SearchLatitude, SearchLongitude, Radius, bingMapsKey);


            GetXmlResponseAsync(requestUrl);


        }
        #endregion

        #region Helper Methods


        public void GetXmlResponseAsync(string requestUrl)
        {

            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;

            request.BeginGetResponse(new AsyncCallback(FinishWebRequest), request);




        }

        void FinishWebRequest(IAsyncResult result)
        {
            HttpWebResponse response = (HttpWebResponse)(result.AsyncState as HttpWebRequest).EndGetResponse(result);

            /*        XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(response.GetResponseStream());
                    new Program().ProcessEntityElements(xmldoc);*/
            ProcessEntityElements(XDocument.Load(response.GetResponseStream()));
            geolocator_PositionChanged();
        }


        public void ProcessEntityElements(XDocument response)
        {

            IEnumerable<XNode> nodes = response.Root.Nodes();
            List<XElement> entryElements = new List<XElement>();
            foreach (var xnode in nodes)
            {
                if (xnode.NodeType == XmlNodeType.Element)
                {
                    if (((XElement)xnode).Name.LocalName.Equals("entry"))
                        entryElements.Add((XElement)xnode);
                }
            }


            int x = entryElements.Count();
            int counter = 0;

            for (int i = 0; i <= x - 1; i++)
            {
                XElement element = (XElement)entryElements.ElementAt(i);

                XElement contentElement = GetDescendantNode(element, "content");
                //      "content").ElementAt(0);
                XElement propElement = GetDescendantNode(contentElement, "properties");
                //    contentElement.Descendants("m:properties").ElementAt(0);
                XElement nameElement = GetDescendantNode(propElement, "Name");
                //propElement.Descendants("d:Name").ElementAt(0);
                //  answer.Add(nameElement.Value.ToString());


                if (nameElement == null)
                    throw new Exception("Name not found");
                XElement latElement = GetDescendantNode(propElement, "Latitude");
                //propElement.Descendants("d:Latitude").ElementAt(0);
                if (latElement == null)
                    throw new Exception("Latitude not found");
                XElement longElement = GetDescendantNode(propElement, "Longitude");
                //propElement.Descendants("d:Longitude").ElementAt(0);

                if (longElement == null)
                    throw new Exception("Longitude not found");
                string name1 = nameElement.Value;
                double latitude = 0;
                Double.TryParse(latElement.Value, out latitude);
                double longitude = 0;
                Double.TryParse(longElement.Value, out longitude);


                answer.Add(new FootSteps.DataModels.Location { locationId = counter, locationName = name1, locationLat = latitude, locationLong = longitude });
                //Console.WriteLine(latitude.ToString());
                watcher = new GeoCoordinate(latitude, longitude);

                d.Add(new FootSteps.DataModels.Location { locationLat = latitude, locationLong = longitude, locationName = name1 });

                counter++;


            }


        }
        #endregion


        public void geolocator_PositionChanged()
        {
            List<GeoCoordinate> locations = new List<GeoCoordinate>();

            allPeople = (List<Person>)PhoneApplicationService.Current.State["allPeople"];

            Dispatcher.BeginInvoke(() =>
            {
                // watcher2 = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

                if (this.map.Children.Count != 0)
                {
                    var pushpin = map.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && ((Pushpin)p).Tag == "locationPushpin"));

                    if (pushpin != null)
                    {
                        this.map.Children.Remove(pushpin);
                    }
                }


                foreach (Person person in allPeople)
                {

                    if (App.isCoordinatesValid(person.latitude, person.longitude))
                    {
                        watcher3 = new GeoCoordinate(person.latitude, person.longitude);
                        locations.Add(watcher3);
                        Pushpin locationPushpin = new Pushpin();
                        locationPushpin.Tag = "locationPushpin";
                        locationPushpin.Location = watcher3;
                        locationPushpin.Content = person.name;


                        this.map.Children.Add(locationPushpin);
                        this.map.SetView(watcher3, 18.0);
                    }

                }

                this.map.SetView(LocationRect.CreateLocationRect(locations));
                LocationRect a = LocationRect.CreateLocationRect(locations);
                watcher3 = a.Center;
                //SearchLatitude = watcher3.Latitude;
                //SearchLongitude = watcher3.Longitude;
                func1();

            });

            /*Dispatcher.BeginInvoke(() =>
            {

                func1();
            });
           */
        }
       
        public void func1()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;





            // ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            //   button1.IconUri = new Uri("/Images/places.png", UriKind.Relative);
            button1.Text = "Suggestions";
            button1.IconUri = new Uri("/Images/places.png", UriKind.Relative);
            ApplicationBar.Buttons.Add(button1);


            //ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/search.png", UriKind.Relative);
            button2.Text = "Search";
            ApplicationBar.Buttons.Add(button2);

            // ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/location.png", UriKind.Relative);
            button3.Text = "My Location";
            ApplicationBar.Buttons.Add(button3);

            /* ApplicationBarIconButton button4 = new ApplicationBarIconButton();
              button4.Text = "Start";
              ApplicationBar.Buttons.Add(button4);
            */

            button4.IconUri = new Uri("/Images/start.png", UriKind.Relative);
            button4.Text = "Start";
            // ApplicationBar.Buttons.Add(button4);



            button1.Click += new EventHandler(suggestion_clickHandler);
            button2.Click += new EventHandler(search_restaurant_ClickHandler);
            button3.Click += new EventHandler(myLocation_ClickHandler);
            button4.Click += new EventHandler(meet_ClickHandler);


        }
        int count = 0;
        private void suggestion_clickHandler(object sender, EventArgs e)
        {
            //migrate to the Tracking page, that has meet option disabled now


            List<GeoCoordinate> locations = new List<GeoCoordinate>();



            int x = d.Count;


            //LocationRect poi=new LocationRect;
            Dispatcher.BeginInvoke(() =>
            {

                watcher2 = new GeoCoordinate(SearchLatitude, SearchLongitude);

                foreach (FootSteps.DataModels.Location de in d)
                {
                    // if(p)
                    watcher = new GeoCoordinate(de.locationLat, de.locationLong);
                    locations.Add(watcher);

                    Microsoft.Phone.Controls.Maps.Pushpin locationPushpin = new Microsoft.Phone.Controls.Maps.Pushpin();

                    locationPushpin.Tag = "locationPushpin";
                    locationPushpin.Location = watcher;
                    locationPushpin.Content = de.locationName;
                    locationPushpin.Background = new SolidColorBrush(Color.FromArgb(100, 255, 0, 56));

                    this.map.Children.Add(locationPushpin);
                    this.map.SetView(watcher, 18.0);
                    this.map.Center = new GeoCoordinate(de.locationLat, de.locationLong);

                    //  location.Add(new Class2 { latit = de.lati, longit = de.lon });
                    //        locations.Add(new Location(de.lati,de.lon));                   

                }

                //double dbZoom = 1;

                locations.Add(watcher2);

                //    p = p + SearchLatitude;
                //  k = k + SearchLongitude;

                //this.map.Center = new GeoCoordinate(p / 2, k / 2);
                //     var bestview = Microsoft.Maps.MapControl.LocationRect
                //;
                // this.map.setView({bounds:bestview });
                //Microsoft.Maps.MapControl.LocationRect pos = new Microsoft.Maps.MapControl.LocationRect(location);


                this.map.SetView(LocationRect.CreateLocationRect(locations));

                // func1();

            });


            ApplicationBar.IsMenuEnabled = true;

            if (count == 0)
            {
                count = 1;
                foreach (FootSteps.DataModels.Location s in answer)
                {


                    ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
                    menuItem1.Text = s.locationName;
                    ApplicationBar.MenuItems.Add(menuItem1);


                    //  menuItem1.Click+=new EventHandler(menu_ClickHandler(menuItem1,s));

                    menuItem1.Click += (sender1, eventArgs) =>
                    {

                        menu_ClickHandler(menuItem1, s);

                    };

                }

            }

        }

        private void menu_ClickHandler(ApplicationBarMenuItem menuItem1, FootSteps.DataModels.Location s)
        {

            this.map.Center = new GeoCoordinate(s.locationLat, s.locationLong);



            foreach (ApplicationBarMenuItem abmt in ApplicationBar.MenuItems)
            {
                if (abmt.IsEnabled == false)

                    abmt.IsEnabled = true;
            }


            menuItem1.IsEnabled = false;


            if (!menuItem1.IsEnabled)
            {
                if (!ApplicationBar.Buttons.Contains(button4))
                    ApplicationBar.Buttons.Add(button4);

            }

        }


        private void myLocation_ClickHandler(object sender, EventArgs e)
        {

            this.map.Center = new GeoCoordinate(SearchLatitude, SearchLongitude);

        }

        private async void meet_ClickHandler(object sender, EventArgs e)
        {

            await start_method();

        }
        private void search_restaurant_ClickHandler(object sender, EventArgs e)
        {

        }

        public async Task start_method()
        {
            FootSteps.DataModels.Location destinationLocation = new FootSteps.DataModels.Location();
            foreach (ApplicationBarMenuItem abmt in ApplicationBar.MenuItems)
            {
                foreach (FootSteps.DataModels.Location example in answer)
                {
                    if (abmt.IsEnabled == false && abmt.Text == example.locationName)
                    {
                        destinationLocation.locationLat = example.locationLat;
                        destinationLocation.locationLong = example.locationLong;
                        destinationLocation.locationName = example.locationName;
                    }
                }
               
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("destinationLatitude", destinationLocation.locationLat.ToString());
            dict.Add("destinationLongitude", destinationLocation.locationLong.ToString());
            dict.Add("destinationName", destinationLocation.locationName);
            dict.Add("groupId",groupId );
            await App.serviceClient.InvokeApiAsync("updateDestinationForGroup", System.Net.Http.HttpMethod.Get, dict);
            NavigationService.Navigate(new Uri("/MainPages/ConnectPage.xaml", UriKind.Relative));

        }


        public XElement GetDescendantNode(XElement element, string name)
        {
            foreach (var node in element.Nodes())
            {
                if (node.NodeType == XmlNodeType.Element && ((XElement)node).Name.LocalName.Equals(name))
                    return (XElement)node;
            }
            return null;
        }



    }
}