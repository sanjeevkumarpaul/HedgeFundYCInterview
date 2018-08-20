using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrappers.Consoles
{
    public class ConsoleOtherOptions
    {
        public ConsoleColor AggregateColor { get; set; } = ConsoleColor.White;
        public ConsoleColor AggregateBorderColor { get; set; } = ConsoleColor.DarkGreen;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.Gray;
        public char BorderChar { get; set; } = '-';

        internal bool IsAggregateRowExists { get; set; } = false;
    }
}
