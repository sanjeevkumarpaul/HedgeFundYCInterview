using System;
using System.Collections.Generic;
using System.Linq;

using Wrappers.Consoles;
using Wrappers.Consoles.Enums;
using Extensions;

namespace Wrappers
{
    public static class WrapConsoleTable
    {
        public static void PutTable(ConsoleTable table)
        {
            table = Sort(table);
            table = CalculateAggregation(table);

            #region ^Finding Column Width
            var max = table.ColumnOptions.Count();
            for (int i = 0; i < max - 1; i++)
            {
                var _len = table.Rows.Select(R => R.Column[i]).Max(R => R.Text.Length);
                if (table.ColumnOptions[i].Width <= _len) table.ColumnOptions[i].Width = _len;
            }
            max = table.ColumnOptions.Sum(c => c.Width);
            #endregion ~Finding Column Width

            var separator = table.OtherOptions.BorderChar.ToString().Repeat(max + (table.ColumnOptions.Count() * 2) + 1);

            Console.WriteLine();
            WrapConsole.WriteLineColor($"{separator}", table.OtherOptions.BorderColor);

            int col = 0;
            foreach (var rows in table.Rows)
            {
                var _borderColor = (rows.IsAggregate || (rows.IsLastRow && table.OtherOptions.IsAggregateRowExists)) ?
                                            table.OtherOptions.AggregateBorderColor : table.OtherOptions.BorderColor;
                var _borderBarColor = rows.IsAggregate ? _borderColor : table.OtherOptions.BorderColor;

                int i = 0;
                WrapConsole.WriteColor("|", _borderBarColor);                
                rows.Column.ForEach(C =>
                {
                    var _option = table.ColumnOptions[i++];
                    var _alText = _option.Alignment == WrapAlignment.LEFT ?
                                        (C.Text).PadRight(_option.Width) :
                                        _option.Alignment == WrapAlignment.RIGHT ? (C.Text).PadLeft(_option.Width) : CenteredText(C.Text, _option.Width);
                    var _color = C.IsAggregate ? table.OtherOptions.AggregateColor : 
                                        (col == 0 && table.OtherOptions.IsFirstRowAsHeader ? table.OtherOptions.HeadingColor : _option.Color);
                    var _prefix = _option.Alignment != WrapAlignment.RIGHT ? " " : ""; //Left space
                    var _postfix = _option.Alignment == WrapAlignment.RIGHT ? " " : "";//Right space

                    WrapConsole.WriteColor($"{_prefix}{ _alText }{_postfix}", _color);

                    if (_option.Alignment == WrapAlignment.CENTER)
                        WrapConsole.WriteColor($"{("|".PadLeft(_option.Width - _alText.Length + 1))}", _borderBarColor);
                    else
                        WrapConsole.WriteColor("|", _borderBarColor);                    
                });
                Console.WriteLine();
                WrapConsole.WriteLineColor($"{separator}", _borderColor);
                col++;
            }
        }

        /// <summary>
        /// Aligns a text center to any width.
        /// Obvious reason being Text should be less than the Width specified.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string CenteredText(string text, int width)
        {
            var len = width - text.Length;
            var padlen = ((int)len / 2) + text.Length;
            return text.PadLeft(padlen);
        }

        /// <summary>
        /// sorting the whole table before presenting
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static ConsoleTable Sort(ConsoleTable table)
        {
            if (table.OtherOptions.Sort.SortColumnIndex > 0)
            {
                var header = table.Rows.ElementAt(0);
                table.Rows.RemoveAt(0);

                List<ConsoleRow> _rows = null;
                if (table.OtherOptions.Sort.SortType == WrapSort.ASCENDING)
                    _rows = table.Rows.OrderBy(d => d.Column.ElementAt(table.OtherOptions.Sort.SortColumnIndex).Text).ToList();
                    //_rows = table.Rows.OrderBy(d => Convert(d.Column.ElementAt(table.OtherOptions.Sort.SortColumnIndex).Text, table.OtherOptions.Sort.DataType).ToList();
                else
                    _rows = table.Rows.OrderByDescending(d => d.Column.ElementAt(table.OtherOptions.Sort.SortColumnIndex).Text).ToList();

                _rows.Insert(0, header);
                table.Rows.Clear();
                table.Rows.AddRange(_rows);
            }

            object Convert(string value, WrapSortDataType sortDataType)
            {
                object _val = value;
                switch (sortDataType)
                {
                    case WrapSortDataType.STRING: break;
                    case WrapSortDataType.NUMBER: _val = value.ToDouble(); break;
                    case WrapSortDataType.DATETIME: _val = value.ToDateTime(); break;
                }

                return value;
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
                    Text = option.Aggregate == WrapAggregate.NONE ? "" : Aggregate(i, option),
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
                    case WrapAggregate.NONE: break;
                    case WrapAggregate.SUM: cal = vals.Sum(d => d); break;
                    case WrapAggregate.AVERAGE: cal = vals.Average(d => d); break;
                    case WrapAggregate.MEDIAN: cal = Median(vals); break;
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
    }
}
