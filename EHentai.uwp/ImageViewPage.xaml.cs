using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using EHentai.uwp.Common;
using Uwp.Common;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EHentai.uwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ImageViewPage : Page
    {
        private static string imgBase64;
        private static double _scale;
        private static double _width;
        private static double _height;

        public static void Create(string url, double width, double height)
        {
            imgBase64 = ImageCache.GetImageBase64(url);

            _scale = width / height;
            _height = (ScreenResolution.Height - 40) / 2.0;
            _width = _height * _scale;

            _height -= 30;
            CreateView();
        }

        public static async void Create(StorageFile file)
        {
            Guid encoderId;
            switch (file.FileType)
            {
                case ".jpg":
                    encoderId = BitmapDecoder.JpegDecoderId;
                    break;
                case ".png":
                    encoderId = BitmapDecoder.PngDecoderId;
                    break;
                case ".bmp":
                    encoderId = BitmapDecoder.BmpDecoderId;
                    break;
                case ".gif":
                    encoderId = BitmapDecoder.GifDecoderId;
                    break;
            }
            using (var stream = await file.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(encoderId, stream);
                _scale = decoder.PixelWidth / (double)decoder.PixelHeight;

                using (var imgStream = stream.AsStream())
                {
                    imgBase64 = ImageCache.GetImageBase64(imgStream, file.FileType);
                }

            }

            _height = (ScreenResolution.Height - 40) / 2.0;
            _width = _height * _scale;

            CreateView();
        }

        private static async void CreateView()
        {
            var newCoreAppView = CoreApplication.CreateNewView();
            var appView = ApplicationView.GetForCurrentView();
            await newCoreAppView.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
            {
                var window = Window.Current;
                var newAppView = ApplicationView.GetForCurrentView();

                var frame = new Frame();
                window.Content = frame;

                frame.Navigate(typeof(ImageViewPage));
                window.Activate();
                await
                    ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppView.Id, ViewSizePreference.Default, appView.Id,
                        ViewSizePreference.Default);
            });
        }

        public ImageViewPage()
        {
            try
            {
                var titleBar = CoreApplication.GetCurrentView().TitleBar;
                if (!titleBar.ExtendViewIntoTitleBar)
                {
                    titleBar.ExtendViewIntoTitleBar = true;//隐藏标题栏
                }
                var appView = ApplicationView.GetForCurrentView();
                appView.TitleBar.BackgroundColor = appView.TitleBar.ButtonBackgroundColor = appView.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                //appView.TitleBar.ButtonHoverBackgroundColor = Colors.Transparent;

                appView.SetPreferredMinSize(new Size(198, 48));
                
                this.InitializeComponent();
                Loaded += ImageViewPage_OnLoaded;

            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 修改窗体大小(需要暂停1毫秒,不然可能修改失败)
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetSize()
        {
            await Task.Delay(10);
            return ApplicationView.GetForCurrentView().TryResizeView(new Size(_width, _height));
        }

        private async void ImageViewPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                while (!await SetSize())
                { }
                string imgStr = $"document.body.innerHTML=\"<img style='width: 100%;' ondragstart='return false' src='{imgBase64}'/> \"";
                imgStr = await ImageView.InvokeScriptAsync("eval", new[] { imgStr });

            }
            catch (Exception ex)
            { }
        }


    }
}
