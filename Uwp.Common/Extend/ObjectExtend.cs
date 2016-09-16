using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Uwp.Common.Extend
{
    public static class ObjectExtend
    {
        public static string ToJsonString(this object value)
        {
            string json = JsonConvert.SerializeObject(value);
            return string.IsNullOrEmpty(json) ? "null" : json;
        }
    }
}
