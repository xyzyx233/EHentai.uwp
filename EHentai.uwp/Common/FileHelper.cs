using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace EHentai.uwp.Common
{
    public class FileHelper
    {
        public static StorageFolder LoaclFolder = ApplicationData.Current.LocalFolder;

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
                StorageFile file = await LoaclFolder.GetFileAsync(path + "\\" + name);
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
        
    }
}
