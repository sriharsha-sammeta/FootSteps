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

namespace Footstepsv3
{
    public partial class MainPage : PhoneApplicationPage
    {



        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            App.client = null;
            App.client=new Person(){
                Id=textBox1.Text,
                phoneNo=textBox1.Text,
                name="dummy name 2",
                latitude=3,
                longitude=3
            };

            await App.serviceClient.InvokeApiAsync<Person,object>("InsertIfNotExistsPerson", App.client);
            NavigationService.Navigate(new Uri("/Passcode.xaml?msg=" + textBox1.Text, UriKind.Relative));
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



    }
}