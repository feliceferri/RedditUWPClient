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

            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Reddit reddit = new Reddit();
            var res = await reddit.GetEntriesAsync();

            if (res.Success == true)
            {
                this.ListView_MainThread.ItemsSource = res.value.data.children;

                //foreach (var item in res.value.data.children)
                //{
                //    Debug.WriteLine("title: " + item.data.title);
                //    Debug.WriteLine("Author: " + item.data.author); //subreddit vs subreddit_name_prefixed
                //    DateTime dt = new DateTime().UnixUTCTimeToLocalDateTime(item.data.created_utc);
                //    Debug.WriteLine("Entry date:" + dt.ToString() + " Hours ago: " + Math.Floor((DateTime.Now - dt).TotalHours));
                //    Debug.WriteLine("Comments: " + item.data.num_comments);
                //    Debug.WriteLine("");
                //}
            }
            else
            {
                throw res.Error;
            }


            //var res = SamplingData.RedditEntries;
            //foreach (var item in res)
            //{
            //    Debug.WriteLine("title: " + item.data.title);
            //    Debug.WriteLine("Author: " + item.data.author); //subreddit vs subreddit_name_prefixed
            //    DateTime dt = new DateTime().UnixUTCTimeToLocalDateTime(item.data.created_utc);
            //    Debug.WriteLine("Entry date:" + dt.ToString() + " Hours ago: " + Math.Floor((DateTime.Now - dt).TotalHours));
            //    Debug.WriteLine("Comments: " + item.data.num_comments);
            //    Debug.WriteLine("");
            //}

            //this.ListView_MainThread.ItemsSource = res;
        }
    }
}
