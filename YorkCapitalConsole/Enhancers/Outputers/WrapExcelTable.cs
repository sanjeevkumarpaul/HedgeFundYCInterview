using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Extensions;
using Wrappers.Consoles;
using Wrappers.Consoles.Enums;
using Wrappers.Outputers.Base;

namespace Wrappers.Outputers
{
    public partial class WrapExcelTable : _BaseOutputTable
    {
        private Excel _xl;
        private Options _option;
        private UInt32Value _rowIndex = 0U;

        public WrapExcelTable(WrapOutputerOptions options) : base(options) { }
        public WrapExcelTable(ConsoleTable table, WrapOutputerOptions options) : base(table, options) { }
        public WrapExcelTable(List<ConsoleTable> tables, WrapOutputerOptions options) : base(tables, options) { }

        protected override void Init() { _xl = new Excel(); _option = new Options(); }
        protected override void Start() { }
        protected override void PutTable()
        {
            CalculateBasics();
            using (OpenExcel()) //Opens Excel as Memory Stream.
            {
                CreateExcelStyles();
                CreateHeadersFooters();
                CreateColumnHeaders();
                CreateData();
                CreateHeadersFooters(false);
            }
        }
        protected override void Finish()
        {
            _xl.SheetPart.Worksheet.InsertAfter(_xl.Columns, _xl.SheetPart.Worksheet.GetFirstChild<SheetFormatProperties>());
            _xl.SheetPart.Worksheet.InsertAfter(_xl.MergeCells, _xl.SheetPart.Worksheet.Elements<SheetData>().First());
            _xl.Document.SaveAs(_path);
            _xl.Document.Close();

            _xl.DisposeOff();
        }
    }

    partial class WrapExcelTable
    {
        private class Excel : IDisposable
        {
            internal MemoryStream Stream { get; set; }
            internal SpreadsheetDocument Document { get; set; }
            internal WorkbookPart BookPart { get; set; }
            internal WorksheetPart SheetPart { get; set; }
            internal WorkbookStylesPart StylePart { get; set; }
            internal SheetData Data { get; set; }
            internal Sheets Sheets { get; set; }
            internal Sheet Current { get; set; }
            internal MergeCells MergeCells { get; set; }
            internal Stylesheet Styles { get; set; }
            internal Columns Columns { get; set; }

            public void Dispose()
            {
               //Only to facilitiate Using() 
            }

            public void DisposeOff()
            {
                if (Stream != null) Stream.Close();
                Stream = null;
            }
        }

        private class Options
        {
            internal int Columns { get; set; }
            internal int TableWidth { get; set; }
            internal string HeaderFooterMergeCellReference { get; set; }
        }

        private void CalculateBasics()
        {
            _option.Columns = _table.ColumnOptions.Count;
            _option.TableWidth = _table.ColumnOptions.Sum(c => c.Width) + (2 * _option.Columns);
            _option.HeaderFooterMergeCellReference = $"{GetColumnAplha(1)}::RINDEX:::{GetColumnAplha(_option.Columns)}::RINDEX::";            
        }

        private Excel OpenExcel()
        {
            if (!_InProgress || _xl.Stream == null)
            {
                _xl.Stream = new MemoryStream();
                _xl.Document = SpreadsheetDocument.Create(_xl.Stream, SpreadsheetDocumentType.Workbook);

                _xl.BookPart = _xl.Document.AddWorkbookPart();
                _xl.BookPart.Workbook = new Workbook();

                _xl.SheetPart = _xl.BookPart.AddNewPart<WorksheetPart>();
                _xl.Data = new SheetData();
                _xl.SheetPart.Worksheet = new Worksheet(_xl.Data);

                _xl.StylePart = _xl.Document.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                _xl.StylePart.Stylesheet = new Stylesheet();
                _xl.Styles = _xl.StylePart.Stylesheet;
                
                _xl.Sheets = _xl.Document.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                               

                _xl.Current = new Sheet()
                {
                    Id = _xl.Document.WorkbookPart.GetIdOfPart(_xl.SheetPart),
                    SheetId = 1,
                    Name = $"{OutOption.Name}"
                };
                _xl.Sheets.AppendChild(_xl.Current);
                _xl.MergeCells = new MergeCells();
                _xl.Columns = new Columns();

                InitiateExcelRequiredStyles();                
            }
            return _xl;
        }

        private void InitiateExcelRequiredStyles()
        {
            // blank font list
            _xl.Styles.Fonts = new Fonts();          
            Font fnt1 = new Font();
            _xl.Styles.Fonts.AppendChild(fnt1); //0U
            _xl.Styles.Fonts.Count = 1;

            // create fills
            _xl.Styles.Fills = new Fills();
            _xl.Styles.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); //0U required, reserved by Excel
            _xl.Styles.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); //0U required, reserved by Excel
            _xl.Styles.Fills.Count = 2;

            // blank border list (Required by excel)
            _xl.Styles.Borders = new Borders();
            _xl.Styles.Borders.Count = 1;
            _xl.Styles.Borders.AppendChild(new Border()); //0U

            // blank cell format list (Required by excel)
            _xl.Styles.CellStyleFormats = new CellStyleFormats();
            _xl.Styles.CellStyleFormats.Count = 1;
            _xl.Styles.CellStyleFormats.AppendChild(new CellFormat()); //0U

            _xl.Styles.CellFormats = new CellFormats();
            _xl.Styles.CellFormats.Count = 1;
            // empty one for index 0, seems to be required
            _xl.Styles.CellFormats.AppendChild(new CellFormat());   //0U

            _xl.Styles.Save();
        }

        private void CreateExcelStyles()
        {
            var _styleIndex = 1U;
            //1. For all columns
            _table.ColumnOptions.ForEach(col => 
            {
                CreateIndividualStyle(col);                
                CreateColum(col);
            });            
            
            //2. For Headers and Footers
            HeaderFooterStyles();
            HeaderFooterStyles(false);

            void HeaderFooterStyles(bool header = true)
            {
                var rows = header ? _table.Headers : _table.Footers;
                if (!rows.Null() && rows.Any())
                {
                    rows.ForEach(row => CreateIndividualStyle(new ConsoleColumnOptions
                                        {
                                            Color = row.Heading.Color,
                                            Alignment = row.Alignment,
                                        }, row)                        
                    );
                }
            }

            void CreateIndividualStyle(ConsoleColumnOptions col, _ConsoleItemBase item = null)
            {
                CreateFont(col, _styleIndex + 1);
                CreateCellFormat(col, _styleIndex + 1);

                _xl.Styles.Save();
                col.XLStyleIndex = _styleIndex;
                if (item != null) item.XLStyleIndex = _styleIndex;
                _styleIndex++;
            }

            void CreateColum(ConsoleColumnOptions col)
            {
                if (_xl.Columns.Count() < (_styleIndex - 1))
                    _xl.Columns.Append(new Column { Min = _styleIndex - 1, Max = _styleIndex - 1, Width = col.Width, CustomWidth = true });
                else
                {
                    //var column = _xl.Columns[_styleIndex - 1];
                    //column.Width = col.Width;
                }
            }

            void CreateFont(ConsoleColumnOptions col, UInt32Value xlStyleIndex)
            {
                _xl.Styles.Fonts.AppendChild(new Font { Color = new Color { Rgb = new HexBinaryValue(ConsoleWebColors.Get(col.Color.ToString())) } });    //1U
                _xl.Styles.Fonts.Count = xlStyleIndex;
            }

            void CreateCellFormat(ConsoleColumnOptions col, UInt32Value xlStyleIndex)
            {
                var cformat = _xl.Styles.CellFormats.AppendChild(new CellFormat { FontId = _styleIndex }); //1U
                cformat.Alignment = new Alignment { Horizontal = GetAlignment(col.Alignment) };
                _xl.Styles.CellFormats.Count = xlStyleIndex;
            }

            HorizontalAlignmentValues GetAlignment(ConsoleAlignment align)
            {
                switch (align)
                {
                    case ConsoleAlignment.LEFT: return HorizontalAlignmentValues.Left;
                    case ConsoleAlignment.RIGHT: return HorizontalAlignmentValues.Right;
                    case ConsoleAlignment.CENTER: return HorizontalAlignmentValues.Center;
                    default: return HorizontalAlignmentValues.Left;
                }
            }
        }

        private void CreateHeadersFooters(bool header = true)
        {
            var rows = header ? _table.Headers : _table.Footers;

            if (!rows.Null() && rows.Any())
            {
                rows.ForEach(r => {

                    var xRow = new Row { RowIndex = ++_rowIndex };
                    _xl.Data.AppendChild(xRow);

                    var _txtRec = new ConsoleRecord { Text = $"{r.Heading.Text} : {r.Value.Text}", Alignment = r.Alignment, Color = r.Value.Color };
                    xRow.AppendChild(CreateTextCell( _txtRec, 1 ,_rowIndex, r.XLStyleIndex ));
                    var _ref = _option.HeaderFooterMergeCellReference.Replace("::RINDEX::", $"{_rowIndex}");
                    
                    _xl.MergeCells.Append(new MergeCell() { Reference = new StringValue(_ref) });                    
                });
            }
        }

        private void CreateColumnHeaders()
        {            
            var row = new Row { RowIndex = ++_rowIndex };
            _xl.Data.AppendChild(row);

            Int32 _cindex = 1;
            if (_table.OtherOptions.IsFirstRowAsHeader)
                _table.Rows.ElementAt(0).Column.ForEach(c => row.AppendChild(CreateTextCell(c, _cindex++, _rowIndex, 0) ));
        }

        private void CreateData()
        {
            foreach(var row in _table.Rows.Skip(_table.OtherOptions.IsFirstRowAsHeader ? 1 : 0))
            {
                Int32 _cindex = 1;
                var _row = new Row { RowIndex = ++_rowIndex };
                _xl.Data.AppendChild(_row);

                row.Column.ForEach(c => 
                {
                    var _opt = _table.ColumnOptions.ElementAt(_cindex - 1);
                    _row.AppendChild(CreateTextCell(c, _cindex++, _rowIndex, _opt.XLStyleIndex));
                });
            }
        }

        private Cell CreateTextCell(ConsoleRecord rec, Int32 colIndex, UInt32Value rowIndex, UInt32Value styleIndex)
        {
            var cell = new Cell
            {
                DataType = CellValues.InlineString,
                CellReference = $"{GetColumnAplha(colIndex)}{rowIndex}",  
                StyleIndex = styleIndex
            };

            var istring = new InlineString();
            var t = new Text { Text = $"{rec.Text}{rec.MText.JoinExt(Environment.NewLine)}" };
            istring.AppendChild(t);
            cell.AppendChild(istring);
            return cell;
        }

        private string GetColumnAplha(Int32 colIndex)
        {
            int div = colIndex;
            string colLetter = String.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }

    }
}
