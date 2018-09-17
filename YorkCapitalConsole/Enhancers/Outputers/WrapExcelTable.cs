using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles;
using Wrappers.Outputers.Base;

namespace Wrappers.Outputers
{
    public partial class WrapExcelTable : _BaseOutputTable
    {
        private Excel xl;
        public WrapExcelTable(WrapOutputerOptions options) : base(options) { }
        public WrapExcelTable(ConsoleTable table, WrapOutputerOptions options) : base(table, options) { }
        public WrapExcelTable(List<ConsoleTable> tables, WrapOutputerOptions options) : base(tables, options) { }

        protected override void Init() { xl = new Excel(); }
        protected override void Start() { }
        protected override void PutTable()
        {
            using (OpenExcel()) //Opens Excel as Memory Stream.
            {
                CreateColumnHeaders();
            }
        }
        protected override void Finish()
        {            
            xl.Document.SaveAs(_path);

            xl.DisposeOff();
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
            internal SheetData Data { get; set; }
            internal Sheets Sheets { get; set; }
            internal Sheet Current { get; set; }

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
                
        private Excel OpenExcel()
        {
            if (!_InProgress || xl.Stream == null)
            {
                xl.Stream = new MemoryStream();
                xl.Document = SpreadsheetDocument.Create(xl.Stream, SpreadsheetDocumentType.Workbook);

                xl.BookPart = xl.Document.AddWorkbookPart();
                xl.BookPart.Workbook = new Workbook();
                xl.SheetPart = xl.BookPart.AddNewPart<WorksheetPart>();
                xl.Data = new SheetData();

                xl.SheetPart.Worksheet = new Worksheet(xl.Data);

                xl.Sheets = xl.Document.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                xl.Current = new Sheet()
                {
                    Id = xl.Document.WorkbookPart.GetIdOfPart(xl.SheetPart),
                    SheetId = 1,
                    Name = $"{OutOption.Name}"
                };
                xl.Sheets.AppendChild(xl.Current);
            }
            return xl;
        }

        private void CreateColumnHeaders()
        {
            UInt32 rowIdex = 0;
            var row = new Row { RowIndex = ++rowIdex };
            xl.Data.AppendChild(row);

            if (_table.OtherOptions.IsFirstRowAsHeader)
                _table.Rows.Take(1).ElementAt(0).Column.ForEach(c => row.AppendChild( CreateTextCell(c.Text) ));
        }

        private Cell CreateTextCell(string text)
        {
            var cell = new Cell
            {
                DataType = CellValues.InlineString,
                CellReference = text.GetHashCode().ToString()
            };

            var istring = new InlineString();
            var t = new Text { Text = text };
            istring.AppendChild(t);
            cell.AppendChild(istring);
            return cell;
        }
    }
}
