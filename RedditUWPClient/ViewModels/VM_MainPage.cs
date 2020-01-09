using RedditUWPClient.Helpers;
using RedditUWPClient.Models;
using RedditUWPClient.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RedditUWPClient.ViewModels
{
    internal class VM_MainPage: INotifyPropertyChanged
    {

        public VM_MainPage()
        {
           cmdCloseFlyOut = new NoParamCommand(CloseFlyOut);
           cmdSaveToGallery = new NoParamCommandAsync(SaveToGallery);
                       
           LoadEntriesAsync(); //FF: Cant and doesnt need to be awaited as the UI will be notified when the IObservableCollection is filled
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


#region Properties
        ObservableCollection<Child> _Reddit_Entries = null;

        public ObservableCollection<Child> Reddit_Entries { 
            get {return _Reddit_Entries;}  
            set { _Reddit_Entries = value;
                NotifyPropertyChanged();
            }
        }

        Child _SelectedEntry = null;
        public Child SelectedEntry
        {
            get { return _SelectedEntry; }
            set
            {
                _SelectedEntry = value;
                NotifyPropertyChanged();

                if (_SelectedEntry != null)
                {
                    //Show Flyout
                    ShowFlyOutImage = true;
                    ShowSaveImageButton = true;
                }
            }
        }

        bool _Processing = false;
        public bool Processing
        {
            get { return _Processing; }
            set
            {
                _Processing = value;
                NotifyPropertyChanged();
            }
        }

        bool _ShowFlyOutImage = false;
        public bool ShowFlyOutImage
        {
            get { return _ShowFlyOutImage; }
            set
            {
                _ShowFlyOutImage = value;
                NotifyPropertyChanged();
            }
        }

        bool _ShowSaveImageButton = false;
        public bool ShowSaveImageButton
        {
            get { return _ShowSaveImageButton; }
            set
            {
                _ShowSaveImageButton = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Commands

        public NoParamCommand cmdCloseFlyOut { get; set; }
        public NoParamCommandAsync cmdSaveToGallery { get; set; }


        #endregion

        private async Task LoadEntriesAsync()
        {
            try
            {
                Processing = true;

                Reddit reddit = new Reddit();
                var res = await reddit.GetEntriesAsync();

                if (res.Success == true)
                {
                    Reddit_Entries = new ObservableCollection<Child>(res.value.data.children);
                }
                else
                {
                    Reddit_Entries = null;
                }
            }
            catch (Exception ex)
            {
                Reddit_Entries = null;
            }
            finally
            {
                Processing = false;
            }
        }

        internal void CloseFlyOut()
        {
            ShowFlyOutImage = false;
            SelectedEntry = null; //FF: So the user can select it again, to see the image for 2nd time
        }

        internal async Task SaveToGallery()
        {
            var resPicture = await new Network().GetPictureFromURLAsync(SelectedEntry.data.url);
            if (resPicture.Success == true)
            {
                var resSaving = await new Storage().SavePictureInGalleryAsync(SelectedEntry.data.id + ".jpg", resPicture.value);
                if (resSaving.Success == true)
                {
                    ShowSaveImageButton = false;
                }
            }
        }


    }
}
