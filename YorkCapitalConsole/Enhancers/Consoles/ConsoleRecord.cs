using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public partial class ConsoleRecord : _ConsoleItemBase
    {       
        internal bool IsAggregate { get; set; } = false;
        internal int Lines { get; set; } = 1;
        internal List<string> MText { get; set; } = new List<string>();
        internal ConsoleSortedValues SortedText { get; set; } = new ConsoleSortedValues();
    }
}
