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
    public partial class GroupDetailsPage : PhoneApplicationPage {
        public GroupDetailsPage() {
            InitializeComponent();
        }

        string group_id;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if (PhoneApplicationService.Current.State.ContainsKey("Name"))
                group_id = (string)PhoneApplicationService.Current.State["Name"];

            List<GroupNames> groups_details = new List<GroupNames>();
            GroupDetails g = new GroupDetails();
            groups_details = g.groups_details;
            Group_Name.Text = groups_details[Int32.Parse(group_id)].groupName;
            Group_Purpose.Text = groups_details[Int32.Parse(group_id)].purpose;

            int i;
            for (i = 0; i < groups_details[Int32.Parse(group_id)].members.Count; i++) {
                //add new row for each member
                Members.RowDefinitions.Add(new RowDefinition());
            }

            int cntRows = 0;
            foreach (var item in groups_details[Int32.Parse(group_id)].members) {
                //create 2 textblocks
                TextBlock tb1 = new TextBlock();
                TextBlock tb2 = new TextBlock();

                tb1.FontSize = 28;
                tb1.TextAlignment = TextAlignment.Left;
                tb2.FontSize = 24;
                tb2.TextAlignment = TextAlignment.Left;

                tb1.Text = groups_details[Int32.Parse(group_id)].members[cntRows].contactName;
                tb2.Text = groups_details[Int32.Parse(group_id)].members[cntRows].contactNumber;

                StackPanel stk = new StackPanel();
                stk.Children.Add(tb1);
                stk.Children.Add(tb2);

                Grid.SetRow(stk, cntRows);

                Members.Children.Add(stk);
            }
        }

    }
}