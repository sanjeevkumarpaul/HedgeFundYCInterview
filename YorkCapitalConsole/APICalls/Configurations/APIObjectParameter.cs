using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations
{
    internal class APIObjectParameter
    {
        internal List<object> ObjectParams { get; } = null;

        internal APIObjectParameter() { ObjectParams = new List<object>(); }
    }    
}
