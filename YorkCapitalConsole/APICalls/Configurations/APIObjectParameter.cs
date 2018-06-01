using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations
{
    public class APIObjectParameter
    {
        internal List<object> ObjectParams { get; } = null;

        public APIObjectParameter() { ObjectParams = new List<object>(); }
    }    
}
