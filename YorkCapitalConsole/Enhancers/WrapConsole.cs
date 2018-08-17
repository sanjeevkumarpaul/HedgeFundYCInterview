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
    
    public enum WrapAlignment
    {
        LEFT,
        RIGHT,
        CENTER
    }

    public static partial class WrapConsole
    {
        public static void Write(this string str, WrapConsoleStyle style = WrapConsoleStyle.NORMAL, WrapAlignment align = WrapAlignment.LEFT)
        {
            WriteIt(str, style, false, align);
        }

        public static void WriteLine(this string str, WrapConsoleStyle style = WrapConsoleStyle.NORMAL, WrapAlignment align = WrapAlignment.LEFT)
        {
            WriteIt(str, style, align:align);
        }

        public static void WriteColor(this string str, ConsoleColor color, WrapAlignment align = WrapAlignment.LEFT)
        {
            WriteItColor(str, color, false, align);
        }

        public static void WriteLineColor(this string str, ConsoleColor color, WrapAlignment align = WrapAlignment.LEFT)
        {
            WriteItColor(str, color, align: align);
        }

        public static void PaintLineBackground(this ConsoleColor color, int lines=1)
        {
            PaintIt(" ", color, color, lines);
        }

        public static void PaintLine(this string str, ConsoleColor bgColor, ConsoleColor foreColor = ConsoleColor.White, WrapAlignment align = WrapAlignment.LEFT)
        {
            PaintIt(str, bgColor, foreColor, 1, align);
        }
        
        public static void WriteTable(ConsoleTable table)
        {
            PutTable(table);
        }
        
    }

    public partial class WrapConsole
    {
        private static void WriteOnConsole(bool linebreak, WrapAlignment align, string str, bool isfill = false)
        {
            if (!linebreak) Console.Write(SetAlignment(str, align, isfill)); else Console.WriteLine(SetAlignment(str, align, isfill));
        }

        private static void WriteIt(string str, WrapConsoleStyle style, bool linebreak = true, WrapAlignment align = WrapAlignment.LEFT)
        {
            SetColorStyle(style);            
            WriteOnConsole(linebreak, align, str);
            Console.ResetColor();
        }

        private static void WriteItColor(string str, ConsoleColor color, bool linebreak = true, WrapAlignment align = WrapAlignment.LEFT)
        {
            SetForeColor(color);
            WriteOnConsole(linebreak, align, str);
            Console.ResetColor();
        }

        private static void PaintIt(string str, ConsoleColor bgcolor, ConsoleColor forecolor, int lines, WrapAlignment align = WrapAlignment.LEFT)
        {
            SetColor(bgcolor, forecolor);
            for(int i = 0; i< lines; i++) WriteOnConsole(true, align, str, true); //Console.WriteLine(SetAlignment(str, align, true));
            Console.ResetColor();
        }

        private static void SetColorStyle(WrapConsoleStyle style)
        {
            switch (style)
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
            SetForeColor(fore);
        }

        private static void SetForeColor(ConsoleColor fore)
        {
            Console.ForegroundColor = fore;
        }

        private static string SetAlignment(string str, WrapAlignment align, bool isfill = false)
        {
            str = str.Trim();
            switch (align)
            {
                case WrapAlignment.LEFT: if (isfill) str = str.PadRight(Console.WindowWidth - 1); break;
                case WrapAlignment.RIGHT: str = str.PadLeft(Console.WindowWidth - 1); break;
                case WrapAlignment.CENTER: CalculateCenter(); break;
            }

            void CalculateCenter()
            {
                var len = (Console.WindowWidth - 1) - str.Length;
                var padlen = ( (int)len / 2 ) + str.Length;
                str = str.PadLeft(padlen);
            }


            return str;
        }
        
        private static void PutTable(ConsoleTable table)
        {
            #region ^Finding Column Width
            var max = table.ColumnOptions.Count();
            for (int i =0; i < max - 1; i++)
            {
                var _len = table.Rows.Select(R => R.Row[i]).Max(R => R.Text.Length);
                if (table.ColumnOptions[i].Width <= _len) table.ColumnOptions[i].Width = _len;
            }
            max = table.ColumnOptions.Sum(c => c.Width);
            #endregion ~Finding Column Width

            var separator = "-".Repeat(max + 5);
            
            Console.WriteLine();
            WriteItColor($"{separator}", ConsoleColor.Gray);

            foreach (var rows in table.Rows)
            {
                WriteItColor("|", ConsoleColor.Gray, false);
                int i = 0;
                rows.Row.ForEach(R => 
                {
                    var _option = table.ColumnOptions[i++];
                    var _color = R.Color == Console.BackgroundColor ? _option.Color : R.Color;

                    WriteItColor( $" { (R.Text).PadRight( _option.Width ) }", _color , false );
                    WriteItColor("|", ConsoleColor.Gray, false);                
                });
                Console.WriteLine();
                WriteItColor($"{separator}", ConsoleColor.Gray);
            }
        }
        
        /*
                WrapConsole.WriteTable(new ConsoleTable
                {
                    ColumnOptions = new List<ConsoleColumnOptions>
                    {
                        new ConsoleColumnOptions {Width = 35, Alignment = WrapAlignment.LEFT , Color = ConsoleColor.Yellow  },
                        new ConsoleColumnOptions {Width = 30, Alignment = WrapAlignment.LEFT , Color = ConsoleColor.White },
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
    }
}
