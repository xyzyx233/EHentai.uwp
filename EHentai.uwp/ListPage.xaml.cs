using EHentai.uwp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Input;
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
using EHentai.uwp.Common;
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
        private string detailtUrl = "";
        private Dictionary<int, string> historyDetailPageUrl = new Dictionary<int, string>();

        public ListPage(int index, string detailtUrl, int count) : base(detailtUrl)
        {
            this.detailtUrl = detailtUrl;
            NowPageIndex = index;
            ImageCount = count;
            PrestrainSize = 5;
            InitializeComponent();
            LoadSize = 1000;
            this.InitializeComponent();
        }

        public override async void LoadDataByPage()
        {
            //初始化所有实体数据
            InitializeData();

            int detailtPage = GeDetailtPageIndex(NowPageIndex);

            if (await GetHref(detailtPage))
            {
                await LoadImage(NowPageIndex);
            }
            IsFirst = false;


            //如果是第一页则只加载后面的
            if (NowPageIndex < PageMax)
            {
                for (int i = 1; i <= PrestrainSize; i++)
                {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                    LoadImage(NowPageIndex + i);
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                }
            }
        }

        private void InitializeData()
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
                    //Index = i.ToString(),
                    Order = order,
                };
                //model.Loaded += ModelLoaded;
                ImageList.Add(model);
            }
        }

        /// <summary>
        /// 获取当前显示图片的坐标
        /// </summary>
        /// <returns></returns>
        private int GetIndex()
        {
            int index = 0;
            //return nowShowIndex = (int)ImageScroll.VerticalOffset - 1;
            double dVer = ImageScroll.VerticalOffset;
            if (ImageScroll.VerticalOffset == 0)
            {
                index = ImageList.First(x => x.Order == EnumOrder.Center).Id;
            }
            else
            {
                var loadeds = ImageList.Where(x => x.ImageLoadState == EnumLoadState.Loaded && x.Height != null).ToList();

                if (loadeds.Any())
                {
                    double height = 0;
                    for (int i = 0; i < loadeds.Count; i++)
                    {
                        var nowHeight = loadeds[i].Height ?? 0;
                        if (dVer >= height && dVer < height + nowHeight)
                        {
                            nowShowImageOffset = dVer - height;
                            nowShowImageOffsetScale = nowShowImageOffset / nowHeight;
                            nowShowIndex = loadeds[i].Id;
                            index = nowShowIndex;
                            break;
                        }
                        height += nowHeight;
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// 根据页数算出当前图片在DetailtPage里面是第几页
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GeDetailtPageIndex(int index)
        {
            int page = (index - 1) / 20;
            return page;
        }

        private async Task<bool> GetHref(int page)
        {
            bool result = false;
            try
            {
                string url = detailtUrl + "?p=" + page;
                if (!historyDetailPageUrl.ContainsValue(url))
                {
                    var doc = await GetHtmlDocument(url);
                    historyDetailPageUrl.Add(page, url);
                    var divs = doc.QuerySelectorAll("#gdt .gdtl");
                    foreach (var div in divs)
                    {
                        var a = div.QuerySelector("a");
                        var img = a.QuerySelector("img");

                        string index = img.Attributes["alt"].Value;
                        int id = index.ToInt();
                        var imgModel = ImageList.First(x => x.Id == id);
                        imgModel.Herf = a.Attributes["href"].Value;
                        imgModel.Index = index;
                        imgModel.CacheName = imgModel.Herf.GetValidFileName() + "_Original." +
                                             img.Attributes["title"].Value.Split('.').Last();
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            { }
            return result;
        }

        private async Task<bool> LoadImage(int index)
        {
            bool result = false;
            try
            {
                var nowModel = ImageList[index - 1];//获取当前页的实体
                if (!await ImageCache.HasCache(nowModel.CacheName))
                {
                    if (string.IsNullOrEmpty(nowModel.Herf))
                    {
                        int detailtPage = GeDetailtPageIndex(index);
                        if (!await GetHref(detailtPage))
                        {
                            return result;
                        }
                    }
                    var doc = await GetHtmlDocument(nowModel.Herf);
                    nowModel.ImageUrl = doc.QuerySelector("#img").Attributes["src"].Value;

                }
                GetImageAsync(nowModel);
                result = true;
            }
            catch (Exception ex)
            { }
            return result;
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
            var list = ImageList.Where(x => x.Id >= nowShowIndex && x.Id <= PreNextPageIndex).OrderBy(x => x.Id).ToList();
            ShowLoadedImage(list);
        }

        private bool IsContinueLoad()
        {
            return ImageScroll.VerticalOffset % 50 <= 10;//普通容器时的判断逻辑(每滚动50高度加载数据)
            //return ImageScroll.VerticalOffset % 1 <= 0.2;//虚拟化容器时的判断逻辑(每滚动0.1高度加载数据)
        }

        private double GetImageHeight(int id)
        {
            double countHeight = ImageList.Where(x => x.Id <= id && x.Height != null).Sum(x => x.Height.Value);
            return countHeight;
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

                if (!IsFirst && IsContinueLoad())
                {
                    //int nowIndex = GetIndex();
                    if (ImageList[PrePrevPageIndex - 1].ImageLoadState == EnumLoadState.NotLoaded &&
                        nowShowIndex - PrePrevPageIndex < PrestrainSize)
                    {
                        LoadImage(PrePrevPageIndex);
                    }
                    if (ImageList[PreNextPageIndex - 1].ImageLoadState == EnumLoadState.NotLoaded &&
                        PreNextPageIndex - nowShowIndex < PrestrainSize)
                    {
                        LoadImage(PreNextPageIndex);
                    }
                }
            }
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

                    //int index = GetIndex();
                    //if (model.Id < index)
                    //{
                    //    ImageScroll.ScrollToVerticalOffset(ImageScroll.VerticalOffset + img.ActualHeight);
                    //}
                    model.Height = img.ActualHeight;
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
                if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                {
                    var properties = e.GetCurrentPoint(img).Properties;
                    if (properties.IsLeftButtonPressed)
                    {
                        var bitmap = img.Source as BitmapImage;
                        var urls = bitmap.UriSource.Segments;
                        var viewModel = new ImageViewModel();
                        viewModel.ImgBase64 = ImageCache.GetImageBase64(urls[urls.Length - 1]);
                        viewModel.Width = img.ActualWidth;
                        viewModel.Height = img.ActualHeight;
                        ImageViewPage.Create(viewModel);
                    }
                    else if (properties.IsRightButtonPressed)
                    {
                        var imgModel = img.Tag as ImageListModel;
                        if (imgModel.ImageLoadState == EnumLoadState.Loaded)
                        {
                            imgModel.ImageLoadState = EnumLoadState.NotLoaded;
                            imgModel.IsCance.Cancel();
                            imgModel.ReLoad();
                            GetImageAsync(imgModel);
                        }
                    }
                }

            }
            catch (Exception ex)
            { }
        }

    }
}
