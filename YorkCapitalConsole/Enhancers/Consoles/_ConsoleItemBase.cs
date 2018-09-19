using DocumentFormat.OpenXml;
using System;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public partial class _ConsoleItemBase
    {
        public string Text { get; set; }
        public int Width { get; set; }
        public virtual ConsoleAlignment Alignment { get; set; } = ConsoleAlignment.LEFT; //CENTER not allowed. If set, LEFT will be considered.
        public virtual ConsoleColor Color { get; set; }
    }
    partial class _ConsoleItemBase
    {
        internal string HTMLCssClass { get; set; }
        internal string HTMLInlineStyles { get; set; }
        internal UInt32Value XLStyleIndex { get; set; }
    }
}
