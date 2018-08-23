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
        public ConsoleColor HeadingColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor ValueColor { get; set; } = ConsoleColor.White;
        public WrapAlignment Alignment { get; set; } = WrapAlignment.LEFT; //CENTER not allowed. If set, LEFT will be considered.
    }
}
