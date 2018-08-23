using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrappers.Consoles
{
    public class ConsoleTable
    {
        public ConsoleOtherOptions OtherOptions { get; set; } = new ConsoleOtherOptions();
        public List<ConsoleColumnOptions> ColumnOptions { get; set; }
        public List<ConsoleRow> Rows { get; set; }                
        public List<ConsoleHeaderFooterRow> Headers { get; set; }
        public List<ConsoleHeaderFooterRow> Footers { get; set; }
    }
}
