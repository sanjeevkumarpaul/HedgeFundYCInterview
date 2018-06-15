using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Dependents
{
    internal class APIParameter
    {       
        internal bool ParametersAsQueryString { get; set; }
        internal Dictionary<string, string> Items { get; set; }
    }

}
