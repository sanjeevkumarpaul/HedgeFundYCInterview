using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public class ConsoleOutput
    {
        public ConsoleOutputStyle Style { get; set; } = ConsoleOutputStyle.CONSOLE;
        public string Path { get; set; } = "Output";
    }
}
