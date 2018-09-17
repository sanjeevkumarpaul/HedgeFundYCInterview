using System;
using System.Collections.Generic;
using System.Linq;

using Wrappers.Consoles;
using Wrappers.Consoles.Enums;
using Extensions;
using Wrappers.Outputers.Base;

namespace Wrappers.Outputers
{
    public class WrapConsoleTable : _BaseOutputTable
    {
        public WrapConsoleTable(WrapOutputerOptions options) : base(options) { }
        public WrapConsoleTable(ConsoleTable table, WrapOutputerOptions options) : base(table, options) { }
        public WrapConsoleTable(List<ConsoleTable> tables, WrapOutputerOptions options) : base(tables, options) { }

        protected override void Init() { }
        protected override void Start() { }
        protected override void Finish() { }
        protected override void PutTable()        
        {
            var max = _table.ColumnOptions.Sum(c => c.Width);
            var separator = _table.OtherOptions.BorderChar.ToString().Repeat(max + (_table.ColumnOptions.Count() * 2) + 1);

            Console.WriteLine();
            DrawHeaderFooter(_table, separator);

            int col = 0;
            foreach (var rows in _table.Rows)
            {
                var _borderColor = (rows.IsAggregate || (rows.IsLastRow && _table.OtherOptions.IsAggregateRowExists)) ?
                                            _table.OtherOptions.AggregateBorderColor : _table.OtherOptions.BorderColor;
                var _borderBarColor = rows.IsAggregate ? _borderColor : _table.OtherOptions.BorderColor;

                var mLines = rows.Column.Max(c => c.Lines) - 1;
                for (int mindex = 0; mindex <= mLines; mindex++)
                {
                    int i = 0;
                    WrapConsole.WriteColor("|", _borderBarColor);
                    rows.Column.ForEach(C =>
                    {
                        var _option = _table.ColumnOptions[i++];
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

                        var _color = C.IsAggregate ? _table.OtherOptions.AggregateColor :
                                            (col == 0 && _table.OtherOptions.IsFirstRowAsHeader ? _table.OtherOptions.HeadingColor : _option.Color);
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

            DrawHeaderFooter(_table, separator, false);
        }

        /// <summary>
        /// Drawing Header and Footers for the console table with LEFT/RIGHT Alignment
        /// CENTER alignment is considered to be LEFT at this moment.
        /// </summary>
        /// <param name="separator">Total Width Bordered Text (helps to calculate the header/footer width)</param>        
        private void DrawHeaderFooter(ConsoleTable table, string separator, bool header = true)
        {
            var _rows = header ? _table.Headers : _table.Footers;

            if (header)
                WrapConsole.WriteLineColor($"{separator}", _table.OtherOptions.BorderColor);

            if (_rows != null && _rows.Any())
            {
                var _leftWidth = _rows.Max(r => r.Heading.Text.Length) + 2;
                int _colonWidth = 3;
                var _rightWidth = separator.Length - (_leftWidth + _colonWidth + 3);
                var _textWidth = _rows.Max(r => r.Value.Text.Length);

                foreach(var row in _rows)
                {
                    WrapConsole.WriteColor($"|", _table.OtherOptions.BorderColor);

                    if (row.Alignment == ConsoleAlignment.LEFT || row.Alignment == ConsoleAlignment.CENTER)
                    {
                        WrapConsole.WriteColor($" { row.Heading.Text }", row.Heading.Color);
                        WrapConsole.WriteColor(" ".PadLeft(_leftWidth - row.Heading.Text.Length), _table.OtherOptions.BorderColor);
                        WrapConsole.WriteColor(" : ", row.Heading.Color);
                        WrapConsole.WriteColor($" {row.Value.Text}", row.Value.Color);
                        WrapConsole.WriteLineColor("|".PadLeft(_rightWidth - row.Value.Text.Length), _table.OtherOptions.BorderColor);
                    }
                    else
                    {
                        WrapConsole.WriteColor(" ".PadLeft(separator.Length - (_leftWidth + _colonWidth + _textWidth + 2)), _table.OtherOptions.BorderColor);
                        WrapConsole.WriteColor($"{ row.Heading.Text }", row.Heading.Color);
                        WrapConsole.WriteColor(" ".PadLeft(_leftWidth - row.Heading.Text.Length), _table.OtherOptions.BorderColor);
                        WrapConsole.WriteColor(" : ", row.Heading.Color);
                        WrapConsole.WriteColor($"{row.Value.Text}", row.Value.Color);
                        if (_textWidth - row.Value.Text.Length > 0)
                            WrapConsole.WriteColor(" ".PadLeft(_textWidth - row.Value.Text.Length ), _table.OtherOptions.BorderColor);
                        WrapConsole.WriteLineColor("|", _table.OtherOptions.BorderColor);
                    }
                }

                WrapConsole.WriteLineColor($"{separator}", _table.OtherOptions.BorderColor);
            }
        }        
    }
}
