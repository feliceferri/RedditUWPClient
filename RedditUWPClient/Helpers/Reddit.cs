using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditUWPClient.Helpers;

namespace RedditUWPClient.Helpers
{
    internal class Reddit
    {

        const string Reddit_Top_URL = @"https://www.reddit.com/top.json";

        internal async Task<Responses.SingleParam<Models.Reddit_Entry>> GetEntriesAsync()
        {
            var res = new Responses.SingleParam<Models.Reddit_Entry>();

            try
            {
                Network network = new Network();
                var networkResponse = await network.GetJsonPayLoadAsync(Reddit_Top_URL);
                if(networkResponse.Success == true)
                {
                    res.value = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Reddit_Entry>(networkResponse.value);
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
      
    }
}
