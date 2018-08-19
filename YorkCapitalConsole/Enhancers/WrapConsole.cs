﻿using System;
using System.Collections.Generic;
using System.Linq;

using Extensions;
using Wrappers.Consoles;
using Wrappers.Consoles.Enums;

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
            switch (align)
            {
                case WrapAlignment.LEFT: if (isfill) str = str.PadRight(Console.WindowWidth - 1); break;
                case WrapAlignment.RIGHT: str = str.PadLeft(Console.WindowWidth - 1); break;
                case WrapAlignment.CENTER: str = CenteredText(str, Console.WindowWidth - 1); break;
            }
            return str;
        }

        private static void PutTable(ConsoleTable table)
        {
            table = CalculateAggregation(table);

            #region ^Finding Column Width
            var max = table.ColumnOptions.Count();
            for (int i =0; i < max - 1; i++)
            {
                var _len = table.Rows.Select(R => R.Column[i]).Max(R => R.Text.Length);
                if (table.ColumnOptions[i].Width <= _len) table.ColumnOptions[i].Width = _len;
            }
            max = table.ColumnOptions.Sum(c => c.Width);
            #endregion ~Finding Column Width

            var separator = "-".Repeat(max + ( table.ColumnOptions.Count() * 2 ) + 1);
            
            Console.WriteLine();
            WriteItColor($"{separator}", ConsoleColor.Gray);

            foreach (var rows in table.Rows)
            {
                WriteItColor("|", ConsoleColor.Gray, false);
                int i = 0;
                rows.Column.ForEach(R => 
                {
                    var _option = table.ColumnOptions[i++];
                    var _color = R.Color == Console.BackgroundColor ? _option.Color : R.Color;
                    var _alText = _option.Alignment == WrapAlignment.LEFT ? 
                                        (R.Text).PadRight(_option.Width) :
                                        _option.Alignment == WrapAlignment.RIGHT ? (R.Text).PadLeft(_option.Width) : CenteredText(R.Text, _option.Width);
                    var _prefix = _option.Alignment != WrapAlignment.RIGHT ? " " : ""; //Left space
                    var _postfix = _option.Alignment == WrapAlignment.RIGHT ? " " : "";//Right space

                    WriteItColor( $"{_prefix}{ _alText }{_postfix}", _color , false );

                    if (_option.Alignment == WrapAlignment.CENTER)
                        WriteItColor($"{("|".PadLeft(_option.Width - _alText.Length + 1))}", ConsoleColor.Gray, false);
                    else
                        WriteItColor("|", ConsoleColor.Gray, false);                
                });
                Console.WriteLine();
                WriteItColor($"{separator}", ConsoleColor.Gray);
            }            
        }

        /// <summary>
        /// Aligns a text center to any width.
        /// Obvious reason being Text should be less than the Width specified.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private static string CenteredText(string text, int width)
        {
            var len = width - text.Length;
            var padlen = ((int)len / 2) + text.Length;
            return text.PadLeft(padlen);
        }

        private static ConsoleTable CalculateAggregation(ConsoleTable table)
        {
            List<ConsoleRecord> Agg = new List<ConsoleRecord>();

            int i = 0;
            foreach (var option in table.ColumnOptions)
            {
                Agg.Add(new ConsoleRecord
                {
                    Text = option.Aggregate == WrapAggregate.NONE ? "" : Aggregate(i, option)
                });
                i++;
            }
            if(Agg.Any(a => !a.Text.Empty()))                
                table.Rows.Add(new ConsoleRow { Column = Agg });
            
            string Aggregate(int colIndex, ConsoleColumnOptions option)
            {
                var vals = table.Rows.Select(r => r.Column[colIndex]).Select(c => c.Text.ToDouble());

                switch(option.Aggregate)
                {
                    case WrapAggregate.NONE: return "";
                    case WrapAggregate.SUM: return vals.Sum(d => d).ToString("(Sum) #.00");
                    case WrapAggregate.AVERAGE: return vals.Average(d => d).ToString("(Average) #.00");
                    case WrapAggregate.MEDIAN: return Median(vals).ToString("(Median) #.00");
                }

                return "";
            }

            double Median(IEnumerable<double> nums)
            {
                var _nums = nums.OrderBy(r => r);
                int _prev = nums.Count() - 1;
                int _next = 1;
                bool _doubleflag = false;

                while(true)
                {
                    if (_prev == _next) break;
                    if (_prev - 1 == _next && _next + 1 == _prev) { _doubleflag = true; break; }
                    _next++; _prev--;
                }

                return _doubleflag ? (_nums.ElementAt(_prev)+ _nums.ElementAt(_next)) / 2 : _nums.ElementAt(_prev);
            }

            return table;
        }        
    }
} 