using System.Threading;
using EHentai.uwp.Model;
using System;
using System.Collections.ObjectModel;
using EHentai.uwp.Extend;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using EHentai.uwp.Common;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EHentai.uwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : ImagePage
    {
        private CancellationTokenSource cancel;
        private FilterModel filter = new FilterModel();


        public HomePage(string url = null) : base(Site.HomeUrl)
        {
            if (url != null)
            {
                string[] param = url.Split('/');
                foreach (string item in param)
                {
                    if (item.Contains(PageName + "="))
                    {
                        NowPageIndex = (url + "&").GetValue("page=", "&").ToInt();
                        filter.SetParam(item);

                        UrlParam = filter.ParamToString();
                        break;
                    }
                }
            }
            InitializeComponent();


            //Uri baseUri = new Uri("ms-appx-web:///Html/HomePage.html");

            //HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://exhentai.org/"));
            //httpRequestMessage.Headers.Add("Cookie", Uwp.Http.Http.Cookie);
            //HomeView.NavigateWithHttpRequestMessage(httpRequestMessage);
            //HomeView.Navigate(baseUri);

            //string filename = "httpsexhentai.orgg9747752c52333376.jpg";
            //string img = $"<img src='{ImageCache.GetImageBase64(filename)}'/>";
            //HomeView.NavigateToString(img);
        }


        /// <summary>
        /// 加载数据
        /// </summary>
        public override void Load()
        {
            if (cancel != null)
                cancel.Cancel();
            cancel = new CancellationTokenSource();
            UrlParam = filter.ParamToString();
            LoadDataByPage();
        }

        public override void LoadDataByPage()
        {
            CreateTask(() =>
            {
                try
                {
                    var isCancel = cancel;

                    if (IsFirst)
                    {
                        //ImageGrid.ShowLoading();
                        //IsFirst = false;
                    }

                    if (!isCancel.IsCancellationRequested)
                    {
                        LoadNowPageData();

                        CreateTask(() =>
                        {
                            if (NowPageIndex == 1)
                            {
                                LoadNowPageData();
                            }
                        }, isCancel.Token);
                    }
                }
                catch (Exception ex)
                {
                    IsLoadNextPage = false;
                    //ImageGrid.HideLoading();
                    ShowMessage(ex.Message);
                }
            }, cancel.Token);
        }

        /// <summary>
        /// 获取当前页的数据
        /// </summary>
        /// <returns></returns>
        private async void LoadNowPageData()
        {
            try
            {
                //获取html对象
                //var document = GetHtmlDocument(Http.GetIndexPage(filter.PramaToString()));
                var document = GetHtml();

                var nodes = document.DocumentNode;

                if (ImageCount == 0)
                {
                    var countStr = nodes.SelectSingleNode("//*[@class=\"ip\"]").InnerHtml.Split(' ');
                    //var countStr = document.FindFirst("p[class=\"ip\"]").InnerHtml().Split(' ');
                    ImageCount = int.Parse(countStr[countStr.Length - 1].Replace(",", ""));

                    string pageMax = nodes.SelectNodes("//*[@class=\"ptt\"]/tr/td").Last().PreviousSibling.FirstChild.InnerHtml;
                    PageMax = pageMax.ToInt() - 1;
                }

                //获取所需要的元素
                //var div = document.Find("div[class=\"id3\"]").ToList();

                var divs = nodes.SelectNodes("//*[@class=\"id3\"]");

                //获取当前页的数据
                //var nowPageData = new ObservableCollection<ImageListModel>(div.Select(x => new ImageListModel
                //{
                //    Title = x.PreviousElement().FindFirst("a").InnerHtml(),
                //    Image = null,
                //    ImageUrl = x.FindFirst("img").Attribute("src").Value(),
                //    Herf = x.FindFirst("a").Attribute("href").Value(),
                //    CacheName = CommonHepler.GetValidFileName(x.FindFirst("a").Attribute("href").Value()) + ".jpg"
                //}).Where(x => ImageList.All(y => y.ImageUrl != x.ImageUrl)).ToList());
                var nowPageData = new ObservableCollection<ImageListModel>();
                foreach (var div in divs)
                {
                    ImageListModel model = new ImageListModel();

                    model.GetImageUrl += ModelGetImageUrl;
                    var a = div.FirstChild;
                    if (ImageList.All(x => x.ImageUrl != model.ImageUrl))
                    {
                        var img = a.FirstChild;
                        model.Title = img.Attributes["title"].Value;
                        model.Herf = a.Attributes["href"].Value;
                        model.CacheName = model.Herf.GetValidFileName() + ".jpg";
                        model.ImageUrl = img.Attributes["src"].Value;
                        //nowPageData.Add(model);
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            try
                            {
                                ImageList.Add(model);

                                string js = model.ToJsonString();
                                await HomeView.InvokeScriptAsync("AddImages", new[] { js });
                            }
                            catch (Exception ex)
                            { }
                        });
                        if (IsFirst)
                        {
                            IsLoadNextPage = IsFirst = false;
                        }
                        else if (IsLoadNextPage)
                        {
                            IsLoadNextPage = false;
                        }
                    }
                }


                //ImageList.AddRange(nowPageData);
                //Dispatcher.Invoke(() => { ImageList.AddRange(nowPageData); });

                NowImageCount = ImageList.Count;
                if (NowPageIndex < PageMax)
                {
                    NowPageIndex++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ModelGetImageUrl(object sender, EventArgs e)
        {
            var model = sender as ImageListModel;
            GetImageAsync(model);
        }


        //private void Search()
        //{
        //    filter.f_search = TxtFiler.Text;
        //    ImageList.Clear();
        //    filter.page = PageIndex = 0;
        //    ImageBoxScroll.ScrollToTop();
        //    Load();
        //}


        //private void SearchViewbox_OnMouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    //Load();
        //}

        //private void SearchViewbox_OnMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        Search();
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowMessage(ex.Message);
        //    }
        //}

        //private void TxtFiler_OnKeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        Search();
        //    }
        //}

        //private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        ImageListModel data = (sender as Image).Tag as ImageListModel;

        //        Main.MainModel.TabContents.Add(new TabContent(data.Title, new Detail(data.Herf)));
        //        Main.tabMain.SelectedIndex = Main.tabMain.Items.Count - 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowMessage(ex.Message);
        //    }
        //}

        ///// <summary>
        ///// 当加载的图片不完整是点击右键重新加载图片
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private async void UIElement_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        ImageListModel data = (sender as Image).Tag as ImageListModel;
        //        data.ImageIsLoaded = false;
        //        await ImageCache.ClearCache(data.CacheName);
        //        GetImageAsync(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowMessage(ex.Message);
        //    }
        //}

        ///// <summary>
        ///// 当图片没有加载成功是点击重新加载
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        ImageListModel data = (sender as Grid).Tag as ImageListModel;

        //        GetImageAsync(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowMessage(ex.Message);
        //    }
        //}
        private void HomeView_OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
    }
}
