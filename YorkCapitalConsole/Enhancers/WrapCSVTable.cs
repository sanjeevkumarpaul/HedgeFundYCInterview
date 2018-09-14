using System.IO;
using System.Collections.Generic;
using System.Linq;
using Wrappers.Consoles;
using Extensions;
using System;
using Wrappers.Consoles.Enums;

namespace Wrappers
{
    public partial class WrapCSVTable
    {
        //Catch is CSV will not put HEADERS and FOOTERS.

        private ConsoleTable _table;
        private StreamWriter _stream;
        
        public WrapCSVTable(ConsoleTable table)
        {
            this._table = table;        
        }

        public void Draw()
        {
            using (Create())
            {
                if (_stream.Null()) return;                 
                WriteData();
                Close();
            }
        }
    }

    partial class WrapCSVTable
    {
        #region ^Handlign Stream
        private StreamWriter Create()
        {
            var _path = WrapIOs.CreateAndCheckPath(_table.OtherOptions.Output.Path, "csv");
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
