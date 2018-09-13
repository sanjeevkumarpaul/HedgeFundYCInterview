using System.IO;
using System.Collections.Generic;
using System.Linq;
using Wrappers.Consoles;
using Extensions;
using System;
using Wrappers.Consoles.Enums;

namespace Wrappers
{
    public partial class WrapTextTable
    {
        private ConsoleTable _table;
        private StreamWriter _stream;
        private Options _option;

        public WrapTextTable(ConsoleTable table)
        {
            this._table = table;
            this._option = new Options();
        }

        public void Draw()
        {
            using (Create())
            {
                if (_stream.Null()) return;

                CalculateBasics();
                WriteHeaderFooter();

                WriteHeaderFooter(false);
                Close();
            }
        }
    }

    partial class WrapTextTable
    {
        internal class Options
        {
            internal int Columns { get; set; }
            internal int TableWidth { get; set; }

            internal List<ConsoleHeaderFooterRow> LeftHeaders { get; set; }
            internal List<ConsoleHeaderFooterRow> RightHeaders { get; set; }
            internal List<ConsoleHeaderFooterRow> CenterHeaders { get; set; }

            internal List<ConsoleHeaderFooterRow> LeftFooters { get; set; }
            internal List<ConsoleHeaderFooterRow> RightFooters { get; set; }
            internal List<ConsoleHeaderFooterRow> CenterFooters { get; set; }
        }

        internal class RowItems
        {
            internal List<ConsoleHeaderFooterRow> Rows { get; set; }
            internal ConsoleAlignment Align { get; set; }
        }


        private StreamWriter Create()
        {
            if (!_table.OtherOptions.Output.Path.Empty())
            {
                var _path = _table.OtherOptions.Output.Path;
                if (_path.Equals(Path.GetFileName(_path)))
                    _path = $@"{WrapIOs.CurrentFolder}\{_path}.txt";
                if (Directory.Exists(Path.GetDirectoryName(_path)))                
                    _stream = new StreamWriter(_path);                
            }

            return _stream;
        }
        private void Close()
        {
            if (!_stream.Null())
                _stream.Close();
            _stream = null;
        }
        private void CalculateBasics()
        {
            _option.Columns = _table.ColumnOptions.Count;
            _option.TableWidth = _table.ColumnOptions.Sum(c => c.Width) + ( 2 * _option.Columns );

            _option.LeftHeaders = _table.Headers?.FindAll(h => h.Alignment == ConsoleAlignment.LEFT);
            _option.RightHeaders = _table.Headers?.FindAll(h => h.Alignment == ConsoleAlignment.RIGHT);
            _option.CenterHeaders = _table.Headers?.FindAll(h => h.Alignment == ConsoleAlignment.CENTER);

            _option.LeftFooters = _table.Footers?.FindAll(h => h.Alignment == ConsoleAlignment.LEFT);
            _option.RightFooters = _table.Footers?.FindAll(h => h.Alignment == ConsoleAlignment.RIGHT);
            _option.CenterFooters = _table.Footers?.FindAll(h => h.Alignment == ConsoleAlignment.CENTER);
        }

        private void WriteHeaderFooter(bool header = true)
        {
            var items = (new List<RowItems>
            {
                new RowItems { Rows = header ? _option.LeftHeaders : _option.LeftFooters, Align = ConsoleAlignment.LEFT },
                new RowItems { Rows = header ? _option.RightHeaders : _option.RightFooters, Align = ConsoleAlignment.RIGHT },
                new RowItems { Rows = header ? _option.CenterHeaders : _option.CenterFooters, Align = ConsoleAlignment.CENTER }
            }).Where(r => r.Rows != null && r.Rows.Any()).ToList();

            if (items.Any())
            {
                var _separator = "=".Repeat(_option.TableWidth);

                if (header) _stream.WriteLine(_separator);
                foreach (var it in items)
                {
                    var _headingWidth = it.Rows.Max(r => r.Heading.Text.Length);
                    var _valueWidth = it.Rows.Max(r => r.Value.Text.Length);
                    var _leftGapWidth = _option.TableWidth - (_headingWidth + _valueWidth + 4); //3 is for " : " 
                    var _centerGapWidth = _leftGapWidth / 2;
                    it.Rows.ForEach(r =>
                    {
                        switch (it.Align)
                        {
                            case ConsoleAlignment.LEFT:  _stream.WriteLine($" {r.Heading.Text.PadRight(_headingWidth)} : {r.Value.Text}"); break;
                            case ConsoleAlignment.RIGHT:                                
                                _stream.WriteLine($"{" ".PadLeft(_leftGapWidth)}{r.Heading.Text.PadRight(_headingWidth)} : {r.Value.Text}");
                                break;
                            case ConsoleAlignment.CENTER:
                                _stream.WriteLine($"{" ".PadLeft(_centerGapWidth)}{r.Heading.Text.PadRight(_headingWidth)} : {r.Value.Text}");
                                break;
                        }

                    });
                }
                _stream.WriteLine(_separator);
            }
        }
    }
}
