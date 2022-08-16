using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Universal_Updater
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    var accentColor = new UISettings().GetColorValue(UIColorType.Accent);
                    statusBar.ForegroundColor = Color.FromArgb(accentColor.A, accentColor.R, accentColor.G, accentColor.B);
                }
            }
            HamburgItems.SelectedIndex = 0;
            MyFrame.Navigate(typeof(Home));
        }

        private async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            //Code to disable the back button
            e.Handled = true;

            //Here you can add your own code and perfrom any task
            if (HomePage.IsSelected)
            {
                try
                {
                    MessageDialog showDialog = new MessageDialog("Are you sure you want exit the app?", "Universal Updater");
                    showDialog.Commands.Add(new UICommand("Yes")
                    {
                        Id = 0
                    });
                    showDialog.Commands.Add(new UICommand("No")
                    {
                        Id = 1
                    });
                    showDialog.DefaultCommandIndex = 0;
                    showDialog.CancelCommandIndex = 1;
                    var result = await showDialog.ShowAsync();
                    if ((int)result.Id == 0)
                    {
                        CoreApplication.Exit();
                    }
                }
                catch (Exception ex) { _ = new MessageDialog(ex.Message).ShowAsync(); }
            }
            else
            {
                HomePage.IsSelected = true;
                MyFrame.Navigate(typeof(Home));
            }
        }

        private void HamburgBtn_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MySplitView.IsPaneOpen = false;

            if (HomePage.IsSelected)
            {
                MyFrame.Navigate(typeof(Home));
            }
            else if (AboutPage.IsSelected)
            {
                MyFrame.Navigate(typeof(About));
            }
            else
            {
                MyFrame.Navigate(typeof(Home));
            }
        }
    }
}
