using RedditUWPClient.Helpers;
using RedditUWPClient.Data;
using RedditUWPClient.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RedditUWPClient.ViewModels
{
    internal class MainSplitted_ViewModel : INotifyPropertyChanged
    {

        private Models.MainSplitted_Model _model = new Models.MainSplitted_Model();

        public delegate void EntrySelectedHandler(Data.Child Entry);
        public event EntrySelectedHandler EntrySelected;

        public MainSplitted_ViewModel()
        {

            cmdCloseFlyOut = new NoParamCommand(CloseFlyOut);
            cmdSaveToGallery = new NoParamCommandAsync(SaveToGallery);
            cmdDismissEntry = new ParamCommand<Data.Data1>(DismissEntryAsync);
            cmdEnlargePicture = new NoParamCommand(EnlargePicture);

            Reddit_Entries = Task.Run(() => _model.LoadEntriesAsync(Services.SuspensionManager.PointerTo_ListOfEntries)).Result; //FF: Cant and doesnt need to be awaited as the UI will be notified when the IObservableCollection is filled
             SelectedEntry = Services.SuspensionManager.PointerTo_SelectedEntry;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnEntrySelected(Data.Child Entry)
        {
            if (EntrySelected != null)
            {
                EntrySelected(Entry);
            }
        }

        #region Properties


        static IncrementalLoadingCollectionOfEntries _Reddit_Entries = null;
        public IncrementalLoadingCollectionOfEntries Reddit_Entries
        {
            get { return _Reddit_Entries; }
            set
            {
                _Reddit_Entries = value;
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
                    SelectedEntry.data.Read = true;
                    OnEntrySelected(SelectedEntry);
                    Services.SuspensionManager.PointerTo_SelectedEntry = _SelectedEntry;
                    Task.Run(async () => { await new Services.Persistance().AddReadFlagToReadHistoryAsync(SelectedEntry.data.id); }).Wait();
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

        bool _ShowReadFlag = false;
        public bool ShowReadFlag
        {
            get { return _ShowReadFlag; }
            set
            {
                _ShowReadFlag = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public NoParamCommand cmdCloseFlyOut { get; set; }
        public NoParamCommandAsync cmdSaveToGallery { get; set; }
        public ParamCommand<Data.Data1> cmdDismissEntry { get; set; }
        public NoParamCommand cmdEnlargePicture { get; set; }
        public NoParamCommand cmdDismissAll { get; set; }



        #endregion



        internal async Task RefreshEntriesAsync()
        {
            this.Processing = true;
            Reddit_Entries = await _model.LoadEntriesAsync(null);
            this.Processing = false;
        }


        internal void CloseFlyOut()
        {
            ShowFlyOutImage = false;
        }


        private void EnlargePicture()
        {
            //Show Flyout
            ShowFlyOutImage = true;
            ShowSaveImageButton = true;
        }

        internal async Task SaveToGallery()
        {
            if (await _model.SaveToGallery(this.SelectedEntry) == true)
            {
                ShowSaveImageButton = false;
            }
        }

        internal async void DismissEntryAsync(Data.Data1 entry)
        {
            await _model.DismissEntryAsync(Reddit_Entries,entry);
        }

        internal async Task DismissEntriesAsync(List<Child> entries)
        {
            await _model.DismissEntriesAsync(Reddit_Entries,entries);
        }
    }
}
