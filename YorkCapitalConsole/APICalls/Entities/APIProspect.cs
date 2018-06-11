using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APICalls.Entities.Interfaces;
using Extensions;

namespace APICalls.Entities
{
    public abstract class APIProspectOptionBase : IDisposable
    {
        public string BaseUrl { get; set; }
        public string APIUri { get; set; }
        public APIMethod Method { get; set; } = APIMethod.POST;
        public Dictionary<string, string> Parameters { get; set; }
        public bool ParametersIsQueryString { get; set; }
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
                    case APIMethod.STREAM: return HttpMethod.Trace;
                    case APIMethod.STRINGARRAY: return HttpMethod.Trace;
                    case APIMethod.BYTEARRAY: return HttpMethod.Trace;

                    default: return HttpMethod.Post;
                }
            }
        }
        internal string Url
        {
            get
            {
                string _url = $"{BaseUrl}{ (APIUri.Empty() ? "" : "/") }{APIUri}".Trim() + ((ParametersIsQueryString || Method == APIMethod.POST) ? QueryString : "");

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

                return (!_url.Empty() ? "?" : "") + _url.TrimEx("&");
            }
        }

        public void Dispose()
        {

        }
    }

    public class APIProspect<T> : APIProspectOptionBase where T: IAPIProspect, new()
    {        
        public T Result { get; set; }

        public APIProspect()
        {
            Result = new T();
        }
    }
}
