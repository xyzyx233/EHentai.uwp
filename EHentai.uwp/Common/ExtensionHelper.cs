using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EHentai.uwp.Common
{
    public static class ExtensionHelper
    {
        /// <summary>
        /// 获得字符串中开始和结束字符串中间得值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="star">开始</param>
        /// <param name="end">结束</param>
        /// <returns></returns> 
        public static string GetValue(this string str, string star, string end)
        {
            Regex rg = new Regex("(?<=(" + Regex.Escape(star) + "))[.\\s\\S]*?(?=(" + Regex.Escape(end) + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(str).Value;
        }

        /// <summary>
        /// 字符串转int类型(如果为空或空字符串默认返回0)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            return string.IsNullOrEmpty(str) ? 0 : int.Parse(str);
        }

        public static T ToEntity<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string ToJsonString(this object value)
        {
            string json = JsonConvert.SerializeObject(value);
            return string.IsNullOrEmpty(json) ? "null" : json;
        }

       
    }
}
