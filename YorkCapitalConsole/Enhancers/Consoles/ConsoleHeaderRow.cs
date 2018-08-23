using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrappers.Consoles
{
    public class ConsoleHeaderFooterRow
    {
        public string StaticText { get; set; }
        public string StaticValue { get; set; }
        public ConsoleColor TextColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor ValueColor { get; set; } = ConsoleColor.White;
    }
}
