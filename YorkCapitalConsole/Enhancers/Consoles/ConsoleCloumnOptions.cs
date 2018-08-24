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
        public WrapAlignment Alignment { get; set; }
        public ConsoleColor Color { get; set; }
        public WrapAggregate Aggregate { get; set; } = WrapAggregate.NONE;
        public WrapConsoleWrapType Wrap { get; set; } = WrapConsoleWrapType.WRAP;
        public ConsoleGroupByOption Group { get; set; }
    }
}
