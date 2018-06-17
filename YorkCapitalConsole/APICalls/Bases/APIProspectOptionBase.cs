using APICalls.Entities.Contracts;
using APICalls.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace APICalls.Bases
{
    public abstract class APIProspectOptionBase : IDisposable
    {
        public string BaseUrl { get; set; }
        public string APIUri { get; set; }
        public APIMethod Method { get; set; } = APIMethod.POST;
        public Dictionary<string, string> ParameterQuery { get; set; }
        public Dictionary<string, string> ParameterBody { get; set; }        
        public APIAuthorization Authorization { get; set; }
        public APIRequestHeaders RequestHeaders { get; set; }

        internal HttpMethod HttpMethod
        {
            get
            {
                switch (Method)
                {
                    case APIMethod.POST: return HttpMethod.Post;
                    case APIMethod.GET: return HttpMethod.Get;
                    case APIMethod.PUT: return HttpMethod.Put;
                    case APIMethod.DELETE: return HttpMethod.Delete;
                    case APIMethod.STREAM:
                    case APIMethod.STRINGARRAY: 
                    case APIMethod.BYTEARRAY: 
                    default: return HttpMethod.Post;
                }
            }
        }
        internal string Url
        {
            get
            {
                string _url = $"{BaseUrl}{ (APIUri.Empty() ? "" : "/") }{APIUri}".Trim() + (ParameterQuery != null ? QueryString : "");

                return _url;

            }
        }

        internal string QueryString
        {
            get
            {
                string _url = "";

                if (ParameterQuery != null)
                    ParameterQuery.Keys.All(k => (_url += $"{k}={ParameterQuery[k]}&") != null);

                return (!_url.Empty() ? "?" : "") + _url.TrimEx("&");
            }
        }

        public void Dispose()
        {

        }
    }
}
