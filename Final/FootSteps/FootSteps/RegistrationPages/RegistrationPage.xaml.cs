using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FootSteps.DataModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FootSteps.RegistrationPages {
    public partial class RegistrationPage : PhoneApplicationPage {
        public RegistrationPage() {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e) {
            // Called when a page becomes the active page in a frame
            base.OnNavigatedFrom(e);
            // Text is param, you can define anything instead of Text 
            // but remember you need to further use same param.
            PhoneApplicationService.Current.State["Text"] = PhoneNumber.Text;
        }
        private void Next_Click(object sender, RoutedEventArgs e) {
            //validate phone number
            if (PhoneNumber.Text.Length == 10) {
                //Take him to the PASSCODE PAGE
                NavigationService.Navigate(new Uri("/RegistrationPages/PasscodePage.xaml", UriKind.Relative));
            } else {
                MessageBox.Show("Invalid Number...!!!");
                PhoneNumber.Text="";
            }
        }
    }
}