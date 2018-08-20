using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrappers.Consoles
{
    public class ConsoleRecord
    {
        public string Text { get; set; }
        public ConsoleColor Color { get; set; }
        internal bool IsAggregate { get; set; } = false;
    }
}
