using _2048_UWP.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace _2048_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Menu : Page
    {
        public Menu()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            split.IsPaneOpen = !split.IsPaneOpen;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            if (e.NavigationMode == NavigationMode.New)
            {
                frame.Navigate(typeof(Setting));
                List<MenuItem> mylist1 = new List<MenuItem>();
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                //str = loader.GetString("back");
                //mylist1.Add(new MenuItem() { Icon = "Back", Title = str });
                var str = loader.GetString("setting");
                mylist1.Add(new MenuItem() { Icon = "Setting", Title = str });
                str = loader.GetString("histroy");
                mylist1.Add(new MenuItem() { Icon = "Favorite", Title = str });
                str = loader.GetString("help");
                mylist1.Add(new MenuItem() { Icon = "Help", Title = str });
                list1.ItemsSource = mylist1;

                mylist1 = new List<MenuItem>();
                str = loader.GetString("about");
                mylist1.Add(new MenuItem() { Icon = "Contact", Title = str });
                str = loader.GetString("like");
                mylist1.Add(new MenuItem() { Icon = "Like", Title = str });
                //str = loader.GetString("back");
                //mylist1.Add(new MenuItem() { Icon = "Back", Title = str });
                list2.ItemsSource = mylist1;
            }
            base.OnNavigatedTo(e);
            CoreWindow.GetForCurrentThread().PointerReleased += (sender, args) =>
            {
                args.Handled = true;
                if (args.CurrentPoint.Properties.PointerUpdateKind == Windows.UI.Input.PointerUpdateKind.XButton1Released) if (Frame.CanGoBack) Frame.GoBack();
            };
        }



        private async void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MenuItem item = null;
            if (sender.Equals(list1))
            {
                Debug.WriteLine("1:" + list1.SelectedIndex);
                if (list1.SelectedIndex == -1) return;
                item = list1.SelectedItem as MenuItem;
                list2.SelectedIndex = -1;
            }
            else if (sender.Equals(list2))
            {
                Debug.WriteLine("2:" + list2.SelectedIndex);
                if (list2.SelectedIndex == -1) return;
                item = list2.SelectedItem as MenuItem;
                list1.SelectedIndex = -1;
            }
            else return;

            if (item.Icon == "Back")
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame == null) return;
                if (rootFrame.CanGoBack) rootFrame.GoBack();
            }
            if (item.Icon == "Setting") frame.Navigate(typeof(Setting));
            if (item.Icon == "Favorite") frame.Navigate(typeof(Histroy));
            if (item.Icon == "Help") frame.Navigate(typeof(Help));
            if (item.Icon == "Contact") frame.Navigate(typeof(About));
            if (item.Icon == "Like")
            {
                try { await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NBLGGH68BB0")); }
                catch (Exception ex) { Debug.WriteLine(ex); }
            }

        }
    }
    class MenuItem
    {
        public string Icon { get; set; }
        public string Title { get; set; }
    }
}
