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
        #region ^Handlign Stream
        private StreamWriter Create()
        {
            var _path = WrapIOs.CreateAndCheckPath(OutOption.Output.Path, "csv");
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
        private void WriteData()
        {
            _table.Rows.ForEach(rw => 
            {
                var _text = "";
                rw.Column.ForEach(col =>
                {
                    //Make sure all MText are taken care of.
                    _text += $"{col.Text}{col.MText.JoinExt()}`";
                });
                _stream.WriteLine(_text.TrimEnd(new char[] { '`'}));
            });
        }
        #endregion ~END OF Writing file with formating
    }
}
