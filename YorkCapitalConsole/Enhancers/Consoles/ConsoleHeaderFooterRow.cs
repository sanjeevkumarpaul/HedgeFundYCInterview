using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public partial class ConsoleHeaderFooterRow 
    {
        private ConsoleRecord _heading;
        private ConsoleRecord _value;


        public ConsoleRecord Heading { get { return _heading;  } set { value.Color = ConsoleColor.DarkYellow; _heading = value; } } 
        public ConsoleRecord Value { get { return _value; } set { value.Color = ConsoleColor.DarkCyan; _value = value; } }
        public ConsoleAlignment Alignment { get; set; } = ConsoleAlignment.LEFT;
    }
}
