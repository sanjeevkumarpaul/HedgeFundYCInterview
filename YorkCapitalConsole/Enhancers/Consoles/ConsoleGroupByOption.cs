using System;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public class ConsoleGroupByOption
    {
        public string Text { get; set; }
        public ConsoleColor Color { get; set; } = ConsoleColor.Cyan;
        public ConsoleAlignment Alignment { get; set; } = ConsoleAlignment.LEFT;
        public ConsoleSortType GroupOrder { get; set; } = Enums.ConsoleSortType.ASCENDING;
    }
}
