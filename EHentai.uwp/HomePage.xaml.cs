using System.Threading;
using EHentai.uwp.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AngleSharp.Dom.Html;
using HtmlAgilityPack;
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
        private FilterModel filter = new FilterModel();


        public HomePage(string url = null) : base(GetHomeUrl(url))
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

        private static string GetHomeUrl(string url)
        {
            string homeUrl= Site.HomeUrl;
            if (!string.IsNullOrEmpty(url) && url.Contains("/tag/"))
            {
                var urls = url.Split('/');
                homeUrl = $"https://{urls[2]}/{urls[3]}/{urls[4]}";
            }
            return homeUrl;
        }

        public override string GetNowPageUrl()
        {
            if (NowUrl.Contains("/tag/"))
            {
                return NowUrl + "/" + NowPageIndex;
            }
            return base.GetNowPageUrl();
        }

        public override async void LoadDataByPage()
        {
            UrlParam = filter.ParamToString();

            try
            {
                //获取当前页的数据
                var datas = await GetNowPageData();
                ShowData(datas);
            }
            catch (Exception ex)
            {
                IsLoadNextPage = false;
                ShowMessage(ex.Message);
            }
        }

        private async void ShowData(ObservableCollection<ImageListModel> datas)
        {
            try
            {
                IsLoadNextPage = false;

                if (datas.Any())
                {
                    if (NowPageIndex < PageMax)
                    {
                        NowPageIndex++;
                    }

                    //将当前页数据转为json格式的字符串
                    string js = datas.ToJsonString();
                    //将数据添加到前台页面
                    await View.InvokeScriptAsync("AddImages", new[] { js });
                }

                if (IsFirst)
                {
                    IsFirst = false;
                    IsLoadNextPage = true;

                    datas = await GetNowPageData();
                    ShowData(datas);
                }
            }
            catch (Exception ex)
            {
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
                    IHtmlDocument document; //当前页html对象

                    if (IsFirst)
                    {
                        //获取html对象
                        document = GetHtmlDocument().Result;

                        //总记录数量
                        var countStr = document.QuerySelector(".ip").InnerHtml.Split(' ');
                        ImageCount = int.Parse(countStr[countStr.Length - 1].Replace(",", ""));

                        //总页数
                        string pageMax = document.QuerySelectorAll(".ptt tr td").Last().PreviousElementSibling.FirstElementChild.InnerHtml;
                        PageMax = pageMax.ToInt() - 1;

                    }
                    else
                    {
                        if (!IsLoaded)
                            document = GetHtmlDocument().Result;
                        else
                            return datas;
                    }

                    var divs = document.QuerySelectorAll(".id3");

                    if (divs!=null&& divs.Any())
                    {
                        foreach (var div in divs)
                        {
                            ImageListModel model = new ImageListModel();

                            //model.GetImageUrl += ModelGetImageUrl;
                            var a = div.FirstElementChild;

                            var img = a.FirstElementChild;
                            model.Title = div.PreviousElementSibling.FirstElementChild.TextContent;
                            model.Herf = a.Attributes["href"].Value;
                            model.CacheName = model.Herf.GetValidFileName() + ".jpg";
                            model.ImageUrl = img.Attributes["src"].Value;
                            datas.Add(model);
                        }
                    }
                   

                    return datas;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

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
    }
}
