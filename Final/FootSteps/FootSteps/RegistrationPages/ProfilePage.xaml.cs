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
    public partial class ProfilePage : PhoneApplicationPage {
        public ProfilePage() {
            InitializeComponent();
        }

        string num;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if (PhoneApplicationService.Current.State.ContainsKey("Text"))
                num = (string)PhoneApplicationService.Current.State["Text"];

            PhoneNumber_tb.Text = num;
        }

        private async void Submit_Click(object sender, RoutedEventArgs e) {
            progress_bar.IsIndeterminate = true;
            // TODO : store profile information locally 
             Person currentUser = new Person {
                 Id = num,
                 phoneNo = num,
                 name = Name_tb.Text,
                 isGloballyVisible=true
             };
             LocalPerson p1 = new LocalPerson
             {
                 Id = num,
                 phoneNo = num,
                 name = Name_tb.Text,
                 latitude=0.0,
                 longitude=0.0
             };


             App.client = currentUser;
            Dictionary <string,string> dict = new Dictionary<string,string>();
            dict.Add("personStr",JsonConvert.SerializeObject(currentUser));
            JToken returnVal = await App.serviceClient.InvokeApiAsync("insertIfNotExistsPerson", System.Net.Http.HttpMethod.Get, dict);
            //returnVal= await App.serviceClient.InvokeApiAsync("insertIfNotExistsPerson", System.Net.Http.HttpMethod.Get, dict);
            object retObject = JsonConvert.DeserializeObject<object>(returnVal.ToString());
             if (retObject != null) {
                 LocalDB.insertLocalPerson(p1);
                 NavigationService.Navigate(new Uri("/MainPages/HomePage.xaml", UriKind.Relative));
             } else {
                 // Unlikely to occur
                 MessageBox.Show("Error: Person Already Exists");
             }
        }
    }
}