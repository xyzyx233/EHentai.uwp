using System.Text.RegularExpressions;

namespace EHentai.uwp.Extend
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
    }
}
