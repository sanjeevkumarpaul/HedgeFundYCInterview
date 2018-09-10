using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public class ConsoleColumnOptions
    {
        public int Width { get; set; }
        public ConsoleAlignment Alignment { get; set; }
        public ConsoleColor Color { get; set; }
        public ConsoleAggregate Aggregate { get; set; } = ConsoleAggregate.NONE;
        public ConsoleWrapType Wrap { get; set; } = ConsoleWrapType.WRAP;
        public ConsoleGroupByOption Group { get; set; }
        public char WrapCharCharacter { get; set; } = '~';
    }
}
