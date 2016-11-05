using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using test.DataModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //testViewModel tvm = new testViewModel(App.MobileService);
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

            //validate mobile number
            if (MobileNumber.Text.Length == 10) {
                try
                {
                    string postUrl = "http://localhost:46816/tables/Contacts";
                    StringContent postContent = new StringContent("TestString");
                    IMobileServiceTable<Person> allPeople = App.MobileService.GetTable<Person>();
                    allPeople.InsertAsync(new Person()
                    {
                        latitude=100,
                        longitude=200,
                        phoneNo="1234567890"                        
                    });
                }
                catch (Exception ex2)
                {
                    ErrorMessage = ex2.Message;
                }

                Frame.Navigate(typeof(page1), MobileNumber.Text);
            } else {
                InvalidNo.Text = "*Invalid Number";
                MobileNumber.Text = String.Empty;
                MobileNumber.PlaceholderText = "Enter Mobile Number";
            }
        }

        public string ErrorMessage { get; set; }
    }
}
