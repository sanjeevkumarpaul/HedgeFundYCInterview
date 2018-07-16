using System.Collections.Generic;

namespace APICalls.Dependents
{
    internal class APIParameter
    {       
        internal bool ParametersAsQueryString { get; set; }
        internal Dictionary<string, string> Items { get; set; }
    }

}
