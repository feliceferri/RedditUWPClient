using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static RedditUWPClient.Helpers.Responses;

namespace RedditUWPClient.Helpers
{
    internal class Network: IDisposable
    {
        bool disposed = false;
        private readonly HttpClient _httpClient = null;

        public Network()
        {
            _httpClient = new HttpClient();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _httpClient.Dispose(); //HttpClient
             }

            disposed = true;
        }

        internal async Task<SingleParam<string>> GetJsonPayLoadAsync(string URL)
        {
            SingleParam<string> res = new SingleParam<string>();

            try
            {

                using (HttpResponseMessage responseMessage = await _httpClient.GetAsync(URL))
                {
                    responseMessage.EnsureSuccessStatusCode();
                    res.value = await responseMessage.Content.ReadAsStringAsync();

                    res.Success = true;
                }
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
                byte[] buffer = await _httpClient.GetByteArrayAsync(URL);
                
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
