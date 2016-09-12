using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EHentai.uwp.Common;

namespace EHentai.uwp.Model
{
    public class FilterModel
    {
        public int f_doujinshi { get; set; }
        public int f_manga { get; set; }
        public int f_artistcg { get; set; }
        public int f_gamecg { get; set; }
        public int f_western { get; set; }
        public int f_non_h { get; set; }
        public int f_imageset { get; set; }
        public int f_cosplay { get; set; }
        public int f_asianporn { get; set; }
        public int f_misc { get; set; }
        public string f_search { get; set; }
        public string f_apply { get; set; }

        public FilterModel()
        {
            f_doujinshi = f_manga = f_artistcg = f_gamecg = f_western = f_non_h = f_imageset = f_cosplay = f_asianporn = f_misc = 1;
            f_apply = "Apply Filter";
        }

        public string ParamToString()
        {
            string result = $"f_doujinshi={f_doujinshi}&f_manga={f_manga}&f_artistcg={f_artistcg}&f_gamecg={f_gamecg}&f_western={f_western}&f_non-h={f_non_h}&f_imageset={f_imageset}&f_cosplay={f_cosplay}&f_asianporn={f_asianporn}&f_misc={f_misc}&f_search={f_search}&f_apply={f_apply}";
            return result;
        }

        public void SetParam(string param)
        {
            param = param.ToLower() + "&";
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo item in properties)
            {
                string name = item.Name;

                if (item.PropertyType == typeof(int))
                {
                    string value = param.GetValue(name + "=", "&");
                    item.SetValue(this, (value == "on" || value == "1") ? 1 : 0);
                }
                else if (item.PropertyType == typeof(string))
                {
                    item.SetValue(this, param.GetValue(name + "=", "&"));
                }
            }
        }
    }
}
