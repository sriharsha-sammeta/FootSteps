using App2.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace App2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {/*
            Group gp = new Group
            {
                Id = new Guid().ToString(),
                name = "group 1"
            };

            List<Person> ll = new List<Person>();
            ll.Add(new Person
            {
                Id = "123",
                name = "rahul",
                phoneNo = "123",
                isGloballyVisible = true,
                isInMeeting = true
            });
            ll.Add(new Person
            {
                Id = "456",
                name = "rajiv",
                phoneNo = "456",
                isGloballyVisible = true,
                isInMeeting = true
            });
            ll.Add(new Person
            {
                Id = "789",
                name = "ramesh",
                phoneNo = "789",
            });

            Group gp2 = new Group
            {
                Id = new Guid().ToString(),
                name = "group 2"
            };

            List<Person> ll2 = new List<Person>();
            ll2.Add(new Person
            {
                Id = "s123",
                name = "ssrahul",
                phoneNo = "s123"
            });
            ll2.Add(new Person
            {
                Id = "s456",
                name = "ssrajiv",
                phoneNo = "s456"
            });
            ll2.Add(new Person
            {
                Id = "s789",
                name = "ssramesh",
                phoneNo = "s789"
            });



            /*
            foreach (Person tempPerson in ll)
            {
                await App.serviceClient.GetTable<Person>().InsertAsync(tempPerson);
            }
            foreach (Person tempPerson in ll2)
            {
                await App.serviceClient.GetTable<Person>().InsertAsync(tempPerson);
            }
            */
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            /*
            parameters.Add("group", JsonConvert.SerializeObject(gp));
            parameters.Add("persons", JsonConvert.SerializeObject(ll));
            //await App.serviceClient.InvokeApiAsync("createGroup", System.Net.Http.HttpMethod.Post, parameters);
            Group mygroup = await App.serviceClient.InvokeApiAsync<Dictionary<string, string>, Group>("createGroup", parameters);

            parameters.Clear();

            parameters.Add("groupId", mygroup.Id);
            parameters.Add("personId", "123");
            JToken dem = await App.serviceClient.InvokeApiAsync("addConnect", System.Net.Http.HttpMethod.Get, parameters);
            object obj = JsonConvert.DeserializeObject<List<Person>>(dem.ToString());

            */
            Dictionary<string, string> apiParameters = new Dictionary<string, string>();
            apiParameters.Add("personId", "123");
            // SERVICE CALL
            //Person returnedPerson = await App.serviceClient.InvokeApiAsync<Dictionary<string,string>, Person>("getPersonFromPersonId", apiParameters);
            JToken returnedToken = await App.serviceClient.InvokeApiAsync("getPersonFromPersonId", System.Net.Http.HttpMethod.Get, apiParameters);
            Person returnedPerson = JsonConvert.DeserializeObject<Person>(returnedToken.ToString());
            /*
            Dictionary<string,string>dict=new Dictionary<string,string>();
            dict.Add("personId", "123");
            dict.Add("groupId", "2fb2002c-be86-4ef6-bead-d0fc93ee68eb");
            //dict.Add("personId", "123");
           // bool newStatus = true;
            //dict.Add("status", JsonConvert.SerializeObject(newStatus));
            //JToken dem =
           JToken dem =  await App.serviceClient.InvokeApiAsync("getConnectUsingPersonAndGroup", System.Net.Http.HttpMethod.Get, dict);
           Connect lp=JsonConvert.DeserializeObject<Connect>(dem.ToString());
            Connect last = lp;
            
            //dict.Add("group", JsonConvert.SerializeObject(gp));
            //dict.Add("persons", JsonConvert.SerializeObject(ll));
            //await App.serviceClient.InvokeApiAsync<Dictionary<string,string>,Group>("createGroup",dict);
            //dict.Clear();

            //dict.Add("group", JsonConvert.SerializeObject(gp2));
            //dict.Add("persons", JsonConvert.SerializeObject(ll2));
            
            //await App.serviceClient.InvokeApiAsync<Dictionary<string, string>, Group>("createGroup", dict);

            //dict.Add("personId", "s123");
            //dict.Add("groupId", "5f9dd4bf-b045-49b5-9b2e-bb8ca61fcfb5");

            //await App.serviceClient.InvokeApiAsync("addpersontogroup", System.Net.Http.HttpMethod.Get, dict);
            //JToken t1 = temp;
            //dict.Add("toggleValue", "true");
            
            //JToken temp=await App.serviceClient.InvokeApiAsync("getPeopleInGroup", System.Net.Http.HttpMethod.Get, dict);
            //List<Person> pp=JsonConvert.DeserializeObject<List<Person>>(temp.ToString());

            //JToken temp = await App.serviceClient.InvokeApiAsync("groupsFromPersonId", System.Net.Http.HttpMethod.Get, dict);
            //List<Group> pp=JsonConvert.DeserializeObject<List<Group>>(temp.ToString());

            //JToken temp = await App.serviceClient.InvokeApiAsync("togglePersonGroupVisibility", System.Net.Http.HttpMethod.Get, dict);
            //object pp=JsonConvert.DeserializeObject<object>(temp.ToString());
            //object tt = pp;
            
            //List<Person> pp = people;

            /*
            Person p =new Person (){                          
                latitude=100,
                longitude=200,
                phoneNo="1234567890",                
                Id="123456890"
            };

            IMobileServiceTable<Person> tables=App.serviceClient.GetTable<Person>();
            await tables.InsertAsync(p);            
            */

            //List<Person> people= await App.serviceClient.GetTable<Person>().ToListAsync();

        }
    }

    public class Temp
    {
        public Group group { get; set; }
       public List<Person> persons { get; set; }
    }
}
