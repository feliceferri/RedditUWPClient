using RedditUWPClient.Helpers;
using RedditUWPClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace RedditUWPClient.Models
{
    internal class IncrementalLoadingCollectionOfEntries : ObservableCollection<Models.Child>, ISupportIncrementalLoading
    {

        ViewModels.VM_MainPage _vm_MainPage;

        public IncrementalLoadingCollectionOfEntries(ViewModels.VM_MainPage vm_MainPage)
        {
            _vm_MainPage = vm_MainPage;
        }
        
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run(async cancelToken =>
            {
                string LastEntryID = "";

                if (Count > 0)
                {
                    var last = this.LastOrDefault();
                    if (last != null)
                    {
                        LastEntryID = last.data.id;
                    }
                }

                Reddit reddit = new Reddit();
                var res = await reddit.GetEntriesAsync(10, LastEntryID);
                if (res.Success == true)
                {
                    await _vm_MainPage.FilterEntriesAsync(res.value.data.children);

                    foreach(var item in res.value.data.children)
                    {
                        Add(item);
                    }

                    return new LoadMoreItemsResult { Count = count };
                }
                else
                {
                    return new LoadMoreItemsResult { Count = 0 };
                }
            });
                        
        }

        public bool HasMoreItems => true;
    }
}
