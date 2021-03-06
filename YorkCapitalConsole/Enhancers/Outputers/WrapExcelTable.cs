﻿using DocumentFormat.OpenXml;
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
            internal SharedStringTablePart SharedStringPart { get; set; }

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

        private class TextStyle
        {
            internal bool Bold { get; set; }
            internal bool Underline { get; set; }
            internal string FontFamily { get; set; }
            internal HexBinaryValue Color { get; set; }
            internal bool Fill { get; set; }
            internal HexBinaryValue FillColor { get; set; }
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
                _xl.SharedStringPart = _xl.Document.WorkbookPart.AddNewPart<SharedStringTablePart>();
                _xl.SharedStringPart.SharedStringTable = new SharedStringTable();

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
            //1. Column Headers first
            CreateIndividualStyle(new ConsoleColumnOptions
            {
                Color = ConsoleColor.Black,
                Alignment = ConsoleAlignment.CENTER                
            },
            fontStyle: new TextStyle
            {
                Bold = true,
                Fill = true,
                FillColor = ConsoleWebColors.GetExcel(ConsoleColor.Yellow)
            });

            //2. For all columns
            _table.ColumnOptions.ForEach(col =>
            {
                CreateIndividualStyle(col);
                CreateColum(col);
            });

            //3. For Headers and Footers
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

            void CreateIndividualStyle(ConsoleColumnOptions col, _ConsoleItemBase item = null, TextStyle fontStyle = null)
            {
                CreateFont(col, _styleIndex + 1, fontStyle);
                CreateBorders(col, _styleIndex + 1);
                CreateFill(_styleIndex + 1, fontStyle);
                CreateCellFormat(col, _styleIndex + 1);                

                _xl.Styles.Save();
                col.XLStyleIndex = _styleIndex;
                if (item != null) item.XLStyleIndex = _styleIndex;
                _styleIndex++;
            }

            void CreateColum(ConsoleColumnOptions col)
            {
                var column = _xl.Columns.Elements<Column>().FirstOrDefault(r => r.Min == col.XLStyleIndex - 1);
                if (!column.Null() && column.Width < col.Width)                
                    column.Width = col.Width;                
                else
                    _xl.Columns.Append(new Column { Min = col.XLStyleIndex - 1, Max = col.XLStyleIndex - 1, Width = col.Width, CustomWidth = true });

                //if (_xl.Columns.Count() < (_styleIndex - 1))
                //    _xl.Columns.Append(new Column { Min = _styleIndex - 1, Max = _styleIndex - 1, Width = col.Width, CustomWidth = true });
                //else
                //{
                //    var column = _xl.Columns.Elements<Column>().FirstOrDefault(r => r.Min == col.XLStyleIndex);
                //    if (!column.Null() && column.Width < col.Width)
                //    {                        
                //        column.Width = col.Width;
                //    }
                //}
            }

            void CreateFont(ConsoleColumnOptions col, uint xlStyleIndex, TextStyle fontStyle = null)
            {
                _xl.Styles.Fonts.AppendChild(new Font
                    {
                        Color = new Color { Rgb = new HexBinaryValue(ConsoleWebColors.GetExcel(col.Color)) },
                        Bold = new Bold { Val = new BooleanValue(fontStyle?.Bold ?? false) }
                    });    //1U
                _xl.Styles.Fonts.Count = xlStyleIndex;
            }

            void CreateFill(uint xlStyleIndex, TextStyle fontStyle = null)
            {
                var _fill = new Fill( new PatternFill
                                      {
                                        ForegroundColor = new ForegroundColor { Rgb = fontStyle?.FillColor ?? ConsoleWebColors.GetExcel(ConsoleColor.DarkGray) },
                                        PatternType = PatternValues.Solid
                                      });


                _xl.Styles.Fills.AppendChild(_fill); 
                _xl.Styles.Fills.Count = xlStyleIndex + 1; //(+1) : Since excel default fill already has 2 fill object.
            }

            void CreateBorders(ConsoleColumnOptions col, uint xlStyleIndex)
            {
                Border br = new Border
                {
                    TopBorder = CreateBorderType<TopBorder>(_table.OtherOptions.BorderColor),
                    RightBorder = CreateBorderType<RightBorder>(_table.OtherOptions.BorderColor),
                    LeftBorder = CreateBorderType<LeftBorder>(_table.OtherOptions.BorderColor),
                    BottomBorder = CreateBorderType<BottomBorder>(_table.OtherOptions.BorderColor),                    
                };

                _xl.Styles.Borders.AppendChild(br);
                _xl.Styles.Borders.Count = xlStyleIndex;
            }

            void CreateCellFormat(ConsoleColumnOptions col, uint xlStyleIndex)
            {
                var cformat = _xl.Styles.CellFormats.AppendChild(new CellFormat
                {
                    FontId = _styleIndex,
                    BorderId = _styleIndex,
                    FillId = _styleIndex + 1,
                    ApplyBorder = true,
                    ApplyFill = true,
                    ApplyNumberFormat = (col.Aggregate == ConsoleAggregate.NONE),
                    NumberFormatId = col.Aggregate == ConsoleAggregate.NONE ? 0U : 39U
                }); //1U
                cformat.Alignment = new Alignment { Horizontal = GetAlignment(col.Alignment), WrapText = true, Vertical = VerticalAlignmentValues.Center };
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

            T CreateBorderType<T>(ConsoleColor color) where T : BorderPropertiesType
            {
                var _bType = Activator.CreateInstance<T>();

                _bType.Style = BorderStyleValues.Thin;
                _bType.Color = new Color { Rgb = new HexBinaryValue(ConsoleWebColors.GetExcel(color)) };
                
                return _bType;
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

                    var _txts = CreateTexts(r);
                    var _cell = CreateSharedCell(_txts, 1, _rowIndex, r.XLStyleIndex);

                    //var _txtRec = new ConsoleRecord { Text = $"{r.Heading.Text} : {r.Value.Text}", Alignment = r.Alignment, Color = r.Value.Color };                    
                    //xRow.AppendChild(CreateTextCell(_txtRec, 1, _rowIndex, r.XLStyleIndex));                    
                    xRow.AppendChild(_cell);

                    for (int cind = 2; cind <= _option.Columns; cind++)
                        xRow.AppendChild(CreateTextCell(new ConsoleRecord { Text = "" }, cind, _rowIndex, r.XLStyleIndex));

                    var _ref = _option.HeaderFooterMergeCellReference.Replace("::RINDEX::", $"{_rowIndex}");
                    _xl.MergeCells.Append(new MergeCell() { Reference = new StringValue(_ref) });
                });
            }

            List<ConsoleRecord> CreateTexts(ConsoleHeaderFooterRow row)
            {
                return new List<ConsoleRecord>
                {
                    new ConsoleRecord { Text = row.Heading.Text, Color = row.Heading.Color },
                    new ConsoleRecord { Text = ":", Color = row.Heading.Color },                //separator.
                    new ConsoleRecord { Text = row.Value.Text, Color = row.Value.Color },
                };
            }
        }

        private void CreateColumnHeaders()
        {            
            var row = new Row { RowIndex = ++_rowIndex };
            _xl.Data.AppendChild(row);

            Int32 _cindex = 1;
            if (_table.OtherOptions.IsFirstRowAsHeader)
                _table.Rows.ElementAt(0).Column.ForEach(c => row.AppendChild(CreateTextCell(c, _cindex++, _rowIndex, 1U)) );
        }

        private void CreateData()
        {
            foreach(var row in _table.Rows.Skip(_table.OtherOptions.IsFirstRowAsHeader ? 1 : 0))
            {
                Int32 _cindex = 1;
                var _row = new Row { RowIndex = ++_rowIndex };
                _xl.Data.AppendChild(_row);

                row.Column.ForEach(c => _row.AppendChild(CreateCell(c, _cindex++, _rowIndex)) );
            }

            Cell CreateCell(ConsoleRecord rec, int colIndex, uint rowIndex)
            {
                var _opt = _table.ColumnOptions.ElementAt(colIndex - 1);
                //return _opt.Aggregate == ConsoleAggregate.NONE ?
                //        CreateTextCell(rec, colIndex, _rowIndex, _opt.XLStyleIndex) :
                //        CreateNumericCell(rec, colIndex, _rowIndex, _opt.XLStyleIndex);
                return CreateTextCell(rec, colIndex, _rowIndex, _opt.XLStyleIndex);
            }
        }

        private Cell CreateTextCell(ConsoleRecord rec, int colIndex, UInt32Value rowIndex, UInt32Value styleIndex)
        {
            var cell = CreateCell(CellValues.InlineString, colIndex, rowIndex, styleIndex);
            InsertCellType<InlineString>(rec, cell, null);
            
            return cell;
        }

        private Cell CreateNumericCell(ConsoleRecord rec, int colIndex, UInt32Value rowIndex, UInt32Value styleIndex)
        {
            var cell = CreateCell(CellValues.Number, colIndex, rowIndex, styleIndex);
            InsertCellType<InlineString>(rec, cell, null,null, true);            
            return cell;
        }

        private Cell CreateSharedCell(List<ConsoleRecord> recs, int colIndex, UInt32Value rowIndex, UInt32Value styleIndex)
        {
            var cell = CreateCell(CellValues.SharedString, colIndex, rowIndex, styleIndex);
            SharedStringItem sharedStr = null;
            recs.ForEach(r => 
            {
                sharedStr = InsertCellType<SharedStringItem>(r, cell, sharedStr, new TextStyle { Color = ConsoleWebColors.GetExcel( r.Color ) });
            });
            //if(!sharedStr.Null()) cell.AppendChild(sharedStr);
            return cell;
        }

        private Cell CreateCell(CellValues cellType, int colIndex, UInt32Value rowIndex, uint styleIndex = 0U)
        {
            return new Cell
            {
                DataType = cellType,                
                CellReference = $"{GetColumnAplha(colIndex)}{rowIndex}",
                StyleIndex = styleIndex
            };
        }

        private T InsertCellType<T>(ConsoleRecord rec, Cell cell, T cellType = null, TextStyle style =  null, bool Numeric = false) where T : RstType
        {
           bool appendToCell = false;
            var strCellType = cellType ?? (T)Activator.CreateInstance<T>();
            var _text = $"{rec.Text}{rec.MText.JoinExt(Environment.NewLine)}";
            var t = new Text { Text = _text };

            if (strCellType is InlineString)
            {
                if (Numeric && _text.IsNumeric())                
                    cell.CellValue = new CellValue(_text);                                    
                else
                { strCellType.AppendChild(t); appendToCell = true; }
            }
            else if (strCellType is SharedStringItem) PutFont();
            
            if (appendToCell) cell.AppendChild(strCellType);

            void PutFont()
            {
                if (cellType.Null())
                {
                    _xl.SharedStringPart.SharedStringTable.Append(strCellType);
                    _xl.SharedStringPart.SharedStringTable.Save();
                    cell.CellValue = new CellValue((_xl.SharedStringPart.SharedStringTable.Elements<SharedStringItem>().Count() - 1).ToString());
                }

                if (!style.Null())
                {
                    var _shared = strCellType as SharedStringItem;
                    Run run = new Run();
                    RunProperties props = new RunProperties();
                    if (style.Bold) props.Append(new Bold());
                    if (style.Underline) props.Append(new Underline { Val = UnderlineValues.Single });
                    props.Append(new Color { Rgb = style.Color });

                    run.Append(props);
                    run.Append(t);

                    _shared.Append(run);
                }
            }

            return strCellType;        
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


/*
 * RUN PROPERTIES
 * RunProperties runProperties1 = new RunProperties();
                Bold bold1 = new Bold();
                Underline underline1 = new Underline();
                FontSize fontSize1 = new FontSize() { Val = 11D };
                Color color1 = new Color() { Theme = (UInt32Value)1U };

                RunFont runFont1 = new RunFont() { Val = "Calibri" };
                FontFamily fontFamily1 = new FontFamily() { Val = 2 };
                FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };
 */
