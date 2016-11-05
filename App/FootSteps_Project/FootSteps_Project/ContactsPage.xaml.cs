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
using FootSteps_Project.Data;

namespace FootSteps_Project {
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

        void Contacts_SearchCompleted(object sender, ContactsSearchEventArgs e) {
            //get devoce contacts
            IEnumerable<Contact> cons;
            cons = e.Results;

            //store device cotnacts in a list
            List<Phone_contacts> contacts_phone = new List<Phone_contacts>();
            string num = null;

            foreach (Contact item in cons) {
                foreach (ContactPhoneNumber x in item.PhoneNumbers) {
                    num = x.PhoneNumber;
                }
                contacts_phone.Add(new Phone_contacts { contactName = item.DisplayName, contactNumber = num });
            }


            //enter footsteps data in a list
            List<Footsteps_contacts> contacts_fs = new List<Footsteps_contacts>();
            contacts_fs.Add(new Footsteps_contacts { contactName = "Name2", contactNumber = "(222) 222-2222" });
            contacts_fs.Add(new Footsteps_contacts { contactName = "Name3", contactNumber = "(333) 333-3333" });
            contacts_fs.Add(new Footsteps_contacts { contactName = "Name5", contactNumber = "(555) 555-5555" });
            
            //*************
            //find common contacts which are present in both device contacts and footsteps contacts
            List<Phone_contacts> add_contacts = new List<Phone_contacts>();
            List<Phone_contacts> invite_contacts = new List<Phone_contacts>();

            string[] dummy = contacts_fs.Select(v => v.contactNumber).ToArray();
            invite_contacts = contacts_phone.Where(o => !dummy.Contains(o.contactNumber)).ToList<Phone_contacts>();

            string[] dummy1 = contacts_fs.Select(v => v.contactNumber).ToArray();
            add_contacts = contacts_phone.Where(o => dummy1.Contains(o.contactNumber)).ToList<Phone_contacts>();
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

           
            int cntRows,invite_rows=0;
            for (cntRows = 0; cntRows < contacts_phone.Count; cntRows++ ) {

                //create 2 textblocks
                TextBlock tb1 = new TextBlock();
                TextBlock tb2 = new TextBlock();

                Button add = new Button();

                tb1.FontSize = 28;
                tb1.TextAlignment = TextAlignment.Left;
                tb2.FontSize = 24;
                tb2.TextAlignment = TextAlignment.Left;

                if (cntRows<add_contacts.Count) {
                    tb1.Text = add_contacts[cntRows].contactName;
                    tb2.Text = add_contacts[cntRows].contactNumber;
                    add.Content = "Add";
                    add.Name = cntRows.ToString();
                } else {
                    tb1.Text = invite_contacts[invite_rows].contactName;
                    tb2.Text = invite_contacts[invite_rows].contactNumber;
                    add.Content = "Invite";
                    add.Name = "invite"+invite_rows.ToString();
                    invite_rows++;
                }
                
                StackPanel stk = new StackPanel();
                stk.Children.Add(tb1);
                stk.Children.Add(tb2);

                Grid.SetColumn(stk, 0);
                Grid.SetRow(stk, cntRows);
                Grid.SetColumn(add, 1);
                Grid.SetRow(add, cntRows);
                ContactsPanel.Children.Add(stk);
                ContactsPanel.Children.Add(add);

                add.Click += new RoutedEventHandler(this.add_click);

            }

        }

        private void add_click(object sender, RoutedEventArgs e) {
            Button btn = sender as Button;
            
        }


    }
}