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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RedditUWPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainSplitted : Page
    {
        public MainSplitted()
        {
            this.InitializeComponent();
        }


        private void btnDismissVisibles_Click(object sender, RoutedEventArgs e)
        {
            ///FF: This story is exclusively related to the UI, thats why is in the View side

            List<Models.Child> ListToDismiss = new List<Models.Child>();

            foreach (Models.Child item in this.ListView_MainThread.Items)
            {

                var con = (UIElement)this.ListView_MainThread.ContainerFromItem(item);
                if (con != null)
                {
                    if (con.ActualOffset.Y + con.ActualSize.Y < this.ListView_MainThread.ActualHeight)
                    {
                        ListToDismiss.Add(item);
                    }
                    else
                    {
                        break; //It already passed it
                    }
                }

            }

                   ((ViewModels.VM_MainPage)this.DataContext).DismissEntries(ListToDismiss);

        }
        
    }
}
