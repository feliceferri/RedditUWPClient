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



        
    }
}
