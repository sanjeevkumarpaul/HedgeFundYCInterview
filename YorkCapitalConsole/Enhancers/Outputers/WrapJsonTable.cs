using System.IO;
using Wrappers.Consoles;
using Extensions;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Wrappers.Outputers
{
    public partial class WrapJsonTable
    {
        private ConsoleTable _table;
        private StreamWriter _stream;
        private StringBuilder _json;
        
        public WrapJsonTable(ConsoleTable table)
        {
            this._table = table;
            this._json = new StringBuilder();
        }

        public void Draw()
        {
            using (Create())
            {
                if (_stream.Null()) return;
                WriteHeadersFooters();
                WriteData();
                WriteHeadersFooters(false);
                Close();
            }
        }
    }

    partial class WrapJsonTable
    {
        #region ^Handlign Stream
        private StreamWriter Create()
        {
            var _path = WrapIOs.CreateAndCheckPath(_table.OtherOptions.Output.Path, "json");
            if (!_path.Empty()) _stream = new StreamWriter(_path);

            return _stream;
        }
        private void Close()
        {
            if (!_stream.Null())
            {
                _stream.WriteLine($"{{{_json.ToString().TrimEx(",")}}}");
                _stream.Close();
            }
            _stream = null;
        }
        #endregion ~END OF Handlign Stream

        #region ^Writing file with formating      
        private void WriteHeadersFooters(bool header = true)
        {
            var rows = header ? _table.Headers : _table.Footers;
            var _head = new StringBuilder();
            if (!rows.Null() && rows.Any())
            {
                rows.ForEach(rw =>
                {
                    _head.Append($"{{\"Title\" : \"{rw.Heading.Text}\", \"Value\" : \"{rw.Value.Text}\"}},");
                });
                _json.Append($"\"{(header? "Headers" : "Footers")}\" : [{_head.ToString().TrimEx(",")}],");
            }            
        }
        private void WriteData()
        {
            List<string> _heads = new List<string>();
            if (_table.OtherOptions.IsFirstRowAsHeader)
                _heads = _table.Rows.ElementAt(0).Column.Select(c => GetText(c).RemoveSpecialCharacters()).ToList();
            else
            {
                int i = 0;
                _table.ColumnOptions.ForEach(c => _heads.Add($"Field{++i}"));
            }
            
            ExtractData(_heads);
            ExtractData(_heads, true);
        }

        private void ExtractData(List<string> colheaders, bool isAggregates = false)
        {
            var _body = new StringBuilder();
            foreach (var rw in _table.Rows.Skip(_table.OtherOptions.IsFirstRowAsHeader ? 1 : 0).Where(r => r.IsAggregate == isAggregates))
            {
                var _text = "";
                var _cIndex = 0;
                rw.Column.ForEach(col =>
                {
                    var _head = colheaders.ElementAt(_cIndex++);
                    _text += $"\"{_head}\" : \"{GetText(col)}\",";
                });
                _body.Append($"{{{_text.TrimEx(",")}}},");
            }
            _json.Append($"\"{(isAggregates? "Aggregates" : "Data")}\" : [{_body.ToString().TrimEx(",")}],");
        }

        private string GetText(ConsoleRecord record)
        {
            return $"{record.Text}{record.MText.JoinExt()}";
        }
        #endregion ~END OF Writing file with formating
    }
}
