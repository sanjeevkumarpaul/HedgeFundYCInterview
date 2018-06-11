using APICalls.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations
{
    public class APINodeBase
    {
        public string Name { get; set; }
        public string GenericType { get; set; }
        public string BaseUrl { get; set; }
        public string ApiUri { get; set; }
        public string ApiKey { get; set; }
        public string IncludeKeyFromBase { get; set; }
        public bool RequiredAuthorization { get; set; }
        public APIAuthenticationType AuthenticationType { get; set; }
        public string Token { get; set; }
        public bool TokenAsHeader { get; set; }
        public string TokenMaster { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public bool ParametersAsQueryString { get; set; } = false;
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Sepration to be done via semicolon at XML
        /// application/json[;application/jpeg[;...]]
        /// </summary>
        public string ContentTypes { get; set; }        
        public string ParamContentType { get; set; }
        public APIMethod Method { get; set; }
    }
}
