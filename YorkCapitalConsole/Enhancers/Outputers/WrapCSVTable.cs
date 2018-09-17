using System.IO;
using System.Collections.Generic;
using System.Linq;
using Wrappers.Consoles;
using Extensions;
using System;
using Wrappers.Consoles.Enums;
using Wrappers.Outputers.Base;

namespace Wrappers.Outputers
{
    public partial class WrapCSVTable : _BaseOutputTable
    {
        private List<string> _header = new List<string>();

        public WrapCSVTable( WrapOutputerOptions options) : base(options) { }
        public WrapCSVTable(ConsoleTable table, WrapOutputerOptions options) : base(table, options) { }
        public WrapCSVTable(List<ConsoleTable> tables, WrapOutputerOptions options) : base(tables, options) { }

        protected override void Init() { }
        protected override void Start() { }
        protected override void Finish() { }
        protected override void PutTable()
        {
            WriteData();
        }
    }

    partial class WrapCSVTable
    {       
        #region ^Writing file with formating      
        private void WriteData()
        {
            _table.Rows.ForEach(rw =>
                WriteDelimitedData(rw, ( _table.OtherOptions.IsFirstRowAsHeader && _table.Rows.First().Equals(rw)) ));
            
            void WriteDelimitedData(ConsoleRow row, bool isHeader = false)
            {
                var _text = "";
                row.Column.ForEach(col =>  _text += $"{col.Text}{col.MText.JoinExt()}`");
                if (isHeader)
                {
                    if (!_header.Any(h => h.Equals(_text)))
                    {
                        _header.Add(_text);
                        WriteToStream(_text);
                    }
                }
                else
                    WriteToStream(_text);
            }

            void WriteToStream(string text)
            {
                _stream.WriteLine(text.TrimEnd(new char[] { '`' }));
            }
        }
        #endregion ~END OF Writing file with formating
    }
}
