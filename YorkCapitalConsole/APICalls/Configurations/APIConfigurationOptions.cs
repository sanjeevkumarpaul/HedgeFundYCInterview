using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations
{
    public class APIConfigurationOptions
    {
        public string Path { get; set; }
        /// <summary>
        /// Either XML or Json if Path is not provided.
        /// </summary>
        public string ConfigString { get; set; }

        /// <summary>
        /// Either XML/JSON. If any other string is provided XML will be considered.
        /// </summary>
        public string Type { get; set; } = "XML";

        public object[] ObjectParams { get; set; }
    }
}
