using APICalls.Dependents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities.Contracts
{
    public sealed class APIRequestHeaders
    {
        public string ParameterContentType { get; set; }
        public APINamePareMedia[] Headers { get; set; }
        public string[] AcceptContentTypes { get; set; }
    }
}
