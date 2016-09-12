using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHentai.uwp.Common;
using System.Net;
using Windows.Graphics.Imaging;
using Uwp.Http;

namespace EHentai.uwp.Model
{
    public abstract class Site
    {
        public string LogonUrl => "https://forums.e-hentai.org/index.php?act=Login&CODE=01";

        public abstract string HomeUrl { get; }

        public Http Http { get; set; }
        public bool IsLogin => Http.Cookie != null;
        public string UserName { get; set; }
        public string PassWord { get; set; }

        public event EventHandler OnLogined; //完成事件

        protected Site()
        {
            try
            {
                Http = new Http();
                //Http.Cookie = CookieHelper.Cookie;
            }
            catch (Exception ex)
            {
            }
        }

        ///// <summary>
        ///// 登录
        ///// </summary>
        ///// <param name="userName">用户名</param>
        ///// <param name="passWord">密码</param>
        ///// <returns></returns>
        //public async void Login()
        //{
        //    try
        //    {
        //        string userName = "516018579";
        //        string passWord = "15107210156";
        //        UserName = userName;
        //        PassWord = passWord;

        //        string url = LogonUrl;
        //        string data = "returntype=8&CookieDate=1&b=d&bt=pone&UserName=" + UserName + "&PassWord=" + PassWord + "&ipb_login_submit=Login%21";
        //        string result = await Http.PostStringAsync(url, data);
        //        //判断是否登录成功
        //        if (result.Contains("You are now logged in as:"))
        //        {
        //            //登录成功后获取ex的cookie
        //            url = HomeUrl + "/?inline_set=dm_t";
        //            await Http.GetStringAsync(url);

        //            url = HomeUrl + "/?inline_set=ts_l";
        //            result = await Http.GetStringAsync(url);

        //            //判断是否成功获取到cookie
        //            if (result.Contains(HomeUrl))
        //            {
        //                Cookie.Cookie = Http.Cookie;
        //                OnLogined?.Invoke(this, EventArgs.Empty);
        //            }
        //            else
        //            {
        //                throw new Exception("登录失败!");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public async Task<string> GetStringAsync(string url)
        {
            try
            {
                if (!ValidateCookie())
                {
                    Login();
                }
                string result = await Http.GetStringAsync(url);
                if (!ValidateHtml(result))
                {
                    Login();
                    result = await Http.GetStringAsync(url);
                }
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Stream> GetStreamAsync(string url)
        {
            try
            {
                if (!ValidateCookie())
                {
                    Login();
                }
                var stream = await Http.GetStreamAsync(url);
                return stream;
                //return Http.GetMemoryStream(stream);
            }
            catch (Exception e)
            {
                return null;
                //throw e;
            }
        }

        public async Task<SoftwareBitmap> DownloadImage(string url)
        {
            try
            {
                if (!ValidateCookie())
                {
                    Login();
                }
                var stream = await Http.DownloadImage(url);
                return stream;
                //return Http.GetMemoryStream(stream);
            }
            catch (Exception e)
            {
                return null;
                //throw e;
            }
        }

        /// <summary>
        /// 验证是否能正常获取网站数据
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public bool ValidateCookie()
        {
            return Http.Cookie != null;
        }

        public bool ValidateHtml(string html)
        {
            return html.Contains(HomeUrl + "/z/0322/x.css");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public async Task<bool> Login()
        {
            try
            {
                string userName = "516018579";
                string passWord = "15107210156";
                UserName = userName;
                PassWord = passWord;

                string url = LogonUrl;
                //string data = "returntype=8&CookieDate=1&b=d&bt=pone&UserName=" + UserName + "&PassWord=" + PassWord + "&ipb_login_submit=Login%21";
                var data = new Dictionary<string, string>
                {
                    {"returntype", "8"},
                    {"CookieDate", "1"},
                    {"b", "d"},
                    {"bt", "pone"},
                    {"UserName",userName},
                    {"PassWord", passWord},
                    {"ipb_login_submit", "Login%21"},
                };
                string result = await Http.PostStringAsync(url, data);
                //判断是否登录成功
                if (result.Contains("You are now logged in as:"))
                {
                    //return SetConfig();
                }
                throw new Exception("登录失败!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        ///// <summary>
        ///// 设置网站配置为大图显示
        ///// </summary>
        //public bool SetConfig()
        //{
        //    string url = HomeUrl + "/?inline_set=dm_t";
        //    string result = Http.GetStringAsync(url);

        //    url = HomeUrl + "/?inline_set=ts_l";
        //    result = Http.GetStringAsync(url);
        //    if (result.Contains(HomeUrl) && result.Contains("class=\"id3\""))
        //    {
        //        CookieHelper.Cookie = Http.Cookie;
        //        OnLogined?.Invoke(this, EventArgs.Empty);
        //        return true;
        //    }

        //    return false;
        //}
    }
}
