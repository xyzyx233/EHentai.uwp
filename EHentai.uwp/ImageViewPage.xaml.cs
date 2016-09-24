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
using EHentai.uwp.Model;
using Uwp.Common;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EHentai.uwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ImageViewPage : Page
    {
        private ImageViewModel _viewModel;
        public static void Create(ImageViewModel viewModel)
        {
            CreateView(viewModel);
        }

        public static async void Create(StorageFile file)
        {
            try
            {
                var viewModel = new ImageViewModel();
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


                    viewModel.Width = decoder.PixelWidth;
                    viewModel.Height = decoder.PixelHeight;

                    using (var imgStream = stream.AsStream())
                    {
                        viewModel.ImgBase64 = ImageCache.GetImageBase64(imgStream, file.FileType);
                    }

                }

                CreateView(viewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async void CreateView(ImageViewModel viewModel)
        {
            var newCoreAppView = CoreApplication.CreateNewView();
            var appView = ApplicationView.GetForCurrentView();
            await newCoreAppView.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
            {
                var window = Window.Current;
                var newAppView = ApplicationView.GetForCurrentView();

                var frame = new Frame();
                window.Content = frame;


                frame.Navigate(typeof(ImageViewPage), viewModel);
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
                var appView = ApplicationView.GetForCurrentView();
                appView.SetPreferredMinSize(new Size(198, 48));
                appView.TitleBar.BackgroundColor = appView.TitleBar.ButtonBackgroundColor = appView.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                //appView.TitleBar.ButtonHoverBackgroundColor = Colors.Transparent;
                var titleBar = CoreApplication.GetCurrentView().TitleBar;
                if (!titleBar.ExtendViewIntoTitleBar)
                {
                    titleBar.ExtendViewIntoTitleBar = true;//隐藏标题栏
                }

                this.InitializeComponent();
                Loaded += ImageViewPage_OnLoaded;

            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 修改窗体大小
        /// </summary>
        /// <returns></returns>
        public bool SetSize(double width, double height)
        {
            return ApplicationView.GetForCurrentView().TryResizeView(new Size(width, height - 32));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel = e.Parameter as ImageViewModel;
        }

        private async void ImageViewPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {

                string imgStr = $"document.body.innerHTML=\"<img style='width: 100%;' ondragstart='return false' src='{_viewModel.ImgBase64}'/> \"";
                imgStr = await ImageView.InvokeScriptAsync("eval", new[] { imgStr });

                while (!SetSize(_viewModel.ViewWidth, _viewModel.ViewHeight))
                { }
            }
            catch (Exception ex)
            { }
        }


    }
}
