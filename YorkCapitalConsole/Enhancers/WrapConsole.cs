using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrappers
{
    public enum WrapConsoleStyle
    {
        NORMAL,
        SUCCESS,
        FAILURE,
        WARNING,
        COMPLETE,
        INFORMATION,
        SATISFACTORY,
        INVERSE,
    }

    public static partial class WrapConsole
    {
        public static void Write(this string str, WrapConsoleStyle style = WrapConsoleStyle.NORMAL)
        {
            WriteIt(str, style, false);
        }

        public static void WriteLine(this string str, WrapConsoleStyle style = WrapConsoleStyle.NORMAL)
        {
            WriteIt(str, style);
        }
    }

    public partial class WrapConsole
    {
        private static void WriteIt(string str, WrapConsoleStyle style, bool linebreak = true)
        {
            SetColorStyle(style);
            if (!linebreak) Console.Write(str); else Console.WriteLine(str);
            Console.ResetColor();
        }

        private static void SetColorStyle(WrapConsoleStyle style)
        {
            switch(style)
            {
                case WrapConsoleStyle.NORMAL: Console.ResetColor(); break;
                case WrapConsoleStyle.SUCCESS: SetColor(ConsoleColor.DarkGreen, ConsoleColor.White); break;
                case WrapConsoleStyle.FAILURE: SetColor(ConsoleColor.DarkRed, ConsoleColor.White); break;
                case WrapConsoleStyle.WARNING: SetColor(ConsoleColor.Red, ConsoleColor.White); break;
                case WrapConsoleStyle.COMPLETE: SetColor(ConsoleColor.Blue, ConsoleColor.White); break;
                case WrapConsoleStyle.INFORMATION: SetColor(ConsoleColor.DarkCyan, ConsoleColor.White); break;
                case WrapConsoleStyle.SATISFACTORY: SetColor(ConsoleColor.DarkGray, ConsoleColor.White); break;
                case WrapConsoleStyle.INVERSE: SetColor(ConsoleColor.White, ConsoleColor.Gray); break;
            }
        }

        private static void SetColor(ConsoleColor back, ConsoleColor fore)
        {
            Console.BackgroundColor = back;
            Console.ForegroundColor = fore;
        }
    }
}
