using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations
{
    internal class APIObjectParameter
    {
       internal HashSet<object> Params { get; } = null;

        internal APIObjectParameter() { Params = new HashSet<object>(); }
    }    
}
