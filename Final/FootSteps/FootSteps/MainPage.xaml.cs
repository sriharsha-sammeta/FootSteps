using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FootSteps.Resources;
using System.Windows.Media;

namespace FootSteps {
    public partial class MainPage : PhoneApplicationPage {
        // Constructor
        public MainPage() {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void Register_click(object sender, RoutedEventArgs e) {

            LocalDB.createLocalPerson();

            var tp = LocalDB.dbConn.Query<LocalPerson>("select * from localperson").FirstOrDefault();

            if (tp!=null)
            {
                NavigationService.Navigate(new Uri("/MainPages/HomePage.xaml", UriKind.Relative));
            }
            else
            {
                        
                NavigationService.Navigate(new Uri("/RegistrationPages/RegistrationPage.xaml", UriKind.Relative));
            }
        }


    }



}