using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations.Filters
{
    internal class APIFilter
    {
        internal string ParamterKey { get; set; }
        internal string Condition { get; set; }
        internal APIWhere Where { get; set; }
    }        
}
