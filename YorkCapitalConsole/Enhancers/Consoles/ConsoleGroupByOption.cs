using System;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public class ConsoleGroupByOption
    {
        public string Text { get; set; }
        public ConsoleColor Color { get; set; } = ConsoleColor.Cyan;
        public WrapAlignment Alignment { get; set; } = WrapAlignment.LEFT;
        public WrapSort GroupOrder { get; set; } = WrapSort.ASCENDING;
    }
}
