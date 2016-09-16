using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uwp.Common;

namespace Uwp.Control.Model
{
    public class PivotContent : NotifyPropertyChanged
    {
        public int CreateIndex { get; set; }//创建当前页时选中的坐标

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="header">标题</param>
        /// <param name="content">内容</param>
        /// <param name="createIndex">初始化这个TabItem的坐标</param>
        /// <param name="isSelected">是否选择</param>
        public PivotContent(string header, object content, int createIndex = 0, bool isSelected = true)
        {
            _header = header;
            _content = content;
            CreateIndex = createIndex;
        }

        private string _header;
        public string Header
        {
            get { return _header; }
            set
            {
                SetProperty(ref _header, value, "Header");
            }
        }

        private object _content;
        public object Content
        {
            get { return _content; }
            set
            {
                SetProperty(ref _content, value);
            }
        }
    }
}
