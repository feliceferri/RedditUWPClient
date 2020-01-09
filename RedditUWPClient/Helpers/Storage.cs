using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using static RedditUWPClient.Helpers.Responses;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

namespace RedditUWPClient.Helpers
{
    internal class Storage
    {

        internal async Task<NoParam> SavePictureInGalleryAsync(string fileName, byte[] image)
        {
            NoParam res = new NoParam();

            try
            {
                using (MemoryStream memoryStream = new MemoryStream(image))
                {
                   IRandomAccessStream imageStream = memoryStream.AsRandomAccessStream();

                    StorageFile destinationFile = await KnownFolders.SavedPictures.CreateFileAsync(fileName,CreationCollisionOption.ReplaceExisting);
                   
                        using (var destinationStream = (await destinationFile.OpenAsync(FileAccessMode.ReadWrite)).GetOutputStreamAt(0))
                        {
                            await RandomAccessStream.CopyAndCloseAsync(imageStream, destinationStream);
                        }
                    
                }

                       
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
