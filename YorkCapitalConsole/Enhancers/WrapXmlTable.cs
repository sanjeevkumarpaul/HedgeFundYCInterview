using System.IO;
using Wrappers.Consoles;
using Extensions;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Wrappers
{
    public partial class WrapXmlTable
    {
        private ConsoleTable _table;
        private StreamWriter _stream;
        private StringBuilder _xml;
        
        public WrapXmlTable(ConsoleTable table)
        {
            this._table = table;
            this._xml = new StringBuilder();
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

    partial class WrapXmlTable
    {
        #region ^Handlign Stream
        private StreamWriter Create()
        {
            var _path = WrapIOs.CreateAndCheckPath(_table.OtherOptions.Output.Path, "xml");
            if (!_path.Empty()) _stream = new StreamWriter(_path);

            return _stream;
        }
        private void Close()
        {
            if (!_stream.Null())
            {                
                _stream.WriteLine($"<?xml version='1.0' encoding='UTF-8' ?><Results>{_xml.ToString()}</Results>");
                _stream.Close();
            }
            _stream = null;
        }
        #endregion ~END OF Handlign Stream

        #region ^Writing file with formating      
        private void WriteHeadersFooters(bool header = true)
        {
            var rows = header ? _table.Headers : _table.Footers;
            var _tag = header ? "Headers" : "Footers";
            var _head = new StringBuilder();
            if (!rows.Null() && rows.Any())
            {
                rows.ForEach(rw =>
                {
                    _head.Append($"<Row><Title>{rw.Heading.Text}</Title><Value>{rw.Value.Text}</Value></Row>");
                });
                _xml.Append($"<{_tag}>{_head.ToString()}</{_tag}>");
            }            
        }
        private void WriteData()
        {
            List<string> _heads = new List<string>();
            if (_table.OtherOptions.IsFirstRowAsHeader)
                _heads = _table.Rows.ElementAt(0).Column.Select(c => GetText(c).Replace(" ","")).ToList();
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
            var _tag = isAggregates ? "Aggregates" : "Data";
            foreach (var rw in _table.Rows.Skip(_table.OtherOptions.IsFirstRowAsHeader ? 1 : 0).Where(r => r.IsAggregate == isAggregates))
            {
                var _text = "";
                var _cIndex = 0;
                rw.Column.ForEach(col =>
                {
                    var _head = colheaders.ElementAt(_cIndex++);
                    _text += $"<{_head}>{GetText(col)}</{_head}>";
                });
                _body.Append($"<Row>{_text}</Row>");
            }
            _xml.Append($"<{_tag}>{_body.ToString()}</{_tag}>");
        }

        private string GetText(ConsoleRecord record)
        {
            return $"{record.Text}{record.MText.JoinExt()}";
        }
        #endregion ~END OF Writing file with formating
    }
}
