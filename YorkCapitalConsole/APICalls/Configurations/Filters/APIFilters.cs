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
        internal string Default { get; set; } = string.Empty;
        internal APIWhere Where { get; set; }
    }        
}
