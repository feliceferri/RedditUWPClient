using RedditUWPClient.Data;
using RedditUWPClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RedditUWPClient.Models
{
    internal class MainSplitted_Model
    {

        internal bool LoadingEntries { get; private set; }

        internal async Task<IncrementalLoadingCollectionOfEntries> LoadEntriesAsync(List<Child> Entries)
        {
            IncrementalLoadingCollectionOfEntries Response_Entries = null;
            LoadingEntries = true;
            
            try
            {
              
                if (Entries == null || Entries.Count == 0)
                {
                    Reddit reddit = new Reddit();
                    var res = await reddit.GetEntriesAsync(Reddit.eKindOfGet.CleanSearchFromTheBeggining, 10);
                    if (res.Success == false)
                    {
                        return null;
                    }

                    Entries = res.value.data.children;
                }

                //To Tag the Read ones, and remove the Dissmissed ones.
                await FilterEntriesAsync(Entries);

                Services.SuspensionManager.PointerTo_ListOfEntries = Entries;
                //Reddit_Entries = new ObservableCollection<Child>(Entries);
                Response_Entries = new IncrementalLoadingCollectionOfEntries(this);
                foreach (var item in Entries)
                {

                    Response_Entries.Add(item);
                }


            }
            catch
            {
                Response_Entries = null;
            }
            finally
            {
               LoadingEntries = false;
            }

            return Response_Entries;
        }

        object Lockobj = new object();

        /// <summary>
        /// If is in the Read History it will tag it as readed
        /// If has been dismissed it wont be shown
        /// </summary>
        internal async Task FilterEntriesAsync(List<Child> Entries)
        {
            //FF: This Method will be accessed also by the Incremental Loading of the Listview
            //    Thats why I'm a adding a Lock to avoid two processes altering the Collection at once
            try
            {
                HashSet<string> hashIds = await new Services.Persistance().LoadReadHistoryAsync();
                lock (Lockobj)
                {
                    foreach (var item in Entries)
                    {
                        if (hashIds.Contains(item.data.id))
                        {
                            item.data.Read = true;
                        }
                    }
                }
            }
            catch
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

                lock (Lockobj)
                {
                    foreach (var item in ListToRemove)
                    {
                        Entries.Remove(item);
                    }
                }

            }
            catch
            {
                //Main flow (showing posts is more important thatn persistance of Dismissed posts
                //So always go along
            }
        }

        internal async Task DismissEntryAsync(IncrementalLoadingCollectionOfEntries Reddit_Entries, Data.Data1 EntryToDismiss)
        {
            if (EntryToDismiss != null )
            {
                var Child = (from p in Reddit_Entries
                             where p.data == EntryToDismiss
                             select p)
                        .FirstOrDefault();
                if (Child != null)
                {
                    Reddit_Entries.Remove(Child);
                    await new Services.Persistance().AddDismissedAsync(Child.data.id);
                }
            }

        }

        internal async Task DismissEntriesAsync(IncrementalLoadingCollectionOfEntries Reddit_Entries, List<Child> EntriesToDismiss)
        {
            if (EntriesToDismiss != null)
            {
                try
                {
                    List<string> IdsToDismiss = new List<string>();
                    foreach (var entry in EntriesToDismiss)
                    {
                        IdsToDismiss.Add(entry.data.id);
                        Reddit_Entries.Remove(entry);
                    }

                    await new Services.Persistance().AddDismissedAsync(IdsToDismiss);
                }
                catch (Exception ex)
                {
                    var messageDialog = new MessageDialog("Could not Dismiss all the visible entries." + Environment.NewLine + "Details: " + ex.Message);
                    await messageDialog.ShowAsync();
                }

            }


        }

        internal async Task<bool> SaveToGallery(Child SelectedEntry)
        {
            using (var network = new Network())
            {
                var resPicture = await network.GetPictureFromURLAsync(SelectedEntry.data.url);
                if (resPicture.Success == true)
                {
                    var resSaving = await new Storage().SavePictureInGalleryAsync(SelectedEntry.data.id + ".jpg", resPicture.value);
                    if (resSaving.Success == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


    }
}
