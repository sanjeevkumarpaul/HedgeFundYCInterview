using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities
{
    public class APIException : Exception, IAPIProspect
    {
        public string Status { get; private set; }        
        public string Url { get; private set; }
        public string Method { get; private set; }
        public APIException(HttpResponseMessage response, APIProspectOptionBase prospect = null) : base(response.ReasonPhrase)
        {
            Status = response.StatusCode.ToString();

            Url = prospect?.Url;
            Method = prospect?.Method.ToString();            
        }

        /// <summary>
        /// Has to be there for IAPIProspect to work.
        /// </summary>
        dynamic IAPIProspect.OtherResponses { get; set; }
    }
}
