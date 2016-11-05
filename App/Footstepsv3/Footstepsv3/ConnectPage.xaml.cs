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
using System.Threading.Timer;

using System.Threading.Tasks;
using Windows.Devices.Geolocation;

using System.IO.IsolatedStorage;
using Footstepsv3.dataModels;
using Microsoft.WindowsAzure.MobileServices;


namespace Footstepsv3
{

    public partial class ConnectPage : PhoneApplicationPage
    {

        GeoCoordinate watcher2;
        List<Person> allPeople = null;


      
      
        async public void connect_PositionChanged()
        {
            allPeople= await App.serviceClient.GetTable<Person>().ToListAsync();


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

                    watcher2 = new GeoCoordinate(person.latitude, person.longitude);
                    Pushpin locationPushpin = new Pushpin();
                    locationPushpin.Tag = "locationPushpin";
                    locationPushpin.Location = watcher2;
                    locationPushpin.Content = person.name;

                    this.map.Children.Add(locationPushpin);
                    this.map.SetView(watcher2, 18.0);

                }

            });

        }


        public ConnectPage()
        {
            InitializeComponent();
            
        }
    }


    public class CounterTester
    {
        public Timer countDownTimer;
        ConnectPage cp = new ConnectPage();
        public CounterTester(int interval)
        {
            countDownTimer = new Timer(interval);
            countDownTimer.Enabled = true;
            countDownTimer.Elapsed += new ElapsedEventHandler(cp.connect_PositionChanged());
            //countDownTimer.Start();
        }

        
    }

}