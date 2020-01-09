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

        private const string ReadHistoryFileNameWithExt = "ReadHistory.txt";

        public VM_MainPage()
        {

           
           cmdCloseFlyOut = new NoParamCommand(CloseFlyOut);
           cmdSaveToGallery = new NoParamCommandAsync(SaveToGallery);
           cmdDismissEntry = new ParamCommand<Models.Data1>(DismissEntry);


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
                    SelectedEntry.data.Read = true;
                    AddReadFlagToReadHistoryAsync(SelectedEntry.data.id);
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
                    List<Models.Child> ListEntries = res.value.data.children;

                    //Load ReadHistory and Match it by id
                    try
                    {
                        HashSet<string> hashIds = await LoadReadHistoryAsync();
                        foreach(var item in ListEntries)
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

                    Reddit_Entries = new ObservableCollection<Child>(ListEntries);
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

        private async Task AddReadFlagToReadHistoryAsync(string id)
        {
            HashSet<string> hashSet = await LoadReadHistoryAsync();
            if(hashSet == null)
            {
                hashSet = new HashSet<string>(); 
            }

            hashSet.Add(id);

            try
            {
                var storage = new Helpers.Storage();
                storage.WriteTextToFileAsync(ReadHistoryFileNameWithExt, Newtonsoft.Json.JsonConvert.SerializeObject(hashSet));
            }
            catch(Exception ex)
            {
                var messageDialog = new MessageDialog("Could not persist the History of Read posts." + Environment.NewLine + "Details: " + ex.Message);
            }
        }

        private async Task<HashSet<string>> LoadReadHistoryAsync()
        {
            HashSet<string> res = null;

            try
            {
                var storage = new Helpers.Storage();
                var resData = await storage.ReadTextFromFileAsync(ReadHistoryFileNameWithExt);
                if(resData.Success == true)
                {
                    res = Newtonsoft.Json.JsonConvert.DeserializeObject<HashSet<string>>(resData.value);
                }
            }
            catch (Exception ex)
            {
                //FF: Wont show an error message. by returning null on the next pass it automatically will create a new file
            }

            return res;
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
            }
            
        }
    }
}
