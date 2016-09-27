using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Windows.UI;
using Newtonsoft.Json;

namespace Uwp.Common.Extend
{
    public static class StringExtend
    {
        /// <summary>
        /// 去除特殊符号 获取符合文件命名的字符串
        /// </summary>
        /// <param name="value"></param>
        public static string GetValidFileName(this string value)
        {
            return Regex.Replace(value, @"\\|/|:|\*|\?|""|<|>|\|", "");
        }

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

        /// <summary>
        /// json字符串转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 转为HTML编码字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string value)
        {
            return WebUtility.HtmlEncode(value);
        }

        /// <summary>
        /// 解码HTML编码字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string value)
        {
            return WebUtility.HtmlDecode(value);
        }

        /// <summary>
        /// 将颜色代码转为Color
        /// </summary>
        /// <param name="hexColor">16进制颜色代码</param>
        /// <returns></returns>
        public static Color GetColor(this string hexColor)
        {
            hexColor = hexColor.Replace("#", string.Empty);
            byte r = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);

            return Color.FromArgb(1, r, g, b);
        }
    }
}
