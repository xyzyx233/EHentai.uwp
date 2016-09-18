using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Uwp.Common;

namespace EHentai.uwp.Common
{
    public class AppSettings : NotifyPropertyChanged
    {
        public static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        /// <summary>
        /// 文件下载路径
        /// </summary>
        public static string DownPath
        {
            get
            {
                return ReadSettings(nameof(DownPath), "");
            }
            set
            {
                SaveSettings(nameof(DownPath), value);
                //OnPropertyChanged();
            }
        }

        private static void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }
        private static T ReadSettings<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            if (null != defaultValue)
            {
                return defaultValue;
            }
            return default(T);
        }
    }
}
