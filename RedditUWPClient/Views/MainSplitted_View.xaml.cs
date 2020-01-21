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

        bool Flag_Skip_LeftFrameWhenPanelIsCollapsed_PointerEntered_Until_PointerIsOutOfLeftNavBar = false;

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
   
        public MainSplitted()
        {
            this.InitializeComponent();

            this.SizeChanged += MainPage_SizeChanged;
            ViewModels.MainSplitted_ViewModel VM = ((ViewModels.MainSplitted_ViewModel)this.DataContext);

            //*******************************************************
            //"Hide Show Left Nav Bar depending on Mouse Moves / Tapping etc"
            //*******************************************************
            VM.EntrySelected += delegate
            {
                if (_DisplayMode == eDisplayMode.Portrait)
                {
                    Flag_Skip_LeftFrameWhenPanelIsCollapsed_PointerEntered_Until_PointerIsOutOfLeftNavBar = true;
                    SetPanelState(eSplittedPanelState.Close);
                }
            };

            this.ListView_MainThread.PointerEntered += delegate{if (_DisplayMode == eDisplayMode.Portrait) SetPanelState(eSplittedPanelState.Open);};
            this.ListView_MainThread.PointerExited += delegate { if (_DisplayMode == eDisplayMode.Portrait) SetPanelState(eSplittedPanelState.Close); };

            ////GridRightContent is the Right Side of the SplitView where the Entry Detail is being rendered
            this.GridRightContent.Tapped += delegate {
                if (_DisplayMode == eDisplayMode.Portrait)
                {
                    Flag_Skip_LeftFrameWhenPanelIsCollapsed_PointerEntered_Until_PointerIsOutOfLeftNavBar = false;
                    SetPanelState(eSplittedPanelState.Close);
                }
            };

            this.GridRightContent.PointerEntered += delegate {
                if (_DisplayMode == eDisplayMode.Portrait)
                {
                    Flag_Skip_LeftFrameWhenPanelIsCollapsed_PointerEntered_Until_PointerIsOutOfLeftNavBar = false;
                    SetPanelState(eSplittedPanelState.Close);
                }
            };
            ///////////////////////////////////////////////////////////////////


            //LEFT Frame; is the black collapsed bar that reamins at the left when the SplitView Left Pane is collapsed
            this.LeftFrameWhenPanelIsCollapsed.Tapped += delegate {
                  Flag_Skip_LeftFrameWhenPanelIsCollapsed_PointerEntered_Until_PointerIsOutOfLeftNavBar = false;
                  SetPanelState(eSplittedPanelState.Open);
            };

            this.LeftFrameWhenPanelIsCollapsed.PointerEntered += delegate {
                if (Flag_Skip_LeftFrameWhenPanelIsCollapsed_PointerEntered_Until_PointerIsOutOfLeftNavBar == true)
                {
                    return;
                }

                SetPanelState(eSplittedPanelState.Open);
            };

            this.LeftFrameWhenPanelIsCollapsed.PointerExited += delegate {
                if (Flag_Skip_LeftFrameWhenPanelIsCollapsed_PointerEntered_Until_PointerIsOutOfLeftNavBar == true)
                {
                    return;
                }

                SetPanelState(eSplittedPanelState.Open);
            };
            ///////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////

        }



        //Pull to Refresh functionality
        private async void RefreshContainer_LeftPanel_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            await ((ViewModels.MainSplitted_ViewModel)this.DataContext).RefreshEntriesAsync();
        }

        
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight > this.ActualWidth)
            {
                _DisplayMode = eDisplayMode.Portrait;
                SetPanelState(eSplittedPanelState.Close);
            }
            else
            {
                _DisplayMode = eDisplayMode.Landscape;
                SetPanelState(eSplittedPanelState.Open);
            }
        }

        private void SetPanelState(eSplittedPanelState state)
        {
            if (_DisplayMode == eDisplayMode.Portrait)
            {
                if (state == eSplittedPanelState.Close)
                {
                    VisualStateManager.GoToState(this, "Portrait_NavClose", true);
                    VisibleCol_WhenPanelIsCollapsed.Width = new GridLength(0.2, GridUnitType.Star);  //Wider so user can click it and open the Panel Again
                }
                else
                {
                    VisualStateManager.GoToState(this, "Portrait_NavOpen", true);
                    VisibleCol_WhenPanelIsCollapsed.Width = new GridLength(0, GridUnitType.Star);
                }
            }
            else
            {
                VisualStateManager.GoToState(this, "Landscape", true);
                VisibleCol_WhenPanelIsCollapsed.Width = new GridLength(0, GridUnitType.Star);
            }
        }

        //Calculates how many Reddit Posts are visible so they can be Deleted in a batch
        private async void btnDismissVisibles_Click(object sender, RoutedEventArgs e)
        {
            ///FF: This story is exclusively related to the ListView Control in the UI, thats why is in the View side

            List<Data.Child> ListToDismiss = new List<Data.Child>();

            foreach (Data.Child item in this.ListView_MainThread.Items)
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

                   await ((ViewModels.MainSplitted_ViewModel)this.DataContext).DismissEntriesAsync(ListToDismiss);

        }

      


    }
}
