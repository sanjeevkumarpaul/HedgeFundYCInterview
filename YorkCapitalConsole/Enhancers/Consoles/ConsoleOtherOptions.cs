using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrappers.Consoles
{
    public class ConsoleOtherOptions
    {
        public bool IsFirstRowAsHeader { get; set; } = true;
        public ConsoleColor HeadingColor { get; set; } = ConsoleColor.White;
        public ConsoleColor AggregateColor { get; set; } = ConsoleColor.White;
        public ConsoleColor AggregateBorderColor { get; set; } = ConsoleColor.DarkGreen;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.Gray;
        public char BorderChar { get; set; } = '-';
        internal bool IsAggregateRowExists { get; set; } = false;
        public ConsoleSort Sort { get; set; } = new ConsoleSort();       
    }
}
