using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.UserData;
using FootSteps.DataModels;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FootSteps.MainPages {
    public partial class ContactsPage : PhoneApplicationPage {
        public ContactsPage() {
            InitializeComponent();
            pageLoad();
        }

       void pageLoad() {
            Contacts cons = new Contacts();

            //Identify the method that runs after the asynchronous search completes.
            cons.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(Contacts_SearchCompleted);

            //Start the asynchronous search.
            cons.SearchAsync(String.Empty, FilterKind.None, "Contacts Test #1");
            
        }


        List<Person> add_contacts = new List<Person>();
        List<Person> invite_contacts = new List<Person>();
       async void Contacts_SearchCompleted(object sender, ContactsSearchEventArgs e) {
            //get devoce contacts
            IEnumerable<Contact> cons;
            cons = e.Results;
            
            //store device cotnacts in a list
            List<Person> contacts_phone = new List<Person>();
            string num = null;

            foreach (Contact item in cons) {
                foreach (ContactPhoneNumber x in item.PhoneNumbers) {
                    num = x.PhoneNumber;
                }
                contacts_phone.Add(new Person { name = item.DisplayName, Id = num, phoneNo = num });
            }

            //for parsing phoneNo in contact_phone
            foreach (var item in contacts_phone) {
                item.phoneNo = Regex.Replace(item.phoneNo,"[^0-9]","");
                item.Id = Regex.Replace(item.Id, "[^0-9]", "");
            }
            //

            //TODO : call the api to determine which of the people contacts_phone are on Footsteps
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("contacts_Person_arr", JsonConvert.SerializeObject(contacts_phone));
            JToken contacts_fs_token = await App.serviceClient.InvokeApiAsync("getContactsMatchingFootsteps", System.Net.Http.HttpMethod.Get, dict);
            List<Person> contacts_fs = JsonConvert.DeserializeObject<List<Person>>(contacts_fs_token.ToString());


            //*************
            //find common contacts which are present in both device contacts and footsteps contacts
           

            string[] dummy = contacts_fs.Select(v => v.phoneNo).ToArray();
            invite_contacts = contacts_phone.Where(o => !dummy.Contains(o.phoneNo)).ToList<Person>();

            string[] dummy1 = contacts_fs.Select(v => v.phoneNo).ToArray();
            add_contacts = contacts_phone.Where(o => dummy1.Contains(o.phoneNo)).ToList<Person>();
            //*************
           

            //adding contacts in contact panel
            //add clolumns to grid
            ContactsPanel.ColumnDefinitions.Add(new ColumnDefinition());
            ContactsPanel.ColumnDefinitions.Add(new ColumnDefinition());
            //add rows to grid
            int i;
            for (i = 0; i < contacts_phone.Count; i++) {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(100);
                ContactsPanel.RowDefinitions.Add(row);
            }


            int cntRows, invite_rows = 0;
            for (cntRows = 0; cntRows < contacts_phone.Count; cntRows++) {

                //create 2 textblocks
                TextBlock tb1 = new TextBlock();
                TextBlock tb2 = new TextBlock();

                //create add buttton
                Button add = new Button();
                add.BorderBrush = null;
                Button invite = new Button();
                invite.BorderBrush = null;

                tb1.FontSize = 28;
                tb1.TextAlignment = TextAlignment.Left;
                tb2.FontSize = 24;
                tb2.TextAlignment = TextAlignment.Left;

                if (cntRows < add_contacts.Count) {
                    tb1.Text = add_contacts[cntRows].name;
                    tb2.Text = add_contacts[cntRows].phoneNo;
                    //add button
                    add.Height = 90;
                    add.Width = 150;
                    add.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/addBefore.png", UriKind.RelativeOrAbsolute)) };
                    add.Name = cntRows.ToString();
                    Grid.SetColumn(add, 1);
                    Grid.SetRow(add, cntRows);
                    if (tb2.Text != App.client.phoneNo)
                        ContactsPanel.Children.Add(add);
                        
                } else {
                    tb1.Text = invite_contacts[invite_rows].name;
                    tb2.Text = invite_contacts[invite_rows].phoneNo;
                    //add.Content = "Invite";
                    invite.Height = 90;
                    invite.Width = 150;
                    invite.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/inviteBefore.png", UriKind.RelativeOrAbsolute)) };
                    invite.Name = "invite" + invite_rows.ToString();
                    invite_rows++;
                    Grid.SetColumn(invite, 1);
                    Grid.SetRow(invite, cntRows);
                    ContactsPanel.Children.Add(invite);
                }

                StackPanel stk = new StackPanel();
                stk.Children.Add(tb1);
                stk.Children.Add(tb2);

                Grid.SetColumn(stk, 0);
                Grid.SetRow(stk, cntRows);
                //Grid.SetColumn(add, 1);
                //Grid.SetRow(add, cntRows);
                ContactsPanel.Children.Add(stk);
                //ContactsPanel.Children.Add(add);

                add.Click += new RoutedEventHandler(this.add_click);
                add.GotFocus += add_GotFocus;
                invite.Click += new RoutedEventHandler(this.invite_click);

            }

        }

       private void invite_click(object sender, RoutedEventArgs e) {
           Button btn = sender as Button;
           btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/inviteAfter.png", UriKind.RelativeOrAbsolute)) };
       }

       void add_GotFocus(object sender, RoutedEventArgs e) {
           Button btn = sender as Button;
           btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/addAfter.png", UriKind.RelativeOrAbsolute)) };
  
       }

        public List<Person> peopleInTheNewGroup = new List<Person>();
        private void add_click(object sender, RoutedEventArgs e) {
            Button btn = sender as Button;
            int rowNo = Int32.Parse(btn.Name);
            peopleInTheNewGroup.Add(add_contacts[rowNo]);

        }

        //APPBAR
        private void clear_tb(object sender, RoutedEventArgs e) {
            groupName_tb.Text = "";
        }

        private async void create_click(object sender, RoutedEventArgs e) {
            //send group name and list of members to server
            FootSteps.DataModels.Group group = new FootSteps.DataModels.Group();
            group.name = groupName_tb.Text;
            Dictionary<string, string> apiParameters = new Dictionary<string, string>();
            if (App.client != null)
            {
                peopleInTheNewGroup.Add(App.client);
                apiParameters.Add("personsListStr", JsonConvert.SerializeObject(peopleInTheNewGroup));
                apiParameters.Add("groupStr", JsonConvert.SerializeObject(group));
                JToken returnedJToken = await App.serviceClient.InvokeApiAsync("createGroup", System.Net.Http.HttpMethod.Get, apiParameters);
                FootSteps.DataModels.Group createdGroup = JsonConvert.DeserializeObject<FootSteps.DataModels.Group>(returnedJToken.ToString());
                NavigationService.Navigate(new Uri("/MainPages/GroupsPage.xaml", UriKind.Relative));
            }
        }
        //

    }
}