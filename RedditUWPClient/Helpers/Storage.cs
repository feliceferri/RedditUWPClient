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

                    StorageFile destinationFile = await KnownFolders.SavedPictures.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
                   
                        using (var destinationStream = (await destinationFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false)).GetOutputStreamAt(0))
                        {
                            await RandomAccessStream.CopyAndCloseAsync(imageStream, destinationStream).AsTask().ConfigureAwait(false);
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

        /// <summary>
        /// FF: Will Replace Existing File
        /// </summary>
        /// <param name="fileNameWithExtension"></param>
        /// <param name=""></param>
        internal async Task<NoParam> WriteTextToFileAsync(string fileNameWithExtension, string data)
        {
            var res = new NoParam();
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                StorageFile file = await localFolder.CreateFileAsync(fileNameWithExtension,
                    CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
                await FileIO.WriteTextAsync(file, data).AsTask().ConfigureAwait(false);
                res.Success = true;
            }
            catch(Exception ex)
            {
                res.Error = ex;
            }
            return res;
        }

        internal async Task<SingleParam<string>> ReadTextFromFileAsync(string fileNameWithExtension)
        {
            var res = new SingleParam<String>();
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                StorageFile file = await localFolder.GetFileAsync(fileNameWithExtension).AsTask().ConfigureAwait(false);
                res.value = await FileIO.ReadTextAsync(file).AsTask().ConfigureAwait(false);
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
