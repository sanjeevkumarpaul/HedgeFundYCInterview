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
        public bool NoRepeat { get; set; } = false;
        /// <summary>
        /// Objects which are refered inside the Configuration to which Parameters gets their input via Objects properties {Object.Property}
        /// </summary>
        public object[] ObjectParams { get; set; }
        /// <summary>
        /// Holds the Subscription result towards which Subscrition would Emit Events into it.
        /// </summary>
        public IAPIResult Subcriber { get; set; } = null;
    }
}
