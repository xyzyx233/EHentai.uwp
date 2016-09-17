using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentai.uwp.Model
{
    public class Torrent
    {
        public string Name { get; set; }//文件名称
        public string DownUrl { get; set; }//下载地址
        public string Posted { get; set; }//上传日期
        public string Size { get; set; }//文件大小
        public string Seeds { get; set; }//种子数
        public string Peers { get; set; }
        public string Downloads { get; set; }//下载量
        public string Uploader { get; set; }//上传人
    }
}
