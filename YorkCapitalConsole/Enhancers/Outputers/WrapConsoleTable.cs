using System;
using System.Collections.Generic;
using System.Linq;

using Wrappers.Consoles;
using Wrappers.Consoles.Enums;
using Extensions;

namespace Wrappers.Outputers
{
    public class WrapConsoleTable
    {
        public void PutTable(ConsoleTable table)
        {
            var max = table.ColumnOptions.Sum(c => c.Width);
            if (!ExternalOutput(table))
            {
                var separator = table.OtherOptions.BorderChar.ToString().Repeat(max + (table.ColumnOptions.Count() * 2) + 1);

                Console.WriteLine();
                DrawHeaderFooter(table, separator);

                int col = 0;
                foreach (var rows in table.Rows)
                {
                    var _borderColor = (rows.IsAggregate || (rows.IsLastRow && table.OtherOptions.IsAggregateRowExists)) ?
                                                table.OtherOptions.AggregateBorderColor : table.OtherOptions.BorderColor;
                    var _borderBarColor = rows.IsAggregate ? _borderColor : table.OtherOptions.BorderColor;

                    var mLines = rows.Column.Max(c => c.Lines) - 1;
                    for (int mindex = 0; mindex <= mLines; mindex++)
                    {
                        int i = 0;
                        WrapConsole.WriteColor("|", _borderBarColor);
                        rows.Column.ForEach(C =>
                        {
                            var _option = table.ColumnOptions[i++];
                            var _alText = C.Text;

                        //Wrapping requirement.
                        if ((_alText.Empty() || mindex > 0) && C.MText.Any() && C.MText.Count > mindex)
                                _alText = C.MText.ElementAt(mindex);
                            else if (!_alText.Empty() && mindex > 0)
                                _alText = "";

                        //Making sure of alignment
                        _alText = _option.Alignment == ConsoleAlignment.LEFT ?
                                                (_alText).PadRight(_option.Width) :
                                                _option.Alignment == ConsoleAlignment.RIGHT ? (_alText).PadLeft(_option.Width) : _alText.CenteredText(_option.Width);

                            var _color = C.IsAggregate ? table.OtherOptions.AggregateColor :
                                                (col == 0 && table.OtherOptions.IsFirstRowAsHeader ? table.OtherOptions.HeadingColor : _option.Color);
                            var _prefix = _option.Alignment != ConsoleAlignment.RIGHT ? " " : ""; //Left space
                        var _postfix = _option.Alignment == ConsoleAlignment.RIGHT ? " " : "";//Right space

                        WrapConsole.WriteColor($"{_prefix}{ _alText }{_postfix}", _color);

                            if (_option.Alignment == ConsoleAlignment.CENTER)
                                WrapConsole.WriteColor($"{("|".PadLeft(_option.Width - _alText.Length + 1))}", _borderBarColor);
                            else
                                WrapConsole.WriteColor("|", _borderBarColor);
                        });
                        Console.WriteLine();
                    }
                    WrapConsole.WriteLineColor($"{separator}", _borderColor);
                    col++;
                }

                DrawHeaderFooter(table, separator, false);
            }
        }

        private bool ExternalOutput(ConsoleTable table)
        {
            List<ConsoleTable> tabs = new List<ConsoleTable> { table, table };

            switch(table.OtherOptions.Output.Style)
            {
                case ConsoleOutputType.CONSOLE: return false;
                case ConsoleOutputType.HTM:
                case ConsoleOutputType.HTML: new WrapHtmlTable(table).Draw(); break;
                case ConsoleOutputType.XL:
                case ConsoleOutputType.XLS:
                case ConsoleOutputType.XSLX:
                case ConsoleOutputType.EXCEL: break;
                case ConsoleOutputType.TXT:
                case ConsoleOutputType.TEXT: new WrapTextTable(table).Draw(); break;
                case ConsoleOutputType.CSV: new WrapCSVTable(table).Draw(); break;
                case ConsoleOutputType.JSON: new WrapJsonTable(table).Draw(); break;
                case ConsoleOutputType.XML: new WrapXmlTable(tabs).Draw(); break;
            }

            return true;
        }

        /// <summary>
        /// Drawing Header and Footers for the console table with LEFT/RIGHT Alignment
        /// CENTER alignment is considered to be LEFT at this moment.
        /// </summary>
        /// <param name="separator">Total Width Bordered Text (helps to calculate the header/footer width)</param>        
        private void DrawHeaderFooter(ConsoleTable table, string separator, bool header = true)
        {
            var _rows = header ? table.Headers : table.Footers;

            if (header)
                WrapConsole.WriteLineColor($"{separator}", table.OtherOptions.BorderColor);

            if (_rows != null && _rows.Any())
            {
                var _leftWidth = _rows.Max(r => r.Heading.Text.Length) + 2;
                int _colonWidth = 3;
                var _rightWidth = separator.Length - (_leftWidth + _colonWidth + 3);
                var _textWidth = _rows.Max(r => r.Value.Text.Length);

                foreach(var row in _rows)
                {
                    WrapConsole.WriteColor($"|", table.OtherOptions.BorderColor);

                    if (row.Alignment == ConsoleAlignment.LEFT || row.Alignment == ConsoleAlignment.CENTER)
                    {
                        WrapConsole.WriteColor($" { row.Heading.Text }", row.Heading.Color);
                        WrapConsole.WriteColor(" ".PadLeft(_leftWidth - row.Heading.Text.Length), table.OtherOptions.BorderColor);
                        WrapConsole.WriteColor(" : ", row.Heading.Color);
                        WrapConsole.WriteColor($" {row.Value.Text}", row.Value.Color);
                        WrapConsole.WriteLineColor("|".PadLeft(_rightWidth - row.Value.Text.Length), table.OtherOptions.BorderColor);
                    }
                    else
                    {
                        WrapConsole.WriteColor(" ".PadLeft(separator.Length - (_leftWidth + _colonWidth + _textWidth + 2)), table.OtherOptions.BorderColor);
                        WrapConsole.WriteColor($"{ row.Heading.Text }", row.Heading.Color);
                        WrapConsole.WriteColor(" ".PadLeft(_leftWidth - row.Heading.Text.Length), table.OtherOptions.BorderColor);
                        WrapConsole.WriteColor(" : ", row.Heading.Color);
                        WrapConsole.WriteColor($"{row.Value.Text}", row.Value.Color);
                        if (_textWidth - row.Value.Text.Length > 0)
                            WrapConsole.WriteColor(" ".PadLeft(_textWidth - row.Value.Text.Length ), table.OtherOptions.BorderColor);
                        WrapConsole.WriteLineColor("|", table.OtherOptions.BorderColor);
                    }
                }

                WrapConsole.WriteLineColor($"{separator}", table.OtherOptions.BorderColor);
            }
        }        
    }
}
