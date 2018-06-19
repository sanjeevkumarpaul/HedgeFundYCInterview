using APICalls.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations.Filters
{
    internal class APICondition
    {
        public string Operand { get; set; }
        public APIOperator? Operator { get; set; } = APIOperator.EQ;
        public string Value { get; set; }
    }
}
