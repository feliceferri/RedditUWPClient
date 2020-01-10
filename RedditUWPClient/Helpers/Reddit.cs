using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RedditUWPClient.Helpers;

namespace RedditUWPClient.Helpers
{
    internal class Reddit
    {

        const string Reddit_Top_URL = @"https://www.reddit.com/top.json";

        internal async Task<Responses.SingleParam<Models.Reddit_Entry>> GetEntriesAsync(int NumberOfEntries = 50, string After_Id ="")
        {
            var res = new Responses.SingleParam<Models.Reddit_Entry>();

            try
            {
                if(NumberOfEntries > 50)
                {
                    throw new Exception("Reddit API allows max 50 entries for retreival");
                }

                string URL = Reddit_Top_URL + "?limit=" + NumberOfEntries;
                if(string.IsNullOrWhiteSpace(After_Id) == false)
                {
                    URL += "&after=" + After_Id;
                }

                Network network = new Network();

                var networkResponse = await network.GetJsonPayLoadAsync(URL);
                if(networkResponse.Success == true)
                {
                    res.value = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Reddit_Entry>(networkResponse.value, new JsonSerializerSettings
                    {
                        Error = HandleDeserializationError
                    });
                    res.Success = true;
                }
                else
                {
                    res.Error = networkResponse.Error;
                }
                
            }   
            catch(Exception ex)
            {
                res.Error = ex;
            }

            return res;
        }

        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
#if DEBUG
            var x = 12;
            //FF: This is a breakpoint that will be fired only in Debug mode to notify that something changed in the Json payload
            //in terms of schema.
            //Happen to me once, some int image fields were suppose to be nullable and found it out in the 2nd day of work
            //after download dozen of json payloads.
#endif
        }
    }
}
