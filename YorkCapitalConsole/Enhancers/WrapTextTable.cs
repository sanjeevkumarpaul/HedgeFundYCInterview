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
                WriteColumnHeaders();
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

            internal List<Widther> Headers { get; set; } = new List<Widther>();
            internal List<Widther> Footers { get; set; } = new List<Widther>();
        }

        internal class Widther
        {
            internal ConsoleAlignment Align { get; private set; }
            internal int HeadingWidth { get; private set; }
            internal int ValueWidth { get; private set; }
            internal int LeftGapWidth { get; private set; }            
            internal Widther(int hwidth, int vwidth, ConsoleAlignment align, Options option)
            {
                Align = align;
                HeadingWidth = hwidth;
                ValueWidth = vwidth;

                if (align != ConsoleAlignment.LEFT)
                {
                    LeftGapWidth = ( option.TableWidth - (HeadingWidth + ValueWidth + 4) ) / ( align == ConsoleAlignment.CENTER ? 2 : 1 );                    
                }
            }
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

            CalculateHeaderFooterWidths();
            CalculateHeaderFooterWidths(false);

            void CalculateHeaderFooterWidths(bool header = true)
            {               
                var rows = header ? _table.Headers : _table.Footers;
                if (rows != null)
                {
                   foreach(var maxs in  (from g in rows.GroupBy(g => g.Alignment)
                                         select new
                                         {
                                           _hwidth = g.Max(m => m.Heading.Text.Length),
                                           _vwidth = g.Max(m => m.Value.Text.Length),
                                           _align = g.Key
                                          }))
                         AssignHeaderFooterWidths(maxs._hwidth, maxs._vwidth, maxs._align,  header);
                }
            }
            void AssignHeaderFooterWidths(int hwidth, int vwidth, ConsoleAlignment align,  bool header = true)
            {
                if (header)                
                    _option.Headers.Add(new Widther(hwidth, vwidth, align, _option));                                   
                else                
                    _option.Footers.Add(new Widther(hwidth, vwidth, align, _option));                
            }
        }

        private void WriteHeaderFooter(bool header = true)
        {
            var items = header ? _table.Headers : _table.Footers;

            if (items.Any())
            {
                var _separator = "=".Repeat(_option.TableWidth);
               
                if (header) _stream.WriteLine(_separator);
                foreach (var r in items)
                {
                    var _widther = header ? _option.Headers.Find(h => h.Align == r.Alignment) : _option.Footers.Find(h => h.Align == r.Alignment);
                    switch (r.Alignment)
                    {
                        case ConsoleAlignment.LEFT: Write(_widther, r); break;
                        case ConsoleAlignment.RIGHT: Write(_widther, r); break;                              
                        case ConsoleAlignment.CENTER: Write(_widther, r); break;                              
                    }
                }
                _stream.WriteLine(_separator);
            }

            void Write(Widther wid, ConsoleHeaderFooterRow r)
            {
                _stream.WriteLine($"{" ".PadLeft(wid.LeftGapWidth)}{r.Heading.Text.PadRight(wid.HeadingWidth)} : {r.Value.Text}");
            }
        }

        private void WriteColumnHeaders()
        {
            if (!_table.OtherOptions.IsFirstRowAsHeader) return;
            
            for (int i = 0; i < 2; i++)
            {
                int _colIndex = 0;                
                _table.Rows.ElementAt(0).Column.ForEach(col =>
                {
                    var opt = _table.ColumnOptions.ElementAt(_colIndex++);

                    if (i == 0)
                    {
                        if (col.Text.Empty()) col.Text = "Name";
                        _stream.Write($" {col.Text.PadRight(opt.Width)} ");
                    }
                    else
                        _stream.Write($" {"=".Repeat(opt.Width)} ");
                });
                _stream.WriteLine();
            }
        }
    }
}
