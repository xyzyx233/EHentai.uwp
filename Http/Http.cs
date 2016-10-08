using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Uwp.Http
{
    public class Http
    {
        #region 请求参数

        public HttpClientHandler Handler { get; set; }

        public HttpClient Client { get; set; }

        //public CookieContainer Cookie { get; set; }
        public static string Cookie = "igneous=61ba9adb4afe5133cacfbddcd2d8c2d11a92fa8a67ce1d9520d446bf0bad49cf5339d22d0523b3416dda8974f8645a73dec6e02bb075041f94293d44c9715dcd;ipb_member_id=1298700;ipb_pass_hash=cb10b3075674d6c425540fb646d05368;s=da3296d6b925660005f6de90009e14ac766829153dc29435737c8fa76b8458c13b8566f81206b184588ffb7920ad9edce1fe5d636224dcebbad69c7f894798e7;uconfig=uh_y-rc_0-cats_0-xns_0-ts_l-tr_2-prn_y-dm_t-ar_0-rx_0-ry_0-ms_n-mt_n-cs_a-to_a-pn_0-sc_0-lt_m-tl_r-fs_p-ru_rrggb-xr_a-sa_y-oi_n-qb_n-tf_n-hh_-hp_-hk_-xl_;lv=1475394535-1475745879;";
        #endregion

        public event EventHandler<OnCompletedEventArgs> OnCompleted; //完成事件

        public Http()
        {
            Handler = new HttpClientHandler();
            Handler.AutomaticDecompression = DecompressionMethods.GZip;
            Handler.UseCookies = false;

            Client = new HttpClient(Handler);
            Client.BaseAddress = new Uri("http://example.com");
            Client.Timeout = TimeSpan.FromSeconds(30);
        }

        public HttpRequestMessage GetRequest(string url)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, url);
            message.Headers.Add("Cookie", Cookie);
            return message;
        }

        public async Task<string> GetStringAsync(string url)
        {
            var result = await Client.SendAsync(GetRequest(url));
            result.EnsureSuccessStatusCode();
            return WebUtility.HtmlDecode(await result.Content.ReadAsStringAsync());
        }
        public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken)
        {
            var result = await Client.SendAsync(GetRequest(url), cancellationToken);
            result.EnsureSuccessStatusCode();
            return WebUtility.HtmlDecode(await result.Content.ReadAsStringAsync());
        }

        public async Task<Stream> GetStreamAsync(string url)
        {
            var result = await Client.SendAsync(GetRequest(url));
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsStreamAsync();
        }
        public async Task<Stream> GetStreamAsync(string url, CancellationToken cancellationToken)
        {
            var result = await Client.SendAsync(GetRequest(url), cancellationToken);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsStreamAsync();
        }

        public async Task<byte[]> GetBtyeAsync(string url)
        {
            var result = await Client.SendAsync(GetRequest(url));
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsByteArrayAsync();
        }
        public async Task<byte[]> GetBtyeAsync(string url, CancellationToken cancellationToken)
        {
            var result = await Client.SendAsync(GetRequest(url), cancellationToken);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsByteArrayAsync();
        }

        public async Task<SoftwareBitmap> DownloadImage(string url)
        {
            try
            {
                IInputStream inputStream = (await GetStreamAsync(url)).AsInputStream();
                IRandomAccessStream memStream = new InMemoryRandomAccessStream();
                await RandomAccessStream.CopyAsync(inputStream, memStream);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(memStream);
                SoftwareBitmap softBmp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                return softBmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<SoftwareBitmap> DownloadImage(string url, CancellationToken cancellationToken)
        {
            try
            {
                IInputStream inputStream = (await GetStreamAsync(url, cancellationToken)).AsInputStream();
                IRandomAccessStream memStream = new InMemoryRandomAccessStream();
                await RandomAccessStream.CopyAsync(inputStream, memStream);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(memStream);
                SoftwareBitmap softBmp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                return softBmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<HttpResponseMessage> GetResponseAsync(string url, Dictionary<string, string> postData)
        {
            var data = new FormUrlEncodedContent(postData);
            data.Headers.Add("Cookie", Cookie);
            var response = await Client.PostAsync(url, data);
            response.EnsureSuccessStatusCode();
            return response;
        }
        public async Task<HttpResponseMessage> GetResponseAsync(string url, Dictionary<string, string> postData, CancellationToken cancellationToken)
        {
            var data = new FormUrlEncodedContent(postData);
            data.Headers.Add("Cookie", Cookie);
            var response = await Client.PostAsync(url, data, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<string> PostStringAsync(string url, Dictionary<string, string> postData)
        {
            try
            {
                var response = GetResponseAsync(url, postData);
                return WebUtility.HtmlDecode(await (await response).Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> PostStringAsync(string url, Dictionary<string, string> postData, CancellationToken cancellationToken)
        {
            try
            {
                var response = GetResponseAsync(url, postData, cancellationToken);
                return WebUtility.HtmlDecode(await (await response).Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Stream> PostStreamAsync(string url, Dictionary<string, string> postData)
        {
            try
            {
                var response = GetResponseAsync(url, postData);
                return await (await response).Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Stream> PostStreamAsync(string url, Dictionary<string, string> postData, CancellationToken cancellationToken)
        {
            try
            {
                var response = GetResponseAsync(url, postData, cancellationToken);
                return await (await response).Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        ///// <summary>
        ///// 创建Request请求
        ///// </summary>
        ///// <param name="url">请求地址</param>
        ///// <param name="param">请求参数</param>
        ///// <param name="method">请求方式(GET/POST)  默认POST</param>
        ///// <returns></returns>
        //private HttpWebRequest CreateRequest(string url, string method = "POST")
        //{
        //    try
        //    {
        //        //创建一个http请求
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        //        //if (!string.IsNullOrEmpty(ProxyIp) && ProxyPort > 0)
        //        //    request.Proxy = new WebProxy(ProxyIp, ProxyPort);
        //        //request.Timeout = 2000;
        //        //设置请求方式
        //        request.Method = method;
        //        //设置cookie
        //        request.CookieContainer = Cookie ?? (Cookie = new CookieContainer());
        //        //设置请求头
        //        request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.125 Safari/537.36";
        //        request.Headers["Content-Encoding"] = "gzip";
        //        //request.Accept = "application/json, text/javascript, */*; q=0.01";
        //        if (Headers != null && Headers.Any())
        //        {
        //            foreach (var header in Headers)
        //            {
        //                request.Headers[header[0]] = header[1];
        //            }
        //        }
        //        //request.Headers.Add("X-Requested-With", "XMLHttpRequest");

        //        return request;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //#region Get
        ///// <summary>
        ///// 创建Get请求
        ///// </summary>
        ///// <returns></returns>
        //private HttpWebRequest CreateGetRequest(string url)
        //{
        //    return CreateRequest(url, "GET");
        //}

        ///// <summary>
        ///// 获取Get请求结果
        ///// </summary>
        ///// <returns></returns>
        //private HttpWebResponse HttpGet(string url)
        //{
        //    try
        //    {
        //        var response = CreateGetRequest(url).GetResponseAsync().Result;
        //        return (HttpWebResponse)response;
        //    }
        //    catch (WebException ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// 返回GET请求的字符串结果
        ///// </summary>
        ///// <returns></returns>
        //public string GetStringAsync(string url)
        //{
        //    HttpWebResponse response = null;
        //    try
        //    {
        //        string retString = null;//保存返回的结果
        //        //创建一个GET请求
        //        response = HttpGet(url);
        //        Cookie.Add(new Uri(url), response.Cookies);
        //        //获取请求结果返回的文件流
        //        Stream responseStream = GetStreamAsync(response);
        //        if (responseStream != null)
        //            retString = GetStringByStream(responseStream);
        //        //请求完成触发事件
        //        OnCompleted?.Invoke(this, new OnCompletedEventArgs(retString, http: this));
        //        response.Dispose();
        //        return retString;
        //    }
        //    //捕获网络请求错误,并返回错误代码
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    finally
        //    {
        //        response?.Dispose();
        //    }
        //}

        ///// <summary>
        ///// GET请求文件
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //public MemoryStream GetStreamAsync(string url)
        //{
        //    HttpWebResponse response = null;
        //    try
        //    {
        //        //创建一个GET请求
        //        response = HttpGet(url);
        //        Cookie.Add(new Uri(url), response.Cookies);
        //        //获取请求结果返回的文件流
        //        Stream responseStream = GetStreamAsync(response);
        //        MemoryStream stream = null;//保存返回的结果
        //        if (responseStream != null)
        //            stream = GetMemoryStream(responseStream);
        //        //请求完成触发事件
        //        OnCompleted?.Invoke(this, new OnCompletedEventArgs(resultStream: stream, http: this));
        //        response.Dispose();
        //        return stream;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    finally
        //    {
        //        response?.Dispose();
        //    }
        //}
        //#endregion

        //#region POST

        ///// <summary>
        ///// 创建Post请求
        ///// </summary>
        ///// <param name="url">请求地址</param>
        ///// <param name="param">POST参数</param>
        //private HttpWebRequest CreatePostRequest(string url, string param)
        //{
        //    //创建一个http请求
        //    var request = CreateRequest(url);
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    //request.ContentLength = Encoding.GetEncoding(0).GetByteCount(param);
        //    using (Stream stream = request.GetRequestStreamAsync().Result)
        //    {
        //        using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.GetEncoding(0)))
        //        {
        //            streamWriter.Write(param);
        //        }
        //    }
        //    return request;
        //}

        ///// <summary>
        ///// 获取Post请求结果
        ///// </summary>
        ///// <returns></returns>
        //private HttpWebResponse HttpPost(string url, string param)
        //{
        //    try
        //    {
        //        var response = CreatePostRequest(url, param).GetResponseAsync().Result;
        //        return (HttpWebResponse)response;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// 返回Post请求的字符串结果
        ///// </summary>
        ///// <returns></returns>
        //public string PostStringAsync(string url, string param = "")
        //{
        //    HttpWebResponse response = null;
        //    try
        //    {
        //        string retString = null;//保存返回的结果
        //        //创建一个POST请求
        //        response = HttpPost(url, param);
        //        Cookie.Add(new Uri(url), response.Cookies);
        //        //获取请求结果返回的文件流
        //        Stream responseStream = GetStreamAsync(response);
        //        if (responseStream != null)
        //            retString = GetStringByStream(responseStream);
        //        //请求完成触发事件
        //        OnCompleted?.Invoke(this, new OnCompletedEventArgs(retString, http: this));
        //        response.Dispose();
        //        return retString;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    finally
        //    {
        //        response?.Dispose();
        //    }
        //}

        ///// <summary>
        ///// Post请求文件
        ///// </summary>
        ///// <returns></returns>
        //public MemoryStream PostStreamAsync(string url, string param = "")
        //{
        //    HttpWebResponse response = null;
        //    try
        //    {
        //        MemoryStream stream = null;//保存返回的结果
        //        //创建一个POST请求
        //        response = HttpPost(url, param);
        //        //获取请求结果返回的文件流
        //        Stream responseStream = GetStreamAsync(response);
        //        if (responseStream != null)
        //            stream = GetMemoryStream(responseStream);
        //        //请求完成触发事件
        //        OnCompleted?.Invoke(this, new OnCompletedEventArgs(resultStream: stream, http: this));
        //        response.Dispose();
        //        return stream;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        response?.Dispose();
        //    }
        //}

        //#endregion

        ///// <summary>
        ///// 根据文件流生成MemoryStream
        ///// </summary>
        ///// <param name="responseStream"></param>
        ///// <returns></returns>
        //public MemoryStream GetMemoryStream(Stream stream)
        //{
        //    if (stream != null)
        //    {
        //        //将Stream转为MemoryStream
        //        MemoryStream stmMemory = new MemoryStream();
        //        byte[] buffer = new byte[64 * 1024];
        //        int i;
        //        while ((i = stream.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            stmMemory.Write(buffer, 0, i);
        //        }
        //        byte[] arraryByte = stmMemory.ToArray();

        //        return new MemoryStream(arraryByte);
        //    }
        //    return null;
        //}

        ///// <summary>
        /////  根据文件流生成字符串
        ///// </summary>
        ///// <param name="responseStream"></param>
        ///// <returns></returns>
        //private string GetStringByStream(Stream stream)
        //{
        //    string retString = "";
        //    try
        //    {

        //        if (stream != null)
        //        {
        //            using (StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
        //            {
        //                //读取文件流并保存为字符串
        //                retString = streamReader.ReadToEnd();
        //            }
        //            stream.Dispose();
        //        }

        //        //html文本解码
        //        retString = WebUtility.HtmlDecode(retString);
        //        return Regex.Unescape(retString ?? "");
        //    }
        //    catch (Exception ex)
        //    {
        //        return retString;
        //    }
        //}

        //private Stream GetStreamAsync(HttpWebResponse response)
        //{
        //    Stream stream;
        //    string encoding = string.IsNullOrEmpty(response.Headers["Content-Encoding"]) ? "" : response.Headers["Content-Encoding"].ToLower();
        //    if (encoding.Contains("gzip"))
        //    {
        //        stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
        //    }
        //    else if (encoding.Contains("deflate"))
        //    {
        //        stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
        //    }
        //    else
        //    {
        //        stream = response.GetResponseStream();
        //    }


        //    return stream;
        //}
    }
}
