using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations.Filters
{
    internal class APIWhere
    {
        public List<APICondition> AndConditions { get; set; } = new List<APICondition>();
        public List<APICondition> OrConditions { get; set; } = new List<APICondition>();

        public bool Exists
        {
            get
            {
                return AndConditions.Any(a => a != null) || OrConditions.Any(a => a != null);
            }
        }

    }
}
