﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EHentai.uwp.Common;
using Uwp.Http;
using EHentai.uwp.Extend;
using EHentai.uwp.Model;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace EHentai.uwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : BasePage
    {
        public static MainPage Main;
        public static Pivot MainPivot;

        //public static ObservableCollection<PivotItem> listItems = new ObservableCollection<PivotItem>();

        public MainPage()
        {
            InitializeComponent();

            Main = this;
            MainPivot = new Pivot();
            MainGrid.Children.Add(MainPivot);

            CreateHomePage();

            //if (Site.IsLogin)
            //{
            //    CreateHomePage();
            //}
            //else
            //{
            //    Site.OnLogined += User_OnLogined;
            //    Site.Login();
            //}

        }

        private void User_OnLogined(object sender, EventArgs e)
        {
            CreateHomePage();
        }

        public void CreateHomePage()
        {
            Add("Home", new HomePage());
        }

        public void Add(string title, Page page)
        {
            PivotItem item = new PivotItem();
            item.Margin = new Thickness(0);
            item.Header = title;
            item.Content = page;

            MainPivot.Add(item);
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
    }
}