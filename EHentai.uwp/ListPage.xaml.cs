﻿using EHentai.uwp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
using Uwp.Common.Extend;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EHentai.uwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ListPage : ImagePage
    {
        public ScrollViewer ImageScroll;
        private int nowShowIndex = 0; //当前显示的图片的坐标
        private double nowOffset = 0; //滚动条当前距离
        private double nowShowImageOffset = 0; //滚动条相对于当前显示图片的滚动距离
        private double nowShowImageOffsetScale = 0; //滚动条相对于当前显示图片的滚动距离与图表高度的比例

        public ListPage(string url) : base(url, null)
        {
            PrestrainSize = 5;
            InitializeComponent();
            LoadSize = 1000;
            this.InitializeComponent();
        }

        public override string GetNowPageUrl()
        {
            return NowUrl;
        }

        public override void LoadDataByPage()
        {
            CreateTask(async () =>
            {
                try
                {
                    if (IsFirst)
                    {
                        var document = GetHtmlNode();
                        
                        PageMax =
                            ImageCount =
                                int.Parse(
                                    document.SelectSingleNode("//*[@id=\"next\"]")
                                        .PreviousSibling.SelectNodes("span")
                                        .Last()
                                        .InnerHtml);
                        nowShowIndex = NowPageIndex = int.Parse(NowUrl.Split('-')[1]); //获取当前页数

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            for (int i = 1; i <= ImageCount; i++)
                            {
                                int id = i;
                                var order = EnumOrder.Next;
                                if (id < NowPageIndex)
                                {
                                    order = EnumOrder.Prev;
                                }
                                else if (id == NowPageIndex)
                                {
                                    order = EnumOrder.Center;
                                }

                                var model = new ImageListModel
                                {
                                    Id = id,
                                    Index = i.ToString(),
                                    Order = order,
                                };
                                model.Loaded += ModelLoaded;
                                ImageList.Add(model);
                            }
                        });




                        //PrePrevPageIndex = NowPageIndex - 1;
                        //PreNextPageIndex = NowPageIndex + 1;

                        IsLoadPrePage = IsLoadNextPage = IsFirst = false;


                        var nowModel = ImageList[NowPageIndex - 1];
                        var elment = document.SelectSingleNode("//*[@id=\"i3\"]/a");
                        //nowModel.Herf = elment.Attribute("href").Value();
                        nowModel.Herf = NowUrl;
                        nowModel.ImageUrl = elment.SelectSingleNode("img").Attributes["src"].Value;
                        nowModel.CacheName = nowModel.Herf.GetValidFileName() + "_Original." +
                                             nowModel.ImageUrl.Split('.').Last();
                        GetImageAsync(nowModel);

                        string next = document.SelectSingleNode("//*[@id=\"next\"]").Attributes["href"].Value;
                        string prev = document.SelectSingleNode("//*[@id=\"prev\"]").Attributes["href"].Value;

                        ImageList[PrePrevPageIndex - 1].Herf = prev;
                        ImageList[PreNextPageIndex - 1].Herf = next;


                        int nextNotLoadCount = PreNextPageIndex - NowPageIndex;
                        int prevNotLoadCount = NowPageIndex - PrePrevPageIndex;
                        //如果是第一页则只加载后面的
                        if (NowPageIndex == 1)
                        {
                            CreateTask(() =>
                            {
                                for (int i = 0; i <= PrestrainSize - nextNotLoadCount; i++)
                                {
                                    LoadNext();
                                }
                            });
                        }
                        //如果是最后一页则只加载前面的
                        else if (NowPageIndex == PageMax)
                        {
                            CreateTask(() =>
                            {
                                for (int i = 0; i <= PrestrainSize - prevNotLoadCount; i++)
                                {
                                    LoadPrev();
                                }
                            });
                        }
                        else
                        {
                            CreateTask(() =>
                            {
                                for (int i = 0; i <= PrestrainSize - nextNotLoadCount; i++)
                                {
                                    LoadNext();
                                }
                            });

                            CreateTask(() =>
                            {
                                for (int i = 0; i < PrestrainSize - prevNotLoadCount; i++)
                                {
                                    LoadPrev();
                                }
                            });
                        }

                        //ImageGrid.HideLoading();
                    }
                }
                catch (Exception ex)
                {
                    //ImageGrid.HideLoading();
                    ShowMessage(ex.Message);
                }
            });
        }

        private void ImageScroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;
            if (ImageScroll == scroll)
            {
                if (nowOffset < ImageScroll.VerticalOffset)
                {
                    ShowNextLoadedImage();
                }
                else
                {
                    ShowPrevLoadedImage();
                }
                nowOffset = ImageScroll.VerticalOffset;
                nowShowIndex = GetIndex();
                //int balance = (int)(ImageScroll.VerticalOffset % height);
                //int balance = (int)nowShowImageOffset;
                //if (balance < 50)
                if (ImageScroll.VerticalOffset % 50 <= 10)
                {
                    //int nowIndex = GetIndex();
                    if (ImageList[PrePrevPageIndex - 1].ImageLoadState == EnumLoadState.NotLoaded &&
                        nowShowIndex - PrePrevPageIndex < PrestrainSize)
                    {
                        CreateTask(LoadPrev);
                    }
                    if (ImageList[PreNextPageIndex - 1].ImageLoadState == EnumLoadState.NotLoaded &&
                        PreNextPageIndex - nowShowIndex < PrestrainSize)
                    {
                        CreateTask(LoadNext);
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前显示图片的坐标
        /// </summary>
        /// <returns></returns>
        private int GetIndex()
        {
            double dVer = ImageScroll.VerticalOffset;

            var loadeds = ImageList.Where(x => x.ImageLoadState == EnumLoadState.Loaded && x.Height != null).ToList();

            if (loadeds.Any())
            {
                double height = 0;
                for (int i = 0; i < loadeds.Count; i++)
                {
                    if (dVer >= height && dVer < height + loadeds[i].Height.Value)
                    {
                        nowShowImageOffset = dVer - height;
                        nowShowImageOffsetScale = nowShowImageOffset / loadeds[i].Height.Value;
                        nowShowIndex = loadeds[i].Id;
                        return nowShowIndex;
                    }
                    height += loadeds[i].Height.Value;
                }
            }

            return 0;
        }

        /// <summary>
        /// 显示加载完成但为显示的图片
        /// </summary>
        /// <param name="list"></param>
        private void ShowLoadedImage(List<ImageListModel> list)
        {
            foreach (ImageListModel model in list)
            {
                if (model.ImageLoadState == EnumLoadState.Loaded)
                {
                    if (model.Visibility != Visibility.Visible)
                    {
                        model.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 加载最上页到当前显示页之间已加载完成但未显示的图片
        /// </summary>
        private void ShowPrevLoadedImage()
        {
            var list =
                ImageList.Where(x => x.Id >= PrePrevPageIndex && x.Id <= nowShowIndex)
                    .OrderByDescending(x => x.Id)
                    .ToList();
            ShowLoadedImage(list);
        }

        /// <summary>
        /// 加载当前显示页到最下页之间已加载完成但未显示的图片
        /// </summary>
        private void ShowNextLoadedImage()
        {
            var list =
                ImageList.Where(x => x.Id >= nowShowIndex && x.Id <= PreNextPageIndex).OrderBy(x => x.Id).ToList();
            ShowLoadedImage(list);
        }

        private void LoadPrev()
        {
            try
            {
                lock (this)
                {
                    ImageListModel nowModel = ImageList[PrePrevPageIndex - 1];

                    if (nowModel.ImageLoadState == EnumLoadState.NotLoaded)
                    {
                        nowModel.ImageLoadState = EnumLoadState.Loading;
                        string html = Http.GetStringAsync(nowModel.Herf).Result;
                        if (!string.IsNullOrEmpty(html))
                        {
                            var document = GetHtmlDocument(html).DocumentNode;
                            var elment = document.SelectSingleNode("//*[@id=\"i3\"]/a");

                            nowModel.ImageUrl = elment.SelectSingleNode("img").Attributes["src"].Value;
                            nowModel.CacheName = nowModel.Herf.GetValidFileName() + "_Original." +
                                                 nowModel.ImageUrl.Split('.').Last();
                            GetImageAsync(nowModel);

                            ImageListModel preModel = ImageList[PrePrevPageIndex - 1];
                            preModel.Herf =
                                document.SelectSingleNode($"//*[@id=\"{nowModel.Order.ToString().ToLower()}\"]")
                                    .Attributes["href"].Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void LoadNext()
        {
            try
            {
                lock (_imageListLock)
                {
                    ImageListModel nowModel = ImageList[PreNextPageIndex - 1];

                    if (nowModel.ImageLoadState == EnumLoadState.NotLoaded)
                    {
                        nowModel.ImageLoadState = EnumLoadState.Loading;
                        string html = Http.GetStringAsync(nowModel.Herf).Result;
                        if (!string.IsNullOrEmpty(html))
                        {
                            var document = GetHtmlDocument(html).DocumentNode;
                            var elment = document.SelectSingleNode("//*[@id=\"i3\"]/a");

                            nowModel.ImageUrl = elment.SelectSingleNode("img").Attributes["src"].Value;
                            nowModel.CacheName = nowModel.Herf.GetValidFileName() + "_Original." +
                                                 nowModel.ImageUrl.Split('.').Last();
                            GetImageAsync(nowModel);

                            ImageListModel preModel = ImageList[PreNextPageIndex - 1];
                            preModel.Herf =
                                document.SelectSingleNode($"//*[@id=\"{nowModel.Order.ToString().ToLower()}\"]")
                                    .Attributes["href"].Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private double GetNowShowImageHeight()
        {
            double countHeight = ImageList.Where(x => x.Id < nowShowIndex && x.Height != null).Sum(x => x.Height.Value);
            return countHeight;
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (ImageScroll == null)
                {
                    ImageScroll = ImageListBox.GetChildControl<ScrollViewer>();
                    ImageScroll.ViewChanged += ImageScroll_ViewChanged;
                }
                else
                {
                    Image img = sender as Image;

                    img.MinHeight = 0;
                    var model = img.Tag as ImageListModel;

                    if (!(model.Height > 0))
                    {
                        model.Height = img.ActualHeight;
                        //if (height == 0)
                        //{
                        //    height = img.ActualHeight;
                        //}
                        if (model.Order == EnumOrder.Prev)
                        {
                            ImageScroll.ScrollToVerticalOffset(GetNowShowImageHeight() +
                                                               nowShowImageOffsetScale * img.ActualHeight);

                            //ImageScroll.ScrollToVerticalOffset(ImageScroll.VerticalOffset + img.ActualHeight);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 图片加载完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModelLoaded(object sender, EventArgs e)
        {
            try
            {
                ImageListModel image = sender as ImageListModel;
                switch (image.Order)
                {
                    //获取最上页到当前显示页之间的图片
                    case EnumOrder.Prev:
                        ShowPrevLoadedImage();
                        break;
                    case EnumOrder.Center:
                        image.Visibility = Visibility.Visible;
                        //ImageGrid.HideLoading();
                        break;
                    //获取当前显示页到最下页之间的图片
                    case EnumOrder.Next:
                        ShowNextLoadedImage();
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void Image_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                Image img = sender as Image;
                var bitmap = img.Source as BitmapImage;
                var urls = bitmap.UriSource.Segments;
                ImageViewPage.Create(urls[urls.Length - 1], img.ActualWidth, img.ActualHeight);
            }
            catch (Exception ex)
            { }
        }
    }
}
