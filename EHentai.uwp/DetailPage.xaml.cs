using System;
using System.Collections.Generic;
using System.IO;
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
using Newtonsoft.Json.Linq;
using Uwp.Common.Extend;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EHentai.uwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailPage : ImagePage
    {
        public double ImageHeight = 250;
        string enTitle = ""; //英文标题
        string jpTitle = ""; //日文标题


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
            string js;
            if (IsFirst)
            {
                //设置标题
                js = $"scope.enTitle='{enTitle.HtmlEncode()}'; scope.jpTitle='{jpTitle.HtmlEncode()}'; scope.$apply();";
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

                datas = await GetNowPageData();
                ShowData(datas);
            }
        }

        private async Task<ObservableCollection<ImageListModel>> GetNowPageData()
        {
            return await Task.Run(() =>
            {
                ObservableCollection<ImageListModel> datas = new ObservableCollection<ImageListModel>();
                try
                {
                    HtmlNode document; //当前页html对象

                    if (IsFirst)
                    {
                        document = GetHtml().DocumentNode;

                        ImageCount = int.Parse(document.SelectNodes("//*[@class=\"gdt1\"]").First(x => x.InnerHtml == "Length:").NextSibling.InnerHtml.Split(' ')[0]);
                        ImageHeight = int.Parse(document.SelectSingleNode("//*[@class=\"gdtl\"]").Attributes["style"].Value.Split(':')[1].Replace("px", "")) - 20;
                        //string coverUrl = document.SelectSingleNode("//[@id=\"gd1\"]").SelectSingleNode("//img").Attributes["src"].Value;

                        enTitle = document.SelectSingleNode("//*[@id=\"gn\"]").InnerHtml; //英文标题
                        jpTitle = document.SelectSingleNode("//*[@id=\"gj\"]").InnerHtml; //日文标题

                        //var download = document.SelectNodes("//*[@id=\"gd5\"]/[@class=\"g2\"]/a").Where(x => x.InnerHtml.Contains("Torrent Download") && !x.InnerHtml.Contains("0"));
                        
                        //CreateTask(() =>
                        //{
                        //    //获取封面图片
                        //    string imgCache = CommonHepler.GetValidFileName(coverUrl) + ".jpg";
                        //    while (!ImageCache.HasCache(imgCache))
                        //    {
                        //        var imgStream = Http.GetStream(coverUrl);
                        //        ImageCache.CreateCache(imgCache, imgStream);
                        //    }
                        //    Dispatcher.BeginInvoke(new Action(() =>
                        //    {
                        //        CoverImage.Source = ImageCache.GetImage(imgCache);
                        //    }));
                        //});
                    }
                    else
                    {
                        if (!IsLoaded)
                            document = GetHtml().DocumentNode;
                        else
                            return datas;
                    }

                    var divs = document.SelectNodes("//*[@id=\"gdt\"]/*[@class=\"gdtl\"]");
                    foreach (var element in divs)
                    {
                        var img = element.FirstChild.FirstChild;
                        var model = new ImageListModel();
                        //model.GetImageUrl += ModelGetImageUrl; 
                        model.Index = img.Attributes["alt"].Value;
                        model.Herf = element.FirstChild.Attributes["href"].Value;
                        model.CacheName = model.Herf.GetValidFileName() + ".jpg";
                        model.ImageUrl = img.Attributes["src"].Value;
                        model.Height = ImageHeight;

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

        private void ModelGetImageUrl(object sender, EventArgs e)
        {
            var model = sender as ImageListModel;
            GetImageBase64Async(model);
        }

        private void View_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value) && e.Value.Contains("ToListPage"))
            {
                JObject jsonBody = JObject.Parse(e.Value);

                ImageListModel model = jsonBody["data"].ToString().ToEntity<ImageListModel>();
                PivotView.AddSelect(model.Title, new ListPage(model.Herf));
            }
        }
    }
}
