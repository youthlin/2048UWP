using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace _2048_UWP.Menus
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Setting : Page
    {
        public Setting()
        {
            this.InitializeComponent();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (green.IsChecked == true)
            {
                coloratxt.Text = Colors.Green.R + "," + Colors.Green.G + "," + Colors.Green.B;
            }
            else if (theme.IsChecked == true)
            {
                coloratxt.Text = ((Color)Application.Current.Resources["SystemAccentColor"]).R
                    + "," + ((Color)Application.Current.Resources["SystemAccentColor"]).G
                    + "," + ((Color)Application.Current.Resources["SystemAccentColor"]).B;
            }
            else if (orange.IsChecked == true)
            {
                coloratxt.Text = Colors.Orange.R + "," + Colors.Orange.G + "," + Colors.Orange.B;
            }

            if (dark.IsChecked == true)
            {
                colorbtxt.Text = Colors.Gray.R + "," + Colors.Gray.G + "," + Colors.Gray.B;
            }
            else if (light.IsChecked == true)
            {
                colorbtxt.Text = Colors.White.R + "," + Colors.White.G + "," + Colors.White.B;

            }

            if (white.IsChecked == true)
            {
                colorctxt.Text = "255,255,255";
            }
            else if (black.IsChecked == true)
            {
                colorctxt.Text = "0,0,0";
            }

            string temp = coloratxt.Text;
            string[] rgb = temp.Split(',');
            byte r, g, b;
            try
            {
                r = byte.Parse(rgb[0]);
                g = byte.Parse(rgb[1]);
                b = byte.Parse(rgb[2]);
            }
            catch (Exception) { r = 0; g = 255; b = 0; coloratxt.Text = "0,255,0"; }
            if (r > 255 || r < 0 || g > 255 || g < 0 || b > 255 || b < 0) { r = 0; g = 255; b = 0; coloratxt.Text = "0,255,0"; }
            (Application.Current.Resources["accent"] as SolidColorBrush).Color = Color.FromArgb(255, r, g, b);

            temp = colorbtxt.Text;
            rgb = temp.Split(',');
            try
            {
                r = byte.Parse(rgb[0]);
                g = byte.Parse(rgb[1]);
                b = byte.Parse(rgb[2]);
            }
            catch (Exception) { r = 0; g = 0; b = 0; colorbtxt.Text = "0,0,0"; }
            if (r > 255 || r < 0 || g > 255 || g < 0 || b > 255 || b < 0) { r = 0; g = 0; b = 0; colorbtxt.Text = "0,0,0"; }
            (Application.Current.Resources["bg"] as SolidColorBrush).Color = Color.FromArgb(255, r, g, b);

            temp = colorctxt.Text;
            rgb = temp.Split(',');
            try
            {
                r = byte.Parse(rgb[0]);
                g = byte.Parse(rgb[1]);
                b = byte.Parse(rgb[2]);
            }
            catch (Exception) { r = 0; g = 0; b = 0; colorctxt.Text = "0,0,0"; }
            if (r > 255 || r < 0 || g > 255 || g < 0 || b > 255 || b < 0) { r = 0; g = 0; b = 0; colorctxt.Text = "0,0,0"; }
            (Application.Current.Resources["txt"] as SolidColorBrush).Color = Color.FromArgb(255, r, g, b);

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                coloratxt.Text = (Application.Current.Resources["accent"] as SolidColorBrush).Color.R
                    + "," + (Application.Current.Resources["accent"] as SolidColorBrush).Color.G
                    + "," + (Application.Current.Resources["accent"] as SolidColorBrush).Color.B;
                colorbtxt.Text = (Application.Current.Resources["bg"] as SolidColorBrush).Color.R
                    + "," + (Application.Current.Resources["bg"] as SolidColorBrush).Color.G
                    + "," + (Application.Current.Resources["bg"] as SolidColorBrush).Color.B;
                colorctxt.Text = (Application.Current.Resources["txt"] as SolidColorBrush).Color.R
                    + "," + (Application.Current.Resources["txt"] as SolidColorBrush).Color.G
                    + "," + (Application.Current.Resources["txt"] as SolidColorBrush).Color.B;
            }
            base.OnNavigatedTo(e);
        }

        private void txt_LostFocus(object sender, RoutedEventArgs e) { RadioButton_Click(sender, e); }
    }
}
