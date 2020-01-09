using RedditUWPClient.Helpers;
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
using RedditUWPClient.ExtensionsMethods;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RedditUWPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

       
        public MainPage()
        {
            this.InitializeComponent();

            this.SizeChanged += MainPage_SizeChanged;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

           
            double borderStars = 0.4; //default

            if(this.ActualHeight> this.ActualWidth)
            {
                borderStars = 0.02;
            }
            else
            {
                double ratio = this.ActualWidth / this.ActualHeight;
                
                if(ratio < 2)
                {
                    borderStars = ratio % 1;  //Remainder of the division, came to this after having a large switch case with different ratios
                }
                else
                {
                    borderStars = 1;
                }
                
            }

            this.MaingGrid_LeftColumnDefinition.Width = new GridLength(borderStars, GridUnitType.Star);
            this.MaingGrid_RightColumnDefinition.Width = new GridLength(borderStars, GridUnitType.Star);
        }
    }
}
