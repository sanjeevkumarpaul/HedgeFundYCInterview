using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities
{
    public class APIRequestHeaders
    {
        public string ParameterContentType { get; set; }
        public APINamePareMedia[] Headers { get; set; }
        public string[] AcceptContentTypes { get; set; }
    }

    public class APINamePareMedia
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
