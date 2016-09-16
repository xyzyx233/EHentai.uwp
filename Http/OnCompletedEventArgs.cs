using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uwp.Http
{
    /// <summary>
    /// 请求完成事件
    /// </summary>
    public class OnCompletedEventArgs
    {
        public Http Http { get; private set; }//请求对象
        public string ResultString { get; private set; }// 返回的字符串结果
        public MemoryStream ResultStream { get; private set; }// 返回的文件流结果
        public OnCompletedEventArgs(string resultString = null, MemoryStream resultStream = null, Http http = null)
        {
            ResultString = resultString;
            ResultStream = resultStream;
            Http = http;
        }
    }
}
