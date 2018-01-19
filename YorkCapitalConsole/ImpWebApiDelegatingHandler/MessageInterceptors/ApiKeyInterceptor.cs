using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using RequestMessageHandler.Entities;

namespace ImpWebApiDelegatingHandler.MessageInterceptors
{
    public class ApiKeyInterceptor : WebIdentity
    {
        public string Connection { get; set; }
        public string Accept { get; set; }
        public string AcceptLanguage { get; set; }
        public string Host { get; set; }
    }
}