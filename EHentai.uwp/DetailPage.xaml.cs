using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EHentai.uwp.Model;
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
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using EHentai.uwp.Common;
using Newtonsoft.Json.Linq;
using Uwp.Common;
using Uwp.Common.Extend;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EHentai.uwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailPage : ImagePage
    {
        private double _imageHeight = 250;
        private string _enTitle = ""; //英文标题
        private string _jpTitle = ""; //日文标题
        private string _torrentPageUrl = ""; //种子下载页面
        private string _coverUrl = "";//封面地址
        private List<Torrent> _torrents;//种子下载
        private List<TagModel> _tagList = new List<TagModel>();//标签


        public DetailPage(string url) : base(url, "p")
        {
            InitializeComponent();

            MainGrid.Children.Add(View);
            View.Navigate(new Uri("ms-appx-web:///Html/DetailPage.html"));
            View.ScriptNotify += View_ScriptNotify;

        }

        public override async void LoadDataByPage()
        {
            try
            {
                IHtmlDocument document = await GetHtmlDocument();
                //获取当前页的数据
                var datas = await GetNowPageData(document);

                if (IsFirst)
                {
                    InitData(document);
                    if (!string.IsNullOrEmpty(_torrentPageUrl))
                    {
                        try
                        {
                            GetDown();
                        }
                        catch (Exception ex)
                        { }
                    }
                }

                ShowData(datas);
            }
            catch (Exception ex)
            {
                IsLoadNextPage = false;
                ShowMessage(ex.Message);
            }
        }

        /// <summary>
        /// 初始化数据(标题,总页数等其它信息)
        /// </summary>
        /// <param name="document"></param>
        public void InitData(IHtmlDocument document)
        {
            ImageCount =
                          int.Parse(
                              document.QuerySelectorAll(".gdt1")
                                  .First(x => x.InnerHtml == "Length:")
                                  .NextElementSibling.InnerHtml.Split(' ')[0]);
            _imageHeight = int.Parse(document.QuerySelector(".gdtl").Attributes["style"].Value.Split(':')[1].Replace("px", "")) - 20;

            _enTitle = document.QuerySelector("#gn").InnerHtml; //英文标题
            _jpTitle = document.QuerySelector("#gj").InnerHtml; //日文标题
            _coverUrl = document.QuerySelector("#gd1 img").Attributes["src"].Value;//获取封面图片

            var download =
                document.QuerySelectorAll("#gd5 .g2 a")
                    .FirstOrDefault(
                        x =>
                            x.InnerHtml.Contains("Torrent Download") &&
                            !x.InnerHtml.Contains("Torrent Download ( 0 )"));
            if (download != null)
            {
                _torrentPageUrl = download.Attributes["onclick"].Value.Split('\'')[1];
            }

            var trs = document.QuerySelectorAll("#taglist tr");
            foreach (var tr in trs)
            {
                TagModel tag = new TagModel();
                var td = tr.QuerySelectorAll("td");
                tag.TagName = td.First().InnerHtml;
                var divTags = td.Last().QuerySelectorAll("div");
                foreach (var div in divTags)
                {
                    var a = div.QuerySelector("a");
                    tag.TagValues.Add(a.InnerHtml, a.Attributes["href"].Value);
                }
                _tagList.Add(tag);
            }
        }

        private async void GetDown()
        {
            try
            {
                _torrents = await GetTorrent();

                if (_torrents.Any())
                {
                    string js = _torrents.ToJsonString();
                    await View.InvokeScriptAsync("addTorrent", new[] { js });
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        private async void ShowData(ObservableCollection<ImageListModel> datas)
        {
            try
            {
                string js;
                if (IsFirst)
                {
                    //设置标题
                    js = $"scope.enTitle='{_enTitle.Replace("'", "\\'").HtmlDecode()}'; scope.jpTitle='{_jpTitle.Replace("'", "\\'").HtmlDecode()}'; scope.coverUrl='{_coverUrl}'; scope.tagList ={_tagList.ToJsonString()}; scope.$apply();";
                    await View.InvokeScriptAsync("eval", new[] { js });
                }
                IsLoadNextPage = false;

                if (datas.Any())
                {
                    NowImageCount += datas.Count;
                    //将当前页数据转为json格式的字符串
                    js = datas.ToJsonString();
                    //将数据添加到前台页面
                    await View.InvokeScriptAsync("AddImages", new[] { js });

                    NowPageIndex++;
                }

                if (IsFirst)
                {
                    IsFirst = false;
                    IsLoadNextPage = true;


                    datas = await GetNowPageData(await GetHtmlDocument());
                    ShowData(datas);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        private async Task<ObservableCollection<ImageListModel>> GetNowPageData(IHtmlDocument document)
        {
            return await Task.Run(() =>
            {
                ObservableCollection<ImageListModel> datas = new ObservableCollection<ImageListModel>();
                try
                {
                    var divs = document.QuerySelectorAll("#gdt .gdtl");
                    foreach (var element in divs)
                    {

                        var img = element.FirstElementChild.FirstElementChild;
                        var model = new ImageListModel();
                        //model.GetImageUrl += ModelGetImageUrl; 
                        model.Index = img.Attributes["alt"].Value;
                        model.Id = model.Index.ToInt();
                        model.Herf = element.FirstElementChild.Attributes["href"].Value;
                        model.CacheName = model.Herf.GetValidFileName() + ".jpg";
                        model.ImageUrl = img.Attributes["src"].Value;
                        model.Height = _imageHeight;

                        datas.Add(model);

                    }

                    return datas;
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                    throw ex;
                }
            });
        }

        private async Task<List<Torrent>> GetTorrent()
        {
            return await Task.Run(() =>
            {
                List<Torrent> torrents = new List<Torrent>();
                try
                {

                    var doc = GetHtmlDocument(_torrentPageUrl).Result;

                    var items = doc.QuerySelectorAll("table");

                    if (items != null && items.Any())
                    {
                        foreach (var node in items)
                        {
                            Torrent torrent = new Torrent();
                            var spans = node.QuerySelectorAll("span");
                            var a = node.QuerySelector("a");
                            torrent.Posted = spans.FirstOrDefault(x => x.InnerHtml == "Posted:").NextSibling.TextContent;
                            torrent.Size = spans.FirstOrDefault(x => x.InnerHtml == "Size:").NextSibling.TextContent;
                            torrent.Seeds = spans.FirstOrDefault(x => x.InnerHtml == "Seeds:").NextSibling.TextContent;
                            torrent.Peers = spans.FirstOrDefault(x => x.InnerHtml == "Peers:").NextSibling.TextContent;
                            torrent.Downloads =
                                spans.FirstOrDefault(x => x.InnerHtml == "Downloads:").NextSibling.TextContent;
                            torrent.Uploader =
                                spans.FirstOrDefault(x => x.InnerHtml == "Uploader:").NextSibling.TextContent;
                            torrent.Name = a.InnerHtml;
                            torrent.DownUrl = a.Attributes["href"].Value;
                            torrents.Add(torrent);
                        }
                    }


                }
                catch (Exception ex)
                {
                    ShowMessage(ex.ToString());
                }
                return torrents;
            });
        }

        private async void View_ScriptNotify(object sender, NotifyEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.Value))
                {
                    JObject jsonBody = JObject.Parse(e.Value);
                    string method = jsonBody["method"].ToString();
                    string data = jsonBody["data"].ToString();
                    switch (method)
                    {
                        case "ToListPage":
                            ImageListModel model = data.ToEntity<ImageListModel>();
                            PivotView.AddSelect(model.Title, new ListPage(model.Id, NowUrl, ImageCount));
                            break;
                        case "ToHomePage":
                            PivotView.AddSelect("主页", new HomePage(data));
                            break;
                        case "DownTorrent":
                            var torrent = _torrents.First(x => x.DownUrl == data);
                            try
                            {
                                var file = await Http.GetBtyeAsync(data);
                                AppSettings.DownPath = await FileHelper.DownFile(torrent.Name + ".torrent", file, AppSettings.DownPath);
                                ShowWebViewToast(torrent.Name + "下载成功!");
                            }
                            catch (Exception ex)
                            {
                                ShowWebViewToast(torrent.Name + "下载失败! 错误信息:" + ex.Message);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }
}
