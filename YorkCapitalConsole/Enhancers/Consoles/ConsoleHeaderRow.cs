using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public class ConsoleHeaderFooterRow
    {
        public string Heading { get; set; }
        public string Value { get; set; }
        public ConsoleColor HeadingColor { get; set; } = ConsoleColor.DarkYellow;
        public ConsoleColor ValueColor { get; set; } = ConsoleColor.DarkCyan;
        public ConsoleAlignment Alignment { get; set; } = ConsoleAlignment.LEFT; //CENTER not allowed. If set, LEFT will be considered.
    }
}
