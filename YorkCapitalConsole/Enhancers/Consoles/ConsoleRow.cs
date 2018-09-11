using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrappers.Consoles
{
    public partial class ConsoleRow : _ConsoleItemBase
    {
        public List<ConsoleRecord> Column { get; set; }
        internal bool IsAggregate { get; set; } = false;
        internal bool IsLastRow { get; set; } = false;        
    }
}
