using System;

using Wrappers.Consoles;
using Wrappers.Consoles.Enums;
using Wrappers.Outputers;
using Extensions;

namespace Wrappers
{
    /*
                WrapConsole.WriteTable(new ConsoleTable
                {
                    ColumnOptions = new List<ConsoleColumnOptions>
                    {
                        new ConsoleColumnOptions {Width = 35, Alignment = WrapAlignment.LEFT , Color = ConsoleColor.Yellow  },
                        new ConsoleColumnOptions {Width = 30, Alignment = WrapAlignment.CENTER , Color = ConsoleColor.White },
                    },

                    Rows = new List<ConsoleRow>
                    {
                         new ConsoleRow
                         {
                             Row = new List<ConsoleRecord>
                             {
                                 new ConsoleRecord { Text = "Name", Color = ConsoleColor.DarkGray },
                                 new ConsoleRecord { Text = "Display Name", Color = ConsoleColor.DarkGray},
                             }
                         },
                         new ConsoleRow
                         {
                             Row = new List<ConsoleRecord>
                             {
                                 new ConsoleRecord { Text = $"{result.Info.SurName}, {result.Info.GivenName}"},
                                 new ConsoleRecord { Text = $"{result.Info.DisplayName}" },
                             }
                         }
                }   });
        */

    public static partial class WrapConsole
    {
        public static void Write(this string str, ConsoleTextStyle style = ConsoleTextStyle.NORMAL, ConsoleAlignment align = ConsoleAlignment.LEFT)
        {
            WriteIt(str, style, false, align);
        }

        public static void WriteLine(this string str, ConsoleTextStyle style = ConsoleTextStyle.NORMAL, ConsoleAlignment align = ConsoleAlignment.LEFT)
        {
            WriteIt(str, style, align:align);
        }

        public static void WriteColor(this string str, ConsoleColor color, ConsoleAlignment align = ConsoleAlignment.LEFT)
        {
            WriteItColor(str, color, false, align);
        }

        public static void WriteLineColor(this string str, ConsoleColor color, ConsoleAlignment align = ConsoleAlignment.LEFT)
        {
            WriteItColor(str, color, align: align);
        }

        public static void PaintLineBackground(this ConsoleColor color, int lines=1)
        {
            PaintIt(" ", color, color, lines);
        }

        public static void PaintLine(this string str, ConsoleColor bgColor, ConsoleColor foreColor = ConsoleColor.White, ConsoleAlignment align = ConsoleAlignment.LEFT)
        {
            PaintIt(str, bgColor, foreColor, 1, align);
        }
        
        public static void WriteTable(ConsoleTable table)
        {
            WrapOutputerRadar.CalculateBoundaries(table);
            new WrapConsoleTable().PutTable(table);
        }
        
    }

    public partial class WrapConsole
    {
        private static void WriteOnConsole(bool linebreak, ConsoleAlignment align, string str, bool isfill = false)
        {
            if (!linebreak) Console.Write(SetAlignment(str, align, isfill)); else Console.WriteLine(SetAlignment(str, align, isfill));
        }

        private static void WriteIt(string str, ConsoleTextStyle style, bool linebreak = true, ConsoleAlignment align = ConsoleAlignment.LEFT)
        {
            SetColorStyle(style);            
            WriteOnConsole(linebreak, align, str);
            Console.ResetColor();
        }

        private static void WriteItColor(string str, ConsoleColor color, bool linebreak = true, ConsoleAlignment align = ConsoleAlignment.LEFT)
        {
            SetForeColor(color);
            WriteOnConsole(linebreak, align, str);
            Console.ResetColor();
        }

        private static void PaintIt(string str, ConsoleColor bgcolor, ConsoleColor forecolor, int lines, ConsoleAlignment align = ConsoleAlignment.LEFT)
        {
            SetColor(bgcolor, forecolor);
            for(int i = 0; i< lines; i++) WriteOnConsole(true, align, str, true); //Console.WriteLine(SetAlignment(str, align, true));
            Console.ResetColor();
        }

        private static void SetColorStyle(ConsoleTextStyle style)
        {
            switch (style)
            {
                case ConsoleTextStyle.NORMAL: Console.ResetColor(); break;
                case ConsoleTextStyle.SUCCESS: SetColor(ConsoleColor.DarkGreen, ConsoleColor.White); break;
                case ConsoleTextStyle.FAILURE: SetColor(ConsoleColor.DarkRed, ConsoleColor.White); break;
                case ConsoleTextStyle.WARNING: SetColor(ConsoleColor.Red, ConsoleColor.White); break;
                case ConsoleTextStyle.COMPLETE: SetColor(ConsoleColor.Blue, ConsoleColor.White); break;
                case ConsoleTextStyle.INFORMATION: SetColor(ConsoleColor.DarkCyan, ConsoleColor.White); break;
                case ConsoleTextStyle.SATISFACTORY: SetColor(ConsoleColor.DarkGray, ConsoleColor.White); break;
                case ConsoleTextStyle.INVERSE: SetColor(ConsoleColor.White, ConsoleColor.Gray); break;
            }
        }

        private static void SetColor(ConsoleColor back, ConsoleColor fore)
        {
            Console.BackgroundColor = back;
            SetForeColor(fore);
        }

        private static void SetForeColor(ConsoleColor fore)
        {
            Console.ForegroundColor = fore;
        }

        private static string SetAlignment(string str, ConsoleAlignment align, bool isfill = false)
        {            
            switch (align)
            {
                case ConsoleAlignment.LEFT: if (isfill) str = str.PadRight(Console.WindowWidth - 1); break;
                case ConsoleAlignment.RIGHT: str = str.PadLeft(Console.WindowWidth - 1); break;
                case ConsoleAlignment.CENTER: str = str.CenteredText(Console.WindowWidth - 1); break;
            }
            return str;
        }

        
    }
} 
