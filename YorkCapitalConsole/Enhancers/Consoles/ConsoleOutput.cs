using Extensions;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public class ConsoleOutput
    {
        private ConsoleOutputType _style;

        public ConsoleOutput()
        {
            Style = ConsoleOutputType.CONSOLE;
        }

        public ConsoleOutputType Style { get { return _style; } set { _style = value; Extension = _style.Description(); } }
        public string Path { get; set; } = "Output";
        internal string Extension { get; private set; } = "txt";
    }
}
