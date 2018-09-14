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
                WriteData();
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
            internal string Separator { get; set; }

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
            _option.Separator = "=".Repeat(_option.TableWidth);

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
                _stream.WriteLine(_option.Separator);
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
                _stream.WriteLine(_option.Separator);
            }

            void Write(Widther wid, ConsoleHeaderFooterRow r)
            {
                _stream.WriteLine($"{" ".PadLeft(wid.LeftGapWidth)}{r.Heading.Text.PadRight(wid.HeadingWidth)} : {r.Value.Text}");
            }
        }

        private void WriteColumnHeaders()
        {
            if (!_table.OtherOptions.IsFirstRowAsHeader) return;

            var _lines = _table.Rows.ElementAt(0).Column.Max(c => c.MText.Count);

            for (int i = 0; i <= _lines; i++)
            {
                int _colIndex = 0;                
                _table.Rows.ElementAt(0).Column.ForEach(col =>
                {
                    var opt = _table.ColumnOptions.ElementAt(_colIndex++);

                    if (i < _lines)
                    {
                        var _ctext = i == 0 ? col.Text : "";
                        if (_ctext.Empty() && (col.MText != null && i < col.MText.Count ))
                            _ctext = col.MText[i];
                        if (!_ctext.Empty()) 
                            _stream.Write($" {_ctext.PadRight(opt.Width)} ");
                    }
                    else
                        _stream.Write($" {"=".Repeat(opt.Width + (_colIndex == _option.Columns ? 1 : 0))} ");
                });
                _stream.WriteLine();
            }
        }

        private void WriteData()
        {
            foreach(var rw in _table.Rows.Skip(_table.OtherOptions.IsFirstRowAsHeader? 1: 0))
            {
                if (rw.IsAggregate) _stream.WriteLine(_option.Separator);

                 var _lines = rw.Column.Max(c => c.MText.Count);

                for(int i = 0; i < (_lines <= 0 ? 1 : _lines); i++)
                {
                    int _colIndex = 0;
                    rw.Column.ForEach(col => 
                    {
                        var opt = _table.ColumnOptions.ElementAt(_colIndex++);
                        var _ctext = i == 0 ? col.Text : "";
                        if (_ctext.Empty() && (col.MText != null && i < col.MText.Count))
                            _ctext = col.MText[i];

                        double _centerPad = (opt.Width - _ctext.Length) / 2.0;
                        int _centerLeftPad = (int)Math.Floor(_centerPad);
                        int _centerRightPad = (int)Math.Ceiling(_centerPad);
                        switch(opt.Alignment)
                        {
                            case ConsoleAlignment.LEFT: _stream.Write($" {_ctext.PadRight(opt.Width)} "); break;
                            case ConsoleAlignment.RIGHT: _stream.Write($" {_ctext.PadLeft(opt.Width)} "); break;
                            case ConsoleAlignment.CENTER: _stream.Write($"{"".PadLeft(_centerLeftPad)}{_ctext}{"".PadRight(_centerRightPad)} "); break;
                        }
                        
                    });
                    _stream.WriteLine();                    
                }
                _stream.WriteLine();
            }


        }
    }
}
