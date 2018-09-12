using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public partial class ConsoleColumnOptions : _ConsoleItemBase
    {        
        public ConsoleAggregate Aggregate { get; set; } = ConsoleAggregate.NONE;
        public ConsoleWrapType Wrap { get; set; } = ConsoleWrapType.WRAP;
        public ConsoleGroupByOption Group { get; set; }
        public char WrapCharCharacter { get; set; } = '~';
    }
}
