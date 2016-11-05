using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp6.Resources;
//using System;
//using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace PhoneApp6
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        string BingMapsKey = "Atl-6KmWz6HWRHJR5zPh_R8n82d1G0fMiPGvujKPYRY5ne3kP3jPddbfwn_EVcVZ";

        string DataSourceID = "20181f26d9e94c81acdf9496133d4f23";
        public MainPage()
        {
            InitializeComponent();
           
            RunExampleQueries();

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
        
    

        
        

        #region Run the Queries

        
        public void RunExampleQueries()
        {
            ExampleFindByAreaRadius();
        }
        #endregion

        #region Query By Area
        public void ExampleFindByAreaRadius()
        {
           

            string dataSourceName = "FourthCoffeeSample";
            
            string dataEntityName = "FourthCoffeeShops";
            string accessId = DataSourceID;
            
            string bingMapsKey = BingMapsKey;
            
            double SearchLatitude = 47.63674;
            double SearchLongitude = -122.30413;
            
            double Radius = 3; 
            
            string requestUrl = string.Format("http://spatial.virtualearth.net/REST/v1/data/{0}/{1}/{2}" +
              "?spatialFilter=nearby({3},{4},{5})&key={6}", accessId, dataSourceName,
              dataEntityName, SearchLatitude, SearchLongitude, Radius, bingMapsKey);
            
            XDocument response = GetXmlResponseAsync(requestUrl);
            
            ProcessEntityElements(response);
        }
        #endregion

        #region Helper Methods


        public XDocument GetXmlResponseAsync(string requestUrl)
        {
            
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.BeginGetResponse(new AsyncCallback(FinishWebRequest), request) as HttpWebResponse;

                //      XDocument xmlDoc = new XDocument();
                return (XDocument.Load(response.GetResponseStream()));
                //   return (xmlDoc);

            
        }

        static void FinishWebRequest(IAsyncResult result)
        {
            HttpWebResponse req = (HttpWebResponse)(result.AsyncState as HttpWebRequest).EndGetResponse(result);
           
        }

        
        private void ProcessEntityElements(XDocument response)
        {
            IEnumerable<XElement> entryElements = response.Descendants("entry");
            int x = entryElements.Count();

            for (int i = 0; i <= x - 1; i++)
            {
                XElement element = (XElement)entryElements.ElementAt(i);
                XElement contentElement = (XElement)element.Descendants(
                  "content").ElementAt(0);
                XElement propElement = (XElement)
                  contentElement.Descendants("m:properties").ElementAt(0);
                XNode nameElement = propElement.Descendants("d:Name").ElementAt(0);
                if (nameElement == null)
                    throw new Exception("Name not found");
                XNode latElement = propElement.Descendants("d:Latitude").ElementAt(0);
                if (latElement == null)
                    throw new Exception("Latitude not found");
                XNode longElement = propElement.Descendants("d:Longitude").ElementAt(0);

                if (longElement == null)
                    throw new Exception("Longitude not found");
                string name = nameElement.ToString();
                double latitude = 0;
                Double.TryParse(latElement.ToString(), out latitude);
                double longitude = 0;
                Double.TryParse(longElement.ToString(), out longitude);
                textBox1.Text = latitude.ToString();
               
                
            
            }
            
        }
        #endregion
    }

}