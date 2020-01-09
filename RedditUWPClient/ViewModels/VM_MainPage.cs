using RedditUWPClient.Helpers;
using RedditUWPClient.Models;
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
           LoadEntriesAsync(); //FF: Cant and doesnt need to be awaited as the UI will be notified when the IObservableCollection is filled
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ObservableCollection<Child> _Reddit_Entries = null;
        public ObservableCollection<Child> Reddit_Entries { 
            get {return _Reddit_Entries;}  
            set { _Reddit_Entries = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task LoadEntriesAsync()
        {
            try
            {
                
                Reddit reddit = new Reddit();
                var res = await reddit.GetEntriesAsync();

                if (res.Success == true)
                {
                    //Reddit_Entries = null;
                    Reddit_Entries = new ObservableCollection<Child>(res.value.data.children);
                    //Reddit_Entries = SamplingData.RedditEntries;
                    Reddit_Entries.RemoveAt(0);
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
        }



        
    }
}
