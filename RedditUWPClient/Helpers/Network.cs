using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static RedditUWPClient.Helpers.Responses;

namespace RedditUWPClient.Helpers
{
    internal class Network
    {
        internal async Task<SingleParam<string>> GetJsonPayLoadAsync(string URL)
        {
            SingleParam<string> res = new SingleParam<string>();

            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage  responseMessage= await httpClient.GetAsync(URL);
                responseMessage.EnsureSuccessStatusCode();
                res.value = await responseMessage.Content.ReadAsStringAsync();

                res.Success = true;
            }
            catch(Exception ex)
            {
                res.Error = ex;
            }

            return res;
        }

        internal async Task<SingleParam<byte[]>> GetPictureFromURLAsync(string URL)
        {
            SingleParam<byte[]> res = new SingleParam<byte[]>();

            try
            {
                HttpClient httpClient = new HttpClient();
                byte[] buffer = await httpClient.GetByteArrayAsync(URL);
                
                res.value = buffer;

                res.Success = true;
            }
            catch (Exception ex)
            {
                res.Error = ex;
            }

            return res;
        }
    }
}
