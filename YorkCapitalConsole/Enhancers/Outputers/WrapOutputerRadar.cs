using System;
using System.Collections.Generic;
using System.Linq;
using Wrappers.Consoles;
using Wrappers.Consoles.Enums;
using Extensions;
using Wrappers.Outputers.Interface;

namespace Wrappers.Outputers
{
    public partial class WrapOutputerRadar
    {
        #region ^Public Methods
        public static bool Output(ConsoleTable table, WrapOutputerOptions options)
        {
            return Output(new List<ConsoleTable> { table }, options);
        }
        public static bool Output(List<ConsoleTable> tables, WrapOutputerOptions options)
        {
            var _output = OutputFactory(tables, options);
            Func<IOutputTable, bool> _func = (IOut) => { IOut.Draw(); return true; };

            return _output == null ? false : _func(_output);
        }        
        public static IOutputTable OutputFactory(ConsoleTable table, WrapOutputerOptions options)
        {
            return OutputFactory(new List<ConsoleTable> { table }, options);
        }
        public static IOutputTable OutputFactory(List<ConsoleTable> tables, WrapOutputerOptions options)
        {
            switch (options.Output.Style)
            {
                case ConsoleOutputType.CONSOLE: return new WrapConsoleTable(tables, options);
                case ConsoleOutputType.HTM:
                case ConsoleOutputType.HTML: return new WrapHtmlTable(tables, options);
                case ConsoleOutputType.XL:
                case ConsoleOutputType.XLS:
                case ConsoleOutputType.XSLX:
                case ConsoleOutputType.EXCEL: return new WrapExcelTable(tables, options);
                case ConsoleOutputType.TXT:
                case ConsoleOutputType.TEXT: return new WrapTextTable(tables, options);
                case ConsoleOutputType.CSV: return new WrapCSVTable(tables, options);
                case ConsoleOutputType.JSON: return new WrapJsonTable(tables, options);
                case ConsoleOutputType.XML: return new WrapXmlTable(tables, options);
            }

            return null;
        }
        #endregion ~Public Methods

        #region ^Internal Methods
        internal static void CalculateBoundaries(ConsoleTable table)
        {
            if (!table.BoundariesCalculate)
            {
                table = Sort(table);
                table = CalculateAggregation(table);

                #region ^Finding Column Width
                var max = table.ColumnOptions.Count();
                for (int i = 0; i < max; i++)
                {
                    Wrap(table, i);

                    var _len = table.Rows.Select(R => R.Column[i]).Max(R => (R.Text.Empty() ? R.Lines : R.Text.Length));
                    if (table.ColumnOptions[i].Width <= _len) table.ColumnOptions[i].Width = _len;
                }
                table.BoundariesCalculate = true;
            }
            #endregion ~Finding Column Width
        }
        #endregion ~Internal Methods

        /// <summary>
        /// sorting the whole table before presenting
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static ConsoleTable Sort(ConsoleTable table)
        {
            if (table.OtherOptions.Sort.SortColumnIndex >= 0)
            {
                var header = table.Rows.ElementAt(0);
                table.Rows.RemoveAt(0);

                table.Rows.ForEach(r => Convert(r.Column.ElementAt(table.OtherOptions.Sort.SortColumnIndex), table.OtherOptions.Sort.DataType));

                List<ConsoleRow> _rows = table.OtherOptions.Sort.SortType == Consoles.Enums.ConsoleSortType.ASCENDING ?
                                               Ascending(table.OtherOptions.Sort.SortColumnIndex, table.OtherOptions.Sort.DataType) :
                                               Descending(table.OtherOptions.Sort.SortColumnIndex, table.OtherOptions.Sort.DataType);

                _rows.Insert(0, header);
                table.Rows.Clear();
                table.Rows.AddRange(_rows);
            }

            void Convert(ConsoleRecord col, ConsoleSortDataType sortDataType)
            {
                switch (sortDataType)
                {
                    case ConsoleSortDataType.STRING: col.SortedText.StringText = col.Text; break;
                    case ConsoleSortDataType.NUMBER: col.SortedText.NumericText = col.Text.ToDouble(); break;
                    case ConsoleSortDataType.DATETIME: col.SortedText.DateText = col.Text.ToDateTime(); break;
                }
            }

            List<ConsoleRow> Ascending(int colindex, ConsoleSortDataType sortDataType)
            {
                switch (sortDataType)
                {
                    case ConsoleSortDataType.STRING: return table.Rows.OrderBy(d => d.Column.ElementAt(colindex).SortedText.StringText).ToList();
                    case ConsoleSortDataType.NUMBER: return table.Rows.OrderBy(d => d.Column.ElementAt(colindex).SortedText.NumericText).ToList();
                    case ConsoleSortDataType.DATETIME: return table.Rows.OrderBy(d => d.Column.ElementAt(colindex).SortedText.DateText).ToList();
                    default: return table.Rows;
                }
            }

            List<ConsoleRow> Descending(int colindex, ConsoleSortDataType sortDataType)
            {
                switch (sortDataType)
                {
                    case ConsoleSortDataType.STRING: return table.Rows.OrderByDescending(d => d.Column.ElementAt(colindex).SortedText.StringText).ToList();
                    case ConsoleSortDataType.NUMBER: return table.Rows.OrderByDescending(d => d.Column.ElementAt(colindex).SortedText.NumericText).ToList();
                    case ConsoleSortDataType.DATETIME: return table.Rows.OrderByDescending(d => d.Column.ElementAt(colindex).SortedText.DateText).ToList();
                    default: return table.Rows;
                }
            }


            return table;
        }
        private static ConsoleTable CalculateAggregation(ConsoleTable table)
        {
            List<ConsoleRecord> Agg = new List<ConsoleRecord>();

            int i = 0;
            foreach (var option in table.ColumnOptions)
            {
                Agg.Add(new ConsoleRecord
                {
                    Text = option.Aggregate == ConsoleAggregate.NONE ? "" : Aggregate(i, option),
                    IsAggregate = true
                });
                i++;
            }
            table.Rows.Last().IsLastRow = true;
            if (Agg.Any(a => !a.Text.Empty()))
            {
                table.Rows.Add(new ConsoleRow { Column = Agg, IsAggregate = true });
                table.OtherOptions.IsAggregateRowExists = true;
            }


            string Aggregate(int colIndex, ConsoleColumnOptions option)
            {
                var vals = table.Rows.Select(r => r.Column[colIndex]).Select(c => c.Text.ToDouble());
                double cal = 0.0d;

                switch (option.Aggregate)
                {
                    case ConsoleAggregate.NONE: break;
                    case ConsoleAggregate.SUM: cal = vals.Sum(d => d); break;
                    case ConsoleAggregate.AVERAGE: cal = vals.Average(d => d); break;
                    case ConsoleAggregate.MEDIAN: cal = Median(vals); break;
                }

                return cal > 0.0d ? cal.ToString($"({option.Aggregate.ToString()}) #.00") : "";
            }

            double Median(IEnumerable<double> nums)
            {
                var _nums = nums.OrderBy(r => r);
                int _prev = nums.Count() - 1;
                int _next = 1; //always start from 2nd row, as 1st row is headers.
                bool _doubleflag = false;

                while (true)
                {
                    if (_prev == _next) break;
                    if (_prev - 1 == _next && _next + 1 == _prev) { _doubleflag = true; break; }
                    _next++; _prev--;
                }

                return _doubleflag ? (_nums.ElementAt(_prev) + _nums.ElementAt(_next)) / 2 : _nums.ElementAt(_prev);
            }

            return table;
        }
        /// <summary>
        /// wrap up the whole text in a colum and arrange it back to column record.
        /// </summary>      
        private static void Wrap(ConsoleTable table, int colIndex)
        {
            var _option = table.ColumnOptions.ElementAt(colIndex);
            table.Rows.ForEach(r =>
            {
                var _col = r.Column.ElementAt(colIndex);
                if ((_col.Text.Length > _option.Width && _option.Wrap != ConsoleWrapType.NOWRAP) || (_option.Wrap == ConsoleWrapType.WORDCHAR))
                {
                    switch (_option.Wrap)
                    {
                        case ConsoleWrapType.ELLIPSES: _col.Text = $"{_col.Text.Substring(0, _option.Width - 3)}..."; break;
                        case ConsoleWrapType.REMOVE: _col.Text = $"{_col.Text.Substring(0, _option.Width - 3)}"; break;
                        case ConsoleWrapType.WRAP: WrapAround(_col); break;
                        case ConsoleWrapType.WORDWRAP: WrapWordAround(_col); break;
                        case ConsoleWrapType.WORDCHAR: WrapChar(_col); break;
                    }//switch

                    if (_col.MText.Any())
                    {
                        var emptylines = _col.MText.Where(l => l.Trim().Empty());
                        if (emptylines.Any())
                            _col.MText.RemoveEx(emptylines.ToList());
                        _col.Lines = _col.MText.Count();
                        _col.Text = "";
                    }
                }
            });

            void WrapAround(ConsoleRecord col, string text = null)
            {
                text = (text ?? col.Text);
                var _lines = (int)Math.Ceiling(text.Length / (_option.Width * 1d));
                int _total = text.Length;
                for (int i = 1; i <= _lines; i++)
                {
                    try
                    {
                        var _len = text.Length < _option.Width ? text.Length : _option.Width;
                        col.MText.Add(text.Substring(0, _len));
                        if (text.Length >= _option.Width)
                            text = text.Substring(_option.Width);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error at Wrap: - {text}, Width: {_option.Width}, Message: {e.Message}");
                    }
                }
            }

            void WrapWordAround(ConsoleRecord col)
            {
                var _words = col.Text.SplitEx(' ').Where(s => !s.Trim().Empty()).ToArray();
                if (_words.Count() == 1) WrapAround(col);
                else
                {
                    var _text = _words[0];
                    int _next = 1;
                    while (true)
                    {
                        var _interim = $"{_text} {_words[_next]}";
                        if (_interim.Length <= _option.Width)
                            _text = _interim;
                        else
                        {
                            col.MText.Add(_text);
                            _text = _words[_next];
                        }
                        _next++;
                        if (_next >= _words.Count()) { col.MText.Add(_text); break; }
                    }
                }
            }

            void WrapChar(ConsoleRecord col)
            {
                var _sentence = col.Text.SplitEx(_option.WrapCharCharacter);
                foreach (var sen in _sentence)
                {
                    if (sen.Length >= _option.Width)
                        WrapAround(col, sen);
                    else
                        col.MText.Add(sen.PadRight(_option.Width));
                }
            }
        }
    }
}
