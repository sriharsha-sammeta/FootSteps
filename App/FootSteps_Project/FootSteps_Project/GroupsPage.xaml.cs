using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FootSteps_Project.Data;

namespace FootSteps_Project {
    public partial class GroupsPage : PhoneApplicationPage {
        public GroupsPage() {
            InitializeComponent();
            pageLoad();
        }
 
        void pageLoad() {
            List<GroupNames> groups_details = new List<GroupNames>();
            GroupDetails g = new GroupDetails();
            groups_details = g.groups_details;
            

            //adding contacts in contact panel
            //add clolumns to grid
            GroupsPanel.ColumnDefinitions.Add(new ColumnDefinition());
            GroupsPanel.ColumnDefinitions.Add(new ColumnDefinition());
            //add rows to grid
            int i;
            for (i = 0; i < groups_details.Count; i++) {
                GroupsPanel.RowDefinitions.Add(new RowDefinition());
            }


            int cntRows = 0;
            foreach (GroupNames item in groups_details) {
                TextBlock tb1 = new TextBlock();

                Button toggle = new Button();
                toggle.Content = "OFF";

                tb1.FontSize = 28;
                tb1.Name = item.id.ToString();
                tb1.TextAlignment = TextAlignment.Left;
                tb1.Text = item.groupName;
                tb1.Tap += tb_tap;


                Grid.SetColumn(tb1, 0);
                Grid.SetRow(tb1, cntRows);
                Grid.SetColumn(toggle, 1);
                Grid.SetRow(toggle, cntRows);
                GroupsPanel.Children.Add(tb1);
                GroupsPanel.Children.Add(toggle);

                cntRows++;
                toggle.Click += new RoutedEventHandler(this.toggle_click);
            }
        }

        TextBlock clicked_tb; 
        private void tb_tap(object sender, System.Windows.Input.GestureEventArgs e) {
            clicked_tb = sender as TextBlock;
            NavigationService.Navigate(new Uri("/GroupDetailsPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e) {
            // Called when a page becomes the active page in a frame
            base.OnNavigatedFrom(e);
            // Text is param, you can define anything instead of Text 
            // but remember you need to further use same param.
            PhoneApplicationService.Current.State["Name"] = clicked_tb.Name;
        }


        private void toggle_click(object sender, RoutedEventArgs e) {
            Button btn = sender as Button;
            if (btn.Content.ToString() == "OFF") {
                btn.Content = "ON";
            } else {
                btn.Content = "OFF";
            }
        }
    }
}