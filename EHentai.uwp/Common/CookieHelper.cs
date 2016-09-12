using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EHentai.uwp.Common
{
    public class CookieHelper
    {
        private static readonly string _cookieFileName = "EHentai.Cookie";

        private static CookieContainer _cookie;

        public static CookieContainer Cookie
        {
            get { return _cookie = GetCookie().Result; }
            set
            {
                _cookie = value;
                SaveCookies(value);
            }
        }

        /// <summary>
        /// 保存Cookie
        /// </summary>
        /// <param name="cookie"></param>
        private static async void SaveCookies(CookieContainer cookie)
        {
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(CookieContainer));
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                var oldFile = storageFolder.TryGetItemAsync(_cookieFileName).GetResults();
                if (oldFile != null)
                {
                    await oldFile.DeleteAsync();
                }
                StorageFile file = await storageFolder.CreateFileAsync(_cookieFileName, CreationCollisionOption.OpenIfExists);
                using (Stream stream = await file.OpenStreamForWriteAsync())
                {
                    serializer.WriteObject(stream, cookie);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 读取Cookie
        /// </summary>
        /// <returns></returns>
        private static async Task<CookieContainer> GetCookie()
        {
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(CookieContainer));
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await storageFolder.GetFileAsync(_cookieFileName);
                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    return (CookieContainer)serializer.ReadObject(stream);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
