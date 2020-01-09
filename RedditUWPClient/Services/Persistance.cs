using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

            var res = await SaveJsonAsync<HashSet<string>>(ReadHistoryFileNameWithExt, hashSet);
            if(res.Success == false)
            {
                var messageDialog = new MessageDialog("Could not persist the History of Read posts." + Environment.NewLine + "Details: " + res.Error.Message);
                await messageDialog.ShowAsync();
            }
            
        }

        internal async Task<HashSet<string>> LoadReadHistoryAsync()
        {

            var res = await LoadJsonAsync<HashSet<string>>(ReadHistoryFileNameWithExt);
            if (res.Success == true)
            {
                return res.value;
            }
            else
            {
                return null;
            }

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

            var res = await SaveJsonAsync<HashSet<string>>(DissmissedFileNameWithExt, hashSet);
            if (res.Success == false)
            {
                var messageDialog = new MessageDialog("Could not persist the Dismiss action." + Environment.NewLine + "Details: " + res.Error.Message);
                await messageDialog.ShowAsync();
            }

            }
       

        internal async Task<HashSet<string>> LoadDismissedAsync()
        {
            
            var res = await LoadJsonAsync<HashSet<string>>(DissmissedFileNameWithExt); 
            if(res.Success == true)
            {
                return res.value;
            }
            else
            {
                return null;
            }
            
        }

        internal async Task<Helpers.Responses.NoParam> SaveJsonAsync<T>(string FileNameWithExt, T data)
        {
            Helpers.Responses.NoParam res = new Helpers.Responses.NoParam();
            try
            {
                var storage = new Helpers.Storage();
                await storage.WriteTextToFileAsync(FileNameWithExt, Newtonsoft.Json.JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                res.Error = ex;
            }

            return res;
        }

        internal async Task<Helpers.Responses.SingleParam<T>> LoadJsonAsync<T>(string FileNameWithExt)
        {
            Helpers.Responses.SingleParam<T> res = new Helpers.Responses.SingleParam<T>();

            try
            {
                var storage = new Helpers.Storage();
                var resData = await storage.ReadTextFromFileAsync(FileNameWithExt);
                if (resData.Success == true)
                {
                    res.value = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(resData.value); ;
                    res.Success = true;
                }
            }
            catch (Exception ex)
            {
                res.Error = ex;
            }

            return res;
        }
    }
}
