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
        public ConsoleOutputType Style { get; set; } = ConsoleOutputType.CONSOLE;
        public string Path { get; set; } = "Output";
    }
}
