using System.IO;
using System.Collections.Generic;
using System.Linq;
using Wrappers.Consoles;
using Extensions;
using System;
using Wrappers.Consoles.Enums;

namespace Wrappers.Outputers
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
        #region ^Internal Classes
        internal class Options
        {
            internal int Columns { get; set; }
            internal int TableWidth { get; set; }
            internal string Separator { get; set; }
            internal string SeparatorChar { get; set; } = "=";

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
        #endregion ~END OF Internal Classes

        #region ^Handlign Stream
        private StreamWriter Create()
        {
            var _path = WrapIOs.CreateAndCheckPath(_table.OtherOptions.Output.Path);
            if (!_path.Empty()) _stream = new StreamWriter(_path);

            return _stream;
        }
        private void Close()
        {
            if (!_stream.Null())
                _stream.Close();
            _stream = null;
        }
        #endregion ~END OF Handlign Stream

        #region ^Writing file with formating
        private void CalculateBasics()
        {
            _option.Columns = _table.ColumnOptions.Count;
            _option.TableWidth = _table.ColumnOptions.Sum(c => c.Width) + ( 2 * _option.Columns );
            _option.Separator = _option.SeparatorChar.Repeat(_option.TableWidth);

            CalculateHeaderFooterWidths();
            CalculateHeaderFooterWidths(false);

            void CalculateHeaderFooterWidths(bool header = true)
            {               
                var rows = header ? _table.Headers : _table.Footers;
                if (rows != null)
                {
                    foreach (var maxs in (from g in rows.GroupBy(g => g.Alignment)
                                          select new
                                          {
                                              _hwidth = g.Max(m => m.Heading.Text.Length),
                                              _vwidth = g.Max(m => m.Value.Text.Length),
                                              _align = g.Key
                                          }))                    
                        (header ? _option.Headers : _option.Footers)
                            .Add(new Widther(maxs._hwidth, maxs._vwidth, maxs._align, _option));                                            
                }
            }            
        }
        private void WriteHeaderFooter(bool header = true)
        {
            var items = header ? _table.Headers : _table.Footers;

            if (!items.Null() && items.Any())
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
            else
                _stream.WriteLine(_option.Separator);

            void Write(Widther wid, ConsoleHeaderFooterRow r)
            {
                _stream.WriteLine($"{" ".PadLeft(wid.LeftGapWidth)}{r.Heading.Text.PadRight(wid.HeadingWidth)} : {r.Value.Text}");
            }
        }
        private void WriteColumnHeaders()
        {
            if (!_table.OtherOptions.IsFirstRowAsHeader) return;           
            WriteData(true);
        }
        private void WriteData(bool onlyHeader = false)
        {
            foreach(var rw in _table.Rows.Take(onlyHeader? 1 : Int32.MaxValue).Skip(_table.OtherOptions.IsFirstRowAsHeader && !onlyHeader ? 1: 0))
            {
                if (rw.IsAggregate) _stream.WriteLine(_option.Separator);

                 var _lines = rw.Column.Max(c => c.MText.Count);
                _lines = (_lines <= 0 ? 1 : _lines) + ( onlyHeader ? 1 : 0 ); //If there is no MText found always take Text which has to get to 1 as total lines.

                for (int i = 0; i < _lines; i++)
                {
                    int _colIndex = 0;
                    rw.Column.ForEach(col => 
                    {
                        var opt = _table.ColumnOptions.ElementAt(_colIndex++);

                        if (onlyHeader && i == (_lines - 1) )
                            _stream.Write($" {_option.SeparatorChar.Repeat(opt.Width + (_colIndex == _option.Columns ? 1 : 0))} ");
                        else
                        {                            
                            var _ctext = i == 0 ? col.Text : "";
                            if (_ctext.Empty() && (col.MText != null && i < col.MText.Count))
                                _ctext = col.MText[i];

                            switch (opt.Alignment)
                            {
                                case ConsoleAlignment.LEFT: _stream.Write($" {_ctext.PadRight(opt.Width)} "); break;
                                case ConsoleAlignment.RIGHT: _stream.Write($" {_ctext.PadLeft(opt.Width)} "); break;
                                case ConsoleAlignment.CENTER: CenterAlign(opt, _ctext); break;
                            }
                        }                        
                    });//Column loop
                    _stream.WriteLine();                    
                }//Inner loop
                if (!onlyHeader)_stream.WriteLine();
            }//Outer loop

            void CenterAlign(ConsoleColumnOptions opt, string text)
            {
                double _centerPad = (opt.Width - text.Length) / 2.0;
                int _centerLeftPad = (int)Math.Floor(_centerPad); //Less of the value at left side Eg.,  if 7.5/7.1/7.9 => take 7
                int _centerRightPad = (int)Math.Ceiling(_centerPad); //Greater of the value at right side Eg., if 7.5/7.1/7.9 => take 8
                _stream.Write($"{"".PadLeft(_centerLeftPad)}{text}{"".PadRight(_centerRightPad)} ");
            }
        }
        #endregion ~END OF Writing file with formating
    }
}
