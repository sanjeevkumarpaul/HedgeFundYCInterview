using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace APICalls.Entities
{
    public class APIProspect<T> : IAPIProspect where T: new()
    {
        public string BaseUrl { get; set; }
        public string APIUril { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public bool ParametersIsQueryString { get; set; }
        public APIAuthorization Authorization { get; set; }
        public APIRequestHeaders RequestHeaders {get; set;}
        public T Result { get; set; }

        public string Url
        {
            get
            {
                string _url = $"{BaseUrl}{ (APIUril.Empty()? "" : "/") }{APIUril}".Trim();

                if (ParametersIsQueryString && Parameters != null)
                    Parameters.Keys.All(k => (_url += $"{k}={Parameters[k]}&") != null );

                return _url.TrimEx("&");
            }
        }
    }
}
