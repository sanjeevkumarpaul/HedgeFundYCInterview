using APICalls.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations
{
    /// <summary>
    /// Public class to create Options for API to execute.
    /// </summary>
    public class APIConfigurationOptions
    {
        private IAPIParallelResult _subscriberParallel = null;
        
        /// <summary>
        /// Either XML or Json direct content or fully wualified path.
        /// </summary>
        public string PathOrContent { get; set; }
        /// <summary>
        /// Either XML/JSON. If any other string is provided XML will be considered.
        /// </summary>
        public string Type { get; set; } = "XML";
        /// <summary>
        /// If TRUE, Repeat configuration will not be executed.
        /// </summary>
        public bool NoRepeat { get; set; } = false;
        /// <summary>
        /// Objects which are refered inside the Configuration to which Parameters gets their input via Objects properties {Object.Property}
        /// </summary>
        public object[] ObjectParams { get; set; }
        /// <summary>
        /// Holds the Subscription result towards which Subscrition would Emit Events into it.
        /// </summary>
        public IAPIResult Subscriber { get; set; } = null;

        internal IAPIParallelResult SubscriberParallel { get { return _subscriberParallel;  } }

        internal void Validate()
        {
            _subscriberParallel = Subscriber is IAPIParallelResult ? Subscriber as IAPIParallelResult :  null;
        }
    }
}
