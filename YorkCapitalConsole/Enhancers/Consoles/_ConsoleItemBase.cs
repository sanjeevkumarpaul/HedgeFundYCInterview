using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public partial class _ConsoleItemBase
    {
        public int Width { get; set; }
        public virtual ConsoleAlignment Alignment { get; set; } = ConsoleAlignment.LEFT; //CENTER not allowed. If set, LEFT will be considered.
        public virtual ConsoleColor Color { get; set; }
    }
    partial class _ConsoleItemBase
    {
        internal string HTMLCssClass { get; set; }
        internal string HTMLInlineStyles { get; set; }
    }
}
