using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;

namespace Uwp.Common
{
    public class FileHelper
    {
        public static StorageFolder LoaclFolder = ApplicationData.Current.LocalFolder;
        
        public static FileSavePicker Savepicker
        {
            get { return GetClass<FileSavePicker>("DownPath").Result; }
        }

        /// <summary>
        /// 获取文件夹
        /// </summary>
        /// <param name="path">路径</param>
        public static async Task<StorageFolder> GetFolderAsync(string path = null)
        {
            try
            {
                StorageFolder folder;
                if (string.IsNullOrEmpty(path))
                {
                    folder = LoaclFolder;
                }
                else
                {
                    folder = await LoaclFolder.TryGetItemAsync(path) as StorageFolder;
                }

                return folder;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 判断文件夹是否存在
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool ExistsFolder(string path)
        {
            try
            {
                //var folder = GetFolder(path);
                var folder = GetFolderAsync(path).Result;
                return folder != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path">路径</param>
        public static void CreateFolder(string path)
        {
            try
            {
                if (!ExistsFolder(path))
                {
                    var folder = LoaclFolder.CreateFolderAsync(path).GetResults();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="path">路径</param>
        public static async Task<StorageFile> GetFile(string name, string path = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && name.Contains(path))
                {
                    path = "";
                }
                else
                {
                    path = path + "\\";
                }
                StorageFile file = await LoaclFolder.GetFileAsync(path + name).AsTask().ConfigureAwait(false);
                return file;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static async Task<bool> ExistsFile(string name, string path = "")
        {
            try
            {
                StorageFile file = await GetFile(name, path);
                return file != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="path">路径</param>
        public static async void CreateFile(Stream stream, string name, string path = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && !ExistsFolder(path))
                {
                    CreateFolder(path);
                }


                StorageFile file = await LoaclFolder.CreateFileAsync(path + "\\" + name, CreationCollisionOption.ReplaceExisting);
                List<byte> allbytes = new List<byte>();
                byte[] buffer = new byte[1000];
                int bytesRead = 0;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    allbytes.AddRange(buffer.Take(bytesRead));
                }

                await FileIO.WriteBytesAsync(file, allbytes.ToArray());
                stream.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<bool> CreateImage(SoftwareBitmap bitmap, string name, string path = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && !ExistsFolder(path))
                {
                    CreateFolder(path);
                }

                SoftwareBitmap softBmp = bitmap;


                StorageFile file = await LoaclFolder.CreateFileAsync(path + "\\" + name, CreationCollisionOption.ReplaceExisting);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    encoder.SetSoftwareBitmap(softBmp);
                    await encoder.FlushAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Stream GetFileStream(string name, string path = "")
        {
            try
            {
                StorageFile file = GetFile(name, path).Result;

                return file.OpenStreamForReadAsync().Result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<IRandomAccessStream> GetAccessStreamAsync(string filename, string path = "")
        {
            try
            {
                StorageFile file = await GetFile(filename, path);
                if (file == null)
                {
                    return null;
                }

                return await file.OpenAsync(FileAccessMode.Read);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async void DownFile(string fileName, byte[] fileBytes, string extension = "")
        {
            try
            {
                FileSavePicker savepicker = Savepicker ?? new FileSavePicker();
                savepicker.SuggestedStartLocation = PickerLocationId.Desktop;
                savepicker.DefaultFileExtension = extension;
                savepicker.SuggestedFileName = fileName;
                savepicker.FileTypeChoices.Add(extension.Replace(".", ""), new List<string> { extension });

                StorageFile file = await savepicker.PickSaveFileAsync();
                if (file != null)
                {

                    CachedFileManager.DeferUpdates(file);
                    await FileIO.WriteBytesAsync(file, fileBytes);

                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                    //SaveClass(savepicker, "DownPath");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async void SaveClass<T>(T t, string fileName)
        {
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                StorageFolder storageFolder = LoaclFolder;
                var oldFile = storageFolder.TryGetItemAsync(fileName).GetResults();
                if (oldFile != null)
                {
                    await oldFile.DeleteAsync();
                }
                StorageFile file = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                using (Stream stream = await file.OpenStreamForWriteAsync())
                {
                    serializer.WriteObject(stream, t);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task<T> GetClass<T>(string fileName)
        {
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await storageFolder.GetFileAsync(fileName);
                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
    }
}
