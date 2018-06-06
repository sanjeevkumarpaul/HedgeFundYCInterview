using APICalls.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations
{
    public class APIConfigurationOptions
    {
        /// <summary>
        /// Either XML or Json direct content or fully wualified path.
        /// </summary>
        public string PathOrContent { get; set; }

        /// <summary>
        /// Either XML/JSON. If any other string is provided XML will be considered.
        /// </summary>
        public string Type { get; set; } = "XML";

        public object[] ObjectParams { get; set; }

        public IAPIResult Subcriber { get; set; } = null;
    }
}
