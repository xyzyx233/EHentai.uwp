using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentai.uwp.Model
{
    public class TagModel
    {
        public string TagName { get; set; }
        public Dictionary<string, string> TagValues { get; } = new Dictionary<string, string>();
    }
}
