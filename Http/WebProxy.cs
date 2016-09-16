using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Uwp.Http
{
    public class WebProxy : IWebProxy
    {
        public string Host { get; }
        public int Port { get; }

        public WebProxy(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public Uri GetProxy(Uri destination)
        {
            return new Uri($"http://{Host}:{Port}");
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

        public ICredentials Credentials { get; set; }
    }
}
