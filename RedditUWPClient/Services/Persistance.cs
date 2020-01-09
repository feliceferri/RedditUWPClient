using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RedditUWPClient.Services
{
    internal class Persistance
    {
        private const string ReadHistoryFileNameWithExt = "ReadHistory.txt";
        private const string DissmissedFileNameWithExt = "Dismissed.txt";

        internal async Task AddReadFlagToReadHistoryAsync(string id)
        {
            HashSet<string> hashSet = await LoadReadHistoryAsync();
            if (hashSet == null)
            {
                hashSet = new HashSet<string>();
            }

            hashSet.Add(id);

            try
            {
                var storage = new Helpers.Storage();
                storage.WriteTextToFileAsync(ReadHistoryFileNameWithExt, Newtonsoft.Json.JsonConvert.SerializeObject(hashSet));
            }
            catch (Exception ex)
            {
                var messageDialog = new MessageDialog("Could not persist the History of Read posts." + Environment.NewLine + "Details: " + ex.Message);
                await messageDialog.ShowAsync();
            }
        }

        internal async Task<HashSet<string>> LoadReadHistoryAsync()
        {
            HashSet<string> res = null;

            try
            {
                var storage = new Helpers.Storage();
                var resData = await storage.ReadTextFromFileAsync(ReadHistoryFileNameWithExt);
                if (resData.Success == true)
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

        internal async Task AddDismissedAsync(string id)
        {
            await AddDismissedAsync(new List<string>() { id });
        }

        internal async Task AddDismissedAsync(List<string> Ids)
        {
            HashSet<string> hashSet = await LoadDismissedAsync();
            if (hashSet == null)
            {
                hashSet = new HashSet<string>();
            }

            foreach (string id in Ids)
            {
                hashSet.Add(id);
            }

            try
            {
                var storage = new Helpers.Storage();
                storage.WriteTextToFileAsync(DissmissedFileNameWithExt, Newtonsoft.Json.JsonConvert.SerializeObject(hashSet));
            }
            catch (Exception ex)
            {
                var messageDialog = new MessageDialog("Could not persist the Dismiss action." + Environment.NewLine + "Details: " + ex.Message);
                await messageDialog.ShowAsync();
            }
        }
       

        internal async Task<HashSet<string>> LoadDismissedAsync()
        {
            HashSet<string> res = null;

            try
            {
                var storage = new Helpers.Storage();
                var resData = await storage.ReadTextFromFileAsync(DissmissedFileNameWithExt);
                if (resData.Success == true)
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
    }
}
