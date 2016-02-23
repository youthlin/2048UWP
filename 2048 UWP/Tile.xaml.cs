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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace _2048_UWP
{
    public sealed partial class Tile : UserControl
    {
        public Tile()
        {
            this.InitializeComponent();
        }

        private int number;
        public int Number
        {
            get { return number; }
            set
            {
                number = value;
                SetBackground();
                SetText();
            }
        }
        public void SetBackground()
        {
            switch (number)
            {
                case 0: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 205, 190, 180)); break;
                case 2: TileBorder.Background = new SolidColorBrush(Color.FromArgb(240, 240, 230, 220)); break;
                case 4: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 240, 224, 200)); break;
                case 8: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 240, 175, 120)); break;
                case 16: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 245, 150, 100)); break;
                case 32: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 250, 125, 90)); break;
                case 64: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 245, 95, 60)); break;
                case 128: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 235, 210, 115)); break;
                case 256: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 235, 205, 100)); break;
                case 512: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 235, 200, 80)); break;
                case 1024: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 235, 200, 60)); break;
                case 2048: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 235, 195, 50)); break;
                case 4096: TileBorder.Background = new SolidColorBrush(Color.FromArgb(255, 240, 195, 30)); break;
                default: TileBorder.Background = new SolidColorBrush(Colors.DarkGray); break;

            }
        }

        public void SetText()
        {
            switch (number)
            {
                case 0: TileText.Text = ""; break;
                case 2: TileText.Text = "2"; TileText.Foreground = new SolidColorBrush(Colors.Black); break;
                case 4: TileText.Text = "4"; TileText.Foreground = new SolidColorBrush(Colors.Black); break;
                default: TileText.Text = number.ToString(); TileText.Foreground = new SolidColorBrush(Colors.White); break;
            }
        }
        public void SetCenterXY(double x, double y) { scl1.CenterX = x; scl1.CenterY = y; }
        public void Appera() { Debug.WriteLine("Appear;"); appera.Begin(); }
        public void Zoom() { zoom.Begin(); }
    }
}
