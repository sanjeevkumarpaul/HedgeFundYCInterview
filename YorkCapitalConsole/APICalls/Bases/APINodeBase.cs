using APICalls.Configurations;
using APICalls.Configurations.Filters;
using APICalls.Dependents;
using APICalls.Entities;
using APICalls.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Bases
{
    internal class APINodeBase
    {
        internal string Name { get; set; }
        internal string ResultType { get; set; }
        internal string BaseUrl { get; set; }
        internal string ApiUri { get; set; }
        internal string ApiKey { get; set; }
        internal string IncludeKeyFromBase { get; set; }
        internal APIKeyPlacement KeyPlacement { get; set; } = APIKeyPlacement.QUERYSTRING;
        internal bool RequiredAuthorization { get; set; }
        internal APIAuthenticationType AuthenticationType { get; set; }
        internal string Token { get; set; }
        internal bool TokenAsHeader { get; set; }
        internal string TokenMaster { get; set; }
        internal Dictionary<string, string> ParametersQuery { get; set; } = new Dictionary<string, string>();
        internal Dictionary<string, string> ParametersBody { get; set; } = new Dictionary<string, string>();
        internal string ParameterContentType { get; set; }
        //internal List<APIParameter> Parameters { get; set; }
        //internal bool ParametersAsQueryString { get; set; } = false;
        internal Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Sepration to be done via semicolon at XML
        /// application/json[;application/jpeg[;...]]
        /// </summary>
        internal string ContentTypes { get; set; }                
        internal APIMethod Method { get; set; }
        internal bool Cache { get; set; } = false;
        /// <summary>
        /// Conditions for any parameter to be checked while assigning to it.
        /// </summary>
        internal List<APIFilter> Filters { get; set; } = new List<APIFilter>();
    }
}
