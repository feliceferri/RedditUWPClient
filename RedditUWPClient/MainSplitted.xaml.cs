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
    
    public sealed partial class MainSplitted : Page
    {

        /*FF: 
            All the code behind refers to code explicity related with this particular view
            especifally the business rules to open and collapse the split panel
            and the Dismiss All button, that has to calculate which are the Entries visible
            so they can be removed.
            Based on that I choose to isolate this code from the modelview, and leave it as part of the the view.
        */


        private enum eDisplayMode
        {
            Portrait,
            Landscape
        }

        private enum eSplittedPanelState
        {
            Open,
            Close
        }

        private eDisplayMode _DisplayMode;
        private GridLength _Original_VisibleCol_WhenPanelIsCollapsed;

        public MainSplitted()
        {
            this.InitializeComponent();

            this.SizeChanged += MainPage_SizeChanged;
            ((ViewModels.VM_MainPage)this.DataContext).EntrySelected += MainSplitted_EntrySelected;

        }

        private void SetPanelState(eSplittedPanelState state)
        {
            if(state == eSplittedPanelState.Close)
            {
                MainSplitView.IsPaneOpen = false;
                _Original_VisibleCol_WhenPanelIsCollapsed = new GridLength(0.2, GridUnitType.Star);  //Wider so user can click it and open the Panel Again
            }
            else
            {
                MainSplitView.IsPaneOpen = true;
                _Original_VisibleCol_WhenPanelIsCollapsed = new GridLength(0, GridUnitType.Star);
            }
        }

        private void MainSplitted_EntrySelected(Models.Child Entry)
        {
            if (_DisplayMode == eDisplayMode.Portrait)
            {
                SetPanelState(eSplittedPanelState.Close);
            }
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight > this.ActualWidth)
            {
                _DisplayMode = eDisplayMode.Portrait;
                MainSplitView.DisplayMode = SplitViewDisplayMode.CompactInline;
                SetPanelState(eSplittedPanelState.Close);

            }
            else
            {
                _DisplayMode = eDisplayMode.Landscape;
                MainSplitView.DisplayMode = SplitViewDisplayMode.Inline;
                SetPanelState(eSplittedPanelState.Open);
                
            }
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

        private void ListView_MainThread_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if(_DisplayMode == eDisplayMode.Portrait)
            {
                SetPanelState(eSplittedPanelState.Open);
            }
        }

        private void GridRightContent_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_DisplayMode == eDisplayMode.Portrait)
            {
                SetPanelState(eSplittedPanelState.Close);
            }
        }

        private void ListView_MainThread_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (_DisplayMode == eDisplayMode.Portrait)
            {
                SetPanelState(eSplittedPanelState.Close);
            }
        }

        private void ListView_MainThread_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {

        }
    }
}
