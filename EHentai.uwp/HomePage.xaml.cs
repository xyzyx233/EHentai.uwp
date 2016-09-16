using System.Threading;
using EHentai.uwp.Model;
using System;
using System.Collections.ObjectModel;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Uwp.Common.Extend;

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

            MainGrid.Children.Add(View);
            View.Navigate(new Uri("ms-appx-web:///Html/HomePage.html"));
            View.ScriptNotify += View_ScriptNotify;
        }

        private void View_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value))
            {
                JObject jsonBody = JObject.Parse(e.Value);
                string method = jsonBody["method"].ToString();
                string data = jsonBody["data"].ToString();
                switch (method)
                {
                    case "ToDetailPage":
                        ImageListModel model = data.ToEntity<ImageListModel>();
                        PivotView.AddSelect(model.Title, new DetailPage(model.Herf));
                        break;
                    case "Search":
                        Search(data);
                        break;
                }
            }
        }

        public override async void LoadDataByPage()
        {
            cancel?.Cancel();
            cancel = new CancellationTokenSource();
            UrlParam = filter.ParamToString();

            try
            {
                //获取当前页的数据
                var datas = await GetNowPageData();
                if (datas.Any())
                {
                    //将当前页数据转为json格式的字符串
                    string js = datas.ToJsonString();
                    //将数据添加到前台页面
                    await View.InvokeScriptAsync("AddImages", new[] { js });
                }

            }
            catch (Exception ex)
            {
                IsLoadNextPage = false;
                ShowMessage(ex.Message);
            }
        }

        /// <summary>
        /// 获取当前页的数据
        /// </summary>
        /// <returns></returns>
        private async Task<ObservableCollection<ImageListModel>> GetNowPageData()
        {
            return await Task.Run(() =>
            {
                ObservableCollection<ImageListModel> datas = new ObservableCollection<ImageListModel>();
                try
                {
                    //获取html对象
                    var document = GetHtml();

                    var nodes = document.DocumentNode;

                    #region 获取记录
                    if (ImageCount == 0)
                    {
                        //总记录数量
                        var countStr = nodes.SelectSingleNode("//*[@class=\"ip\"]").InnerHtml.Split(' ');
                        ImageCount = int.Parse(countStr[countStr.Length - 1].Replace(",", ""));

                        //总页数
                        string pageMax = nodes.SelectNodes("//*[@class=\"ptt\"]/tr/td").Last().PreviousSibling.FirstChild.InnerHtml;
                        PageMax = pageMax.ToInt() - 1;
                    }
                    #endregion

                    var divs = nodes.SelectNodes("//*[@class=\"id3\"]");

                    foreach (var div in divs)
                    {
                        ImageListModel model = new ImageListModel();

                        //model.GetImageUrl += ModelGetImageUrl;
                        var a = div.FirstChild;

                        var img = a.FirstChild;
                        model.Title = img.Attributes["title"].Value;
                        model.Herf = a.Attributes["href"].Value;
                        model.CacheName = model.Herf.GetValidFileName() + ".jpg";
                        model.ImageUrl = img.Attributes["src"].Value;
                        datas.Add(model);
                        if (IsFirst)
                        {
                            IsLoadNextPage = IsFirst = false;
                        }
                        else if (IsLoadNextPage)
                        {
                            IsLoadNextPage = false;
                        }
                    }

                    NowImageCount = ImageList.Count;
                    if (NowPageIndex < PageMax)
                    {
                        NowPageIndex++;
                    }

                    return datas;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

        }

        private void ModelGetImageUrl(object sender, EventArgs e)
        {
            var model = sender as ImageListModel;
            GetImageBase64Async(model);
        }


        private async void Search(string filer)
        {
            try
            {
                filter.f_search = filer;
                ImageList.Clear();
                NowPageIndex = 0;

                //将当前页数据转为json格式的字符串
                string js = "reload();";
                //将数据添加到前台页面
                await View.InvokeScriptAsync("eval", new[] { js });
                //View.Refresh();
                LoadDataByPage();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }


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

        private void HomePage_OnLoaded(object sender, RoutedEventArgs e)
        {
            //View.ScriptNotify += View_ScriptNotify;
        }
    }
}
