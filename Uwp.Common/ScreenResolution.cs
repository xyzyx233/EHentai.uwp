using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Graphics.Display;

namespace Uwp.Common
{
    public static class ScreenResolution
    {
        /// <summary>
        /// 获取屏幕高度。
        /// </summary>
        public static double Height
        {
            get
            {
                var rect = PointerDevice.GetPointerDevices().Last().ScreenRect;
                //var scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                return rect.Height;
            }
        }

        /// <summary>
        /// 获取屏幕宽度。
        /// </summary>
        public static double Width
        {
            get
            {
                var rect = PointerDevice.GetPointerDevices().Last().ScreenRect;
                //var scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                return rect.Width;
            }
        }
    }
}
