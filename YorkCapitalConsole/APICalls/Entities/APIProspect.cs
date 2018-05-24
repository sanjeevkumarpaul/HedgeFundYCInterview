using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities
{
    public class APIProspect<T> : IAPIProspect where T: new()
    {
        public string BaseUrl { get; set; }
        public string APIUril { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public bool ParametersIsQueryString { get; set; }
        public APIAuthorization Authorization { get; set; }
        public APIContentType ContentType {get; set;}

        public T Result { get; set; }
    }
}
