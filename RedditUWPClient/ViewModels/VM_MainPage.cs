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
using Windows.UI.Popups;

namespace RedditUWPClient.ViewModels
{
    internal class VM_MainPage: INotifyPropertyChanged
    {

        public delegate void EntrySelectedHandler(Models.Child Entry);
        public event EntrySelectedHandler EntrySelected;

        public VM_MainPage()
        {

           
           cmdCloseFlyOut = new NoParamCommand(CloseFlyOut);
           cmdSaveToGallery = new NoParamCommandAsync(SaveToGallery);
           cmdDismissEntry = new ParamCommand<Models.Data1>(DismissEntry);
           cmdEnlargePicture = new NoParamCommand(EnlargePicture);
           
            
            LoadEntriesAsync(Services.SuspensionManager.PointerTo_ListOfEntries); //FF: Cant and doesnt need to be awaited as the UI will be notified when the IObservableCollection is filled
            SelectedEntry = Services.SuspensionManager.PointerTo_SelectedEntry;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnEntrySelected(Models.Child Entry)
        {
            if(EntrySelected != null)
            {
                EntrySelected(Entry);
            }
        }

#region Properties
        static ObservableCollection<Child> _Reddit_Entries = null;

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
                    SelectedEntry.data.Read = true;
                    OnEntrySelected(SelectedEntry);
                    Services.SuspensionManager.PointerTo_SelectedEntry = _SelectedEntry;
                    new Services.Persistance().AddReadFlagToReadHistoryAsync(SelectedEntry.data.id);
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
        public ParamCommand<Models.Data1> cmdDismissEntry { get; set; }
        public NoParamCommand cmdEnlargePicture { get; set; }
        public NoParamCommand cmdDismissAll { get; set; }



        #endregion

        private async Task LoadEntriesAsync(List<Child> Entries)
        {
            try
            {
                Processing = true;

                if (Entries == null || Entries.Count == 0)
                {
                    Reddit reddit = new Reddit();
                    var res = await reddit.GetEntriesAsync();
                    if (res.Success == false)
                    {
                        Reddit_Entries = null;
                        return;
                    }

                    Entries = res.value.data.children;
                }

                
                   

                    //Load ReadHistory and Match it by id
                    try
                    {
                        HashSet<string> hashIds = await new Services.Persistance().LoadReadHistoryAsync();
                        foreach(var item in Entries)
                        {
                            if(hashIds.Contains(item.data.id))
                            {
                                item.data.Read = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //Main flow (showing posts is more important thatn persistance of Read posts
                        //So always go along
                    }

                    //Remove the Dismissed ones, Match it by id
                    try
                    {
                        HashSet<string> hashIds = await new Services.Persistance().LoadDismissedAsync();
                        List<Child> ListToRemove = new List<Child>();
                        foreach (var item in Entries)
                        {
                            if (hashIds.Contains(item.data.id))
                            {
                                ListToRemove.Add(item);
                            }
                        }

                        foreach(var item in ListToRemove)
                        {
                        Entries.Remove(item);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        //Main flow (showing posts is more important thatn persistance of Dismissed posts
                        //So always go along
                    }

                    Services.SuspensionManager.PointerTo_ListOfEntries = Entries;
                    Reddit_Entries = new ObservableCollection<Child>(Entries);
                
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

      

        private void DismissEntry(Models.Data1 entry)
        {
            
            var Child = (from p in Reddit_Entries
                         where p.data == entry
                         select p)
                        .FirstOrDefault();
            if (Child != null)
            {
                Reddit_Entries.Remove(Child);
                new Services.Persistance().AddDismissedAsync(Child.data.id);
            }
            
        }

        internal async Task DismissEntries(List<Child> entries)
        {
            if (entries != null)
            {
                try
                {
                    List<string> IdsToDismiss = new List<string>();
                    foreach (var entry in entries)
                    {
                        IdsToDismiss.Add(entry.data.id);
                        _Reddit_Entries.Remove(entry);
                    }

                    new Services.Persistance().AddDismissedAsync(IdsToDismiss);
                }
                catch (Exception ex)
                {
                    var messageDialog = new MessageDialog("Could not Dismiss all the visible entries." + Environment.NewLine + "Details: " + ex.Message);
                    await messageDialog.ShowAsync();
                }
                
            }
            

        }

        
        

        private void EnlargePicture()
        {
            //Show Flyout
            ShowFlyOutImage = true;
            ShowSaveImageButton = true;
        }
        
    }
}
