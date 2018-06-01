using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities
{
    public class APINodeBase
    {
        public string Name { get; set; }
        public string GenericType { get; set; }
        public string BaseUrl { get; set; }
        public string ApiUri { get; set; }
        public bool RequiredAuthorization { get; set; }
        public APIAuthenticationType AuthenticationType { get; set; }
        public string Token { get; set; }
        public bool TokenAsHeader { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Sepration to be done via semicolon at XML
        /// application/json[;application/jpeg[;...]]
        /// </summary>
        public string ContentType { get; set; }
        public APIMethod Method { get; set; }
    }
}
