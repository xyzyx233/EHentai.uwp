using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private static string _url;
        private static double _scale;
        private static double _width;
        private static double _height;

        public static async void Create(string url, double width, double height)
        {
            _url = url;
            _scale = width / height;

            _height = ScreenResolution.Height / 2.0;
            _width = _height * _scale;

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
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppView.Id, ViewSizePreference.Default, appView.Id, ViewSizePreference.Default);

            });
        }

        public ImageViewPage()
        {
            try
            {
                this.InitializeComponent();
                Loaded += ImageViewPage_OnLoaded;

            }
            catch (Exception ex)
            { }
        }

        private async void ImageViewPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var appView = ApplicationView.GetForCurrentView();
                appView.TitleBar.BackgroundColor = appView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                appView.SetPreferredMinSize(new Size(198, 48));
                while (!appView.TryResizeView(new Size(_width, _height)))
                {

                }


                string imgStr = "document.write(\"<img style='width: 100%;' src='" + ImageCache.GetImageBase64(_url) + "'/> \")";
                string a = await ImageView.InvokeScriptAsync("eval", new[] { imgStr });
            }
            catch (Exception ex)
            { }
        }
    }
}
