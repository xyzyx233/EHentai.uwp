using System;
using System.Collections.Generic;
using System.IO;
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
using Newtonsoft.Json.Linq;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EHentai.uwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailPage : ImagePage
    {
        public double ImageHeight = 250;
        string enTitle = "";

        public DetailPage(string url) : base(url, "p")
        {
            InitializeComponent();
        }

        public override void LoadDataByPage()
        {
            CreateTask(() =>
            {
                try
                {
                    LoadNowPageData();

                    IsLoadNextPage = false;
                    //ImageGrid.HideLoading();


                    CreateTask(() =>
                    {
                        if (NowPageIndex == 1)
                        {
                            IsLoadNextPage = true;
                            NowPageIndex++;

                            LoadNowPageData();
                        }
                    });

                }
                catch (Exception ex)
                {
                    //ImageGrid.HideLoading();
                    ShowMessage(ex.Message);
                }
            });
        }

        private async void LoadNowPageData()
        {
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
                    //string jpTitle = document.SelectSingleNode("//*[@id=\"gj\"]").InnerHtml; //日文标题
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



                    //IsFirst = false;
                }
                else
                {
                    if (!IsLoadedImage())
                        document = GetHtml().DocumentNode;
                    else
                        return;
                }

                var divs = document.SelectNodes("//*[@id=\"gdt\"]/*[@class=\"gdtl\"]");

                foreach (var element in divs)
                {
                    var img = element.FirstChild.FirstChild;
                    var model = new ImageListModel();
                    model.GetImageUrl += ModelGetImageUrl; ;
                    model.Index = img.Attributes["alt"].Value;
                    model.Herf = element.FirstChild.Attributes["href"].Value;
                    model.CacheName = model.Herf.GetValidFileName() + ".jpg";
                    model.ImageUrl = img.Attributes["src"].Value;
                    model.Height = ImageHeight;
                    model.Title = enTitle;


                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        try
                        {
                            ImageList.Add(model);

                            string js = model.ToJsonString();

                            await DetailView.InvokeScriptAsync("AddImages", new[] { js });
                        }
                        catch (Exception ex)
                        { }
                    });
                    if (IsFirst)
                    {
                        IsLoadNextPage = IsFirst = false;
                        //ImageGrid.HideLoading();
                    }
                    else if (IsLoadNextPage)
                    {
                        IsLoadNextPage = false;
                    }
                }

                NowImageCount = ImageList.Count;
                NowPageIndex++;

            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        private void ModelGetImageUrl(object sender, EventArgs e)
        {
            var model = sender as ImageListModel;
            GetImageBase64Async(model);
        }

        private void DetailPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            DetailView.ScriptNotify += DetailView_ScriptNotify;
        }

        private void DetailView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value) && e.Value.Contains("ToListPage"))
            {
                JObject jsonBody = JObject.Parse(e.Value);

                ImageListModel model = jsonBody["data"].ToString().ToEntity<ImageListModel>();
                Main.Add(model.Title, new ListPage(model.Herf));
            }
        }
    }
}
