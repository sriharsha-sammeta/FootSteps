using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using FootSteps.DataModels;

namespace FootSteps.RegistrationPages {
    public partial class PasscodePage : PhoneApplicationPage {
        public PasscodePage() {
            InitializeComponent();
        }

        string num;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if (PhoneApplicationService.Current.State.ContainsKey("Text"))
                 num= (string)PhoneApplicationService.Current.State["Text"];
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e) {
            // Called when a page becomes the active page in a frame
            base.OnNavigatedFrom(e);
            // Text is param, you can define anything instead of Text 
            // but remember you need to further use same param.
            PhoneApplicationService.Current.State["Text"] = num;
        }

        private async void Submit_Click(object sender, RoutedEventArgs e) {
            progress_bar.IsIndeterminate = true;
            //TODO : Verify the passcode provided by the user with the third-party service.
            // Hard coded passcode for now is - 1234
            if (passcode.Text == "1234") {
                Dictionary<string,string> apiParameters = new Dictionary<string,string>();
                apiParameters.Add("personId", num);
                // SERVICE CALL ... Check if the person exists
                //Person returnedPerson = await App.serviceClient.InvokeApiAsync<Dictionary<string,string>, Person>("getPersonFromPersonId", apiParameters);
                JToken returnedToken = await App.serviceClient.InvokeApiAsync("getPersonFromPersonId", System.Net.Http.HttpMethod.Get, apiParameters);
                Person returnedPerson = JsonConvert.DeserializeObject<Person>(returnedToken.ToString());
                if (returnedPerson == null) {
                    // The person does not exist. Make a service call to insert the person
                    NavigationService.Navigate(new Uri("/RegistrationPages/ProfilePage.xaml", UriKind.Relative));
                } else {
                    App.client.phoneNo = num;
                    App.client.Id = num;
                    App.client.isGloballyVisible = true;                    
                    // Navigate to profile page
                    LocalPerson p1 = new LocalPerson
                    {
                        Id = num,
                        phoneNo = num,
                        name ="aaa",
                        latitude = 0.0,
                        longitude = 0.0
                    };
                    LocalDB.insertLocalPerson(p1);
                    NavigationService.Navigate(new Uri("/MainPages/HomePage.xaml", UriKind.Relative));
                }
            } else {
                MessageBox.Show("Wrong PassCode !");
            }
        }
    }
}