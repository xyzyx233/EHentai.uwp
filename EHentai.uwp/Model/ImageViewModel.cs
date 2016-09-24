using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uwp.Common;

namespace EHentai.uwp.Model
{
    public class ImageViewModel
    {
        /// <summary>
        /// 图片Base64字符串
        /// </summary>
        public string ImgBase64 { get; set; }
        /// <summary>
        /// 图标宽高比
        /// </summary>
        public double Scale => Height == 0 ? 0 : Width / Height;

        /// <summary>
        /// 图片宽度
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 图片高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 视图宽度
        /// </summary>
        public double ViewWidth => ViewHeight * Scale;

        /// <summary>
        /// 视图高度
        /// </summary>
        public double ViewHeight => (ScreenResolution.Height - 40) / 2.0;
    }
}
