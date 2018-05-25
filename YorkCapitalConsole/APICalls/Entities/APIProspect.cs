using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace APICalls.Entities
{
    public class APIProspect<T> : IAPIProspect where T: new()
    {
        public string BaseUrl { get; set; }
        public string APIUri { get; set; }
        public HttpMethod Method { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public bool ParametersIsQueryString { get; set; }
        public APIAuthorization Authorization { get; set; }
        public APIRequestHeaders RequestHeaders {get; set;}
        public T Result { get; set; }

        internal string Url
        {
            get
            {
                string _url = $"{BaseUrl}{ (APIUri.Empty() ? "" : "/") }{APIUri}".Trim() + ( (ParametersIsQueryString || Method == HttpMethod.Get) ? QueryString : "");

                return _url;

            }
        }

        internal string QueryString
        {
            get
            {
                string _url = "";

                if (Parameters != null)
                  Parameters.Keys.All(k => (_url += $"{k}={Parameters[k]}&") != null);

                return  (!_url.Empty()? "?" : "") + _url.TrimEx("&");
            }
        }
    }
}
