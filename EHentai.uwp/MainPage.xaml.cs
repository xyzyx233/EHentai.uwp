using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EHentai.uwp.Common;
using Uwp.Common;
using Uwp.Common.Extend;
using Uwp.Http;
using Uwp.Control;
using Uwp.Control.Model;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace EHentai.uwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : BasePage
    {
        public static Button HideButton = new Button();

        public MainPage()
        {
            var appView = ApplicationView.GetForCurrentView();
            appView.TryResizeView(new Size(ScreenResolution.Width * 0.85, ScreenResolution.Height * 0.85));

            InitializeComponent();

            Main = this;
            PivotView = new PivotViewModel();

            HideButton = hideButton;

            DataContext = PivotView;

            if (Site.IsLogin)
            {
                PivotView.Add("主页", new HomePage());
            }
            //else
            //{
            //    Site.OnLogined += User_OnLogined;
            //    Site.Login();
            //}


        }

        private void User_OnLogined(object sender, EventArgs e)
        {
            //PivotView.Add("主页", new HomePage());
        }

        private async void MainPage_OnDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                var id = await e.DataView.GetTextAsync();
                var itemIdsToMove = id.Split(',');


            }
        }

        private void MainPage_OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            PivotView.SelectedIndex = 0;
        }
    }
}
