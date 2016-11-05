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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FootSteps.MainPages
{
    public partial class GroupsPage : PhoneApplicationPage
    {
        List<TextBlock> tbs = new List<TextBlock>();
        List<Button> btns = new List<Button>();
        List<Group> groups = new List<Group>();
        public GroupsPage()
        {
            InitializeComponent();            
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            refresh();
            //refresh_click(null, null);
        }

        private async void refresh()
        {
            Dictionary<string, string> apiParameters = new Dictionary<string, string>();
            apiParameters.Add("personId", App.client.Id);
            JToken returnedJToken = await App.serviceClient.InvokeApiAsync("groupsFromPersonId", System.Net.Http.HttpMethod.Get, apiParameters);
            groups = JsonConvert.DeserializeObject<List<Group>>(returnedJToken.ToString());

            apiParameters.Clear();
            apiParameters.Add("personId",App.client.Id);
            apiParameters.Add("groupsListStr",JsonConvert.SerializeObject(groups));

            JToken toggleValueDictToken = await App.serviceClient.InvokeApiAsync("getToggleValuesForGroupList", System.Net.Http.HttpMethod.Get, apiParameters);
            Dictionary<string, string> groupToToggleDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(toggleValueDictToken.ToString());


            GroupsPanel.ColumnDefinitions.Clear();
            GroupsPanel.RowDefinitions.Clear();
            GroupsPanel.Children.Clear();
            //add clolumns to grid
            GroupsPanel.ColumnDefinitions.Add(new ColumnDefinition());
            GroupsPanel.ColumnDefinitions.Add(new ColumnDefinition());
            GroupsPanel.ColumnDefinitions.Add(new ColumnDefinition());

            //add rows to grid
            
            for (int i = 0; i < groups.Count; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(100);
                GroupsPanel.RowDefinitions.Add(row);
            }

            
            int cntRows = 0;
            foreach (Group item in groups)
            {
                TextBlock tb1 = new TextBlock();
                TextBlock tb2 = new TextBlock();

                Button toggle = new Button();
                toggle.Name = item.Id+"_Button";
                toggle.Height = 85;
                toggle.Width = 130;
                btns.Add(toggle);       

                tb1.FontSize = 28;
                tb1.Name = item.Id+"_textBlock";
                tb1.TextAlignment = TextAlignment.Left;
                tb1.Text = item.name;
                tb1.Tap += tb1_Tap;
                tbs.Add(tb1);
                tb2.FontSize = 20;
                //tb1.TextAlignment = TextAlignment.Left;
                //tb2.TextAlignment = TextAlignment.Center;


                if (groupToToggleDict[item.Id] == "True")
                {
                    toggle.Content = "ON";
                    toggle.BorderBrush = null;
                    toggle.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/on.png", UriKind.RelativeOrAbsolute)) };
                    tbs[btns.FindIndex(x => x == (toggle))].Tap += tb1_Tap;
                }
                else
                {
                    toggle.Content = "OFF";
                    toggle.BorderBrush = null;
                    toggle.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/off.png", UriKind.RelativeOrAbsolute)) };
                    tbs[btns.FindIndex(x => x == (toggle))].Tap -= tb1_Tap;
                }
                
                // TODO : Set the label according to the state of the group i.e. isGroupInMeeting or not
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("groupId", item.Id);
                JToken retToken = await App.serviceClient.InvokeApiAsync("isGroupInMeeting", System.Net.Http.HttpMethod.Get, dict);
                object retObject = JsonConvert.DeserializeObject<object>(retToken.ToString());
                if (retObject != null)
                {
                    tb2.Text = "In Meeting";
                }
                else
                {
                    tb2.Text = "Start A Connect";
                }

                Grid.SetColumn(tb1, 0);
                Grid.SetRow(tb1, cntRows);
                Grid.SetColumn(tb2, 1);
                Grid.SetRow(tb2, cntRows);
                Grid.SetColumn(toggle, 2);
                Grid.SetRow(toggle, cntRows);
                GroupsPanel.Children.Add(tb1);
                GroupsPanel.Children.Add(tb2);
                GroupsPanel.Children.Add(toggle);
                

                cntRows++;
                 
                toggle.Click += new RoutedEventHandler(this.toggle_click);
            }

        }

        //group name click handler
        TextBlock selectedGroup;
        void tb1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedGroup = tbs.FirstOrDefault(x=>x.Name==(sender as TextBlock).Name);                            
            
            NavigationService.Navigate(new Uri("/MainPages/ConnectPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Called when a page becomes the active page in a frame
            base.OnNavigatedFrom(e);
            // Text is param, you can define anything instead of Text 
            // but remember you need to further use same param.
            if (selectedGroup != null)
            {

                int index= tbs.FindIndex(x => x.Name == selectedGroup.Name);
                if (index >= 0)
                {
                    PhoneApplicationService.Current.State["groupId"] = groups[index].Id;
                }                
            }
            
        }
        //******

        private void toggle_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            bool toggleValue=false;
            
            if (btn.Content.ToString() == "OFF")
            {
                btn.Content = "ON";
                btn.BorderBrush = null;
                btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/on.png", UriKind.RelativeOrAbsolute)) };
                toggleValue=true;
                tbs[btns.FindIndex(x => x == (sender as Button))].Tap += tb1_Tap;
            }
            else
            {
                btn.Content = "OFF";
                btn.BorderBrush = null;
                btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/off.png", UriKind.RelativeOrAbsolute)) };
                toggleValue=false;
                tbs[btns.FindIndex(x => x == (sender as Button))].Tap -= tb1_Tap;
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("personId",App.client.Id);
            int index = btns.FindIndex(x => x == (sender as Button));
            dict.Add("groupId", groups[btns.FindIndex(x=> x==(sender as Button))].Id);
            dict.Add("toggleValue", JsonConvert.SerializeObject(toggleValue));
            App.serviceClient.InvokeApiAsync("togglePersonGroupVisibility", System.Net.Http.HttpMethod.Get, dict);
        }

        //*****APPBAR****
        private void create_click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPages/ContactsPage.xaml", UriKind.Relative));
        }

        private void refresh_click(object sender, EventArgs e)
        {
            refresh();
        }
        //*****


    }
}