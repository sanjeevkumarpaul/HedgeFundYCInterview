using APICalls.Bases;
using APICalls.Entities.Interfaces;
using System;
using System.Net.Http;

namespace APICalls.Entities.Contracts
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
    }
}
