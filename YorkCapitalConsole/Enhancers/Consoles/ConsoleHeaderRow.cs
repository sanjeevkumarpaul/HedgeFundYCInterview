using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public partial class ConsoleHeaderFooterRow : _ConsoleItemBase
    {
        public string Heading { get; set; }
        public string Value { get; set; }
        public ConsoleColor HeadingColor { get; set; } = ConsoleColor.DarkYellow;
        public override ConsoleColor Color { get; set; } = ConsoleColor.DarkCyan;        
    }
}
