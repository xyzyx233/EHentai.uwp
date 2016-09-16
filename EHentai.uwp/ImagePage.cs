using EHentai.uwp.Common;
using EHentai.uwp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using HtmlAgilityPack;
using Uwp.Common.Extend;

namespace EHentai.uwp
{
    public abstract class ImagePage : BasePage
    {
        #region 属性
        public readonly object _imageListLock = new object();

        public int ItemIndex { get; set; }
        public WebView View { get; set; }//多线程管理
        public List<Task> Tasks { get; set; }//多线程管理
        public ObservableCollection<ImageListModel> ImageList { get; set; } //当前页面数据源
        public bool IsFirst { get; set; }//是否第一次加载
        public bool IsLoadNextPage { get; set; } //是否正在加载下一页
        public bool IsLoadPrePage { get; set; }//是否正在加载上一页
        public int ImageCount { get; set; }//图片总数量
        public int NowImageCount { get; set; }//当前已加载的图片数量
        public int LoadSize { get; set; } //滚动加载的距离

        private int _nowPageIndex;//当前页数
        public int NowPageIndex
        {
            get { return _nowPageIndex; }
            set { _nowPageIndex = value < 1 ? 1 : value; }
        }
        public int PrestrainSize { get; set; }//预加载页数

        private int _prePrevPageIndex;//预加载最前面的页数
        public int PrePrevPageIndex
        {
            get
            {
                var prev = ImageList.LastOrDefault(x => x.Order == EnumOrder.Prev && x.ImageLoadState == EnumLoadState.NotLoaded);
                return prev == null ? 1 : prev.Id;
                //return _prePrevPageIndex;
            }
            //set { _prePrevPageIndex = value < 1 ? 1 : value; }
        }

        private int _preNextPageIndex;//预加载最后面的页数
        public int PreNextPageIndex
        {
            get
            {
                var next = ImageList.FirstOrDefault(x => x.Order == EnumOrder.Next && x.ImageLoadState == EnumLoadState.NotLoaded);
                return next == null ? ImageList.Count : next.Id;
                //return _preNextPageIndex;
            }
            //set { _preNextPageIndex = value > PageMax ? PageMax : value; }
        }

        private string _pageName;//分页参数名
        public string PageName
        {
            get { return _pageName; }
        }
        public int PageMax { get; set; }//最大页数
        public string UrlParam { get; set; }//url参数

        private readonly string _nowUrl;//当前页面网址
        public string NowUrl
        {
            get { return _nowUrl; }
        }
        #endregion

        protected ImagePage(string url, string pageName = "page")
        {
            var param = url.Split('/');
            foreach (string item in param)
            {
                _nowUrl += item + "/";
                if (!string.IsNullOrEmpty(pageName) && item.Contains(pageName + "="))
                {
                    NowPageIndex = (url + "&").GetValue("page=", "&").ToInt();
                    break;
                }
            }

            //初始化属性
            _nowUrl = _nowUrl.TrimEnd('/');
            LoadSize = 700;
            _pageName = pageName;
            IsFirst = true;
            Tasks = new List<Task>();
            ImageList = new ObservableCollection<ImageListModel>();
            View = new WebView(WebViewExecutionMode.SameThread);

            //// 开启集合的异步访问支持
            //BindingOperations.EnableCollectionSynchronization(ImageList, _imageListLock);


            //初始化事件
            Loaded += ImagePage_Loaded;
            Unloaded += ImagePage_Unloaded;
        }

        protected ImagePage()
        {
            Loaded += ImagePage_Loaded; ;
            Unloaded += ImagePage_Unloaded; ;
        }

        public virtual void Load()
        {
            LoadDataByPage();
        }

        public abstract void LoadDataByPage();

        public virtual string GetNowPageUrl()
        {
            return NowUrl + "/?" + PageName + "=" + (NowPageIndex - 1) + "&" + UrlParam;
        }

        public virtual string GetPageUrl(int pageIndex)
        {
            return NowUrl + "/?" + PageName + "=" + pageIndex + "&" + UrlParam;
        }

        public HtmlDocument GetHtmlDocument(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }

        /// <summary>
        /// 获取当前url下的html对象
        /// </summary>
        /// <returns></returns>
        public HtmlDocument GetHtml()
        {
            return GetHtmlDocument(Site.GetStringAsync(GetNowPageUrl()).Result);
        }

        public async void GetImage(ImageListModel item, CancellationTokenSource isCancel = null)
        {
            try
            {
                item.ImageLoadState = EnumLoadState.Loading;
                if (!string.IsNullOrEmpty(item.CacheName))
                {
                    if (!await ImageCache.HasCache(item.CacheName))
                    {
                        var imgStream = await Site.DownloadImage(item.ImageUrl);
                        if (imgStream != null)
                        {
                            await ImageCache.CreateCache(item.CacheName, imgStream);
                        }
                    }


                    item.Src = ImageCache.GetImageBase64(item.CacheName);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        try
                        {
                            BitmapImage img = await ImageCache.HasCache(item.CacheName) ? ImageCache.GetImage(item.CacheName) : ImageCache.ErrorImage;

                            if (img == null)
                            {
                                item.Image = ImageCache.ErrorImage;
                            }

                            item.Image = img;
                            item.ImageLoadState = EnumLoadState.Loaded;
                            item.OnLoaded();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    });
                }

            }
            catch (Exception ex)
            { }
        }

        public async void GetImageBase64(ImageListModel item, CancellationTokenSource isCancel = null)
        {
            try
            {
                item.ImageLoadState = EnumLoadState.Loading;
                if (!string.IsNullOrEmpty(item.CacheName))
                {
                    if (!await ImageCache.HasCache(item.CacheName))
                    {
                        var imgStream = await Site.DownloadImage(item.ImageUrl);
                        if (imgStream != null)
                        {
                            await ImageCache.CreateCache(item.CacheName, imgStream);
                        }
                    }


                    item.Src = ImageCache.GetImageBase64(item.CacheName);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        try
                        {
                            item.ImageLoadState = EnumLoadState.Loaded;
                            item.OnLoaded();

                            await View.InvokeScriptAsync("SetSrc", new[] { item.ToJsonString() });
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    });
                }

            }
            catch (Exception ex)
            { }
        }

        public void GetImageAsync(ImageListModel item, CancellationTokenSource isCancel = null)
        {
            CreateTask(() =>
            {
                GetImage(item, isCancel);
            });
        }

        public void GetImageBase64Async(ImageListModel item, CancellationTokenSource isCancel = null)
        {
            CreateTask(() =>
            {
                GetImageBase64(item, isCancel);
            });
        }

        /// <summary>
        /// 是否加载完当前图册的所有图片
        /// </summary>
        /// <returns></returns>
        public bool IsLoadedImage()
        {
            return ImageCount <= NowImageCount;
        }

        /// <summary>
        /// 滚动翻页
        /// </summary>
        public virtual async void Sorcll()
        {
            if (View != null)
            {
                string js = @"window.onscroll = function() { var scrollTop = $(window).scrollTop(); var contentHeight = $('#divImageList').height(); var windowHeight = $(window).height(); if (scrollTop + windowHeight > contentHeight - " + LoadSize + ") { var data = { method: 'Scroll', data: scrollTop }; window.external.notify(JSON.stringify(data)); } };";
                await View.InvokeScriptAsync("eval", new[] { js });
                View.ScriptNotify += View_ScriptNotify;
            }
        }

        private void ImagePage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ImageList = null;
        }

        private void ImagePage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (IsFirst)
            {
                ListBox listBox = this.GetChildControl<ListBox>("ImageListBox");
                if (listBox!=null)
                {
                    //设置List的数据源
                    listBox.ItemsSource = ImageList;
                }
                Sorcll();
                Load();
            }
        }

        private void View_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (e.Value.Contains("Scroll") && !IsLoadNextPage)
            {
                IsLoadNextPage = true;
                LoadDataByPage();
            }
        }
    }
}
