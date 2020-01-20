using RedditUWPClient.Helpers;
using RedditUWPClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace RedditUWPClient.Data
{
    internal class IncrementalLoadingCollectionOfEntries : ObservableCollection<Child>, ISupportIncrementalLoading
    {

        Reddit _reddit = null;
        Models.MainSplitted_Model _mainSplittedModel;

        private bool _LoadingEntries = false;

        public IncrementalLoadingCollectionOfEntries(Models.MainSplitted_Model mainSplittedModel)
        {
            _mainSplittedModel = mainSplittedModel;
            _reddit = new Reddit();
        }
        
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run(async cancelToken =>
            {
                if(_mainSplittedModel.LoadingEntries == true || _LoadingEntries == true)
                {
                    return new LoadMoreItemsResult { Count = 0 };
                }

                _LoadingEntries = true;
                string LastEntryID = "";

                if (Count > 0)
                {
                    var last = this.OrderByDescending(x=> x.data.created_utc).FirstOrDefault();
                    if (last != null)
                    {
                        LastEntryID = last.data.name;
                    }
                }

                //Debug.WriteLine("Count: " + Count + " LastEntryID: " + LastEntryID);

                var res = await _reddit.GetEntriesAsync(Reddit.eKindOfGet.AfterLastEntry);
                
                _LoadingEntries = false;
                
                if (res.Success == true)
                {
                    await _mainSplittedModel.FilterEntriesAsync(res.value.data.children);

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
