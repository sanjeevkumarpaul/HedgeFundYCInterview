using System.IO;
using Wrappers.Consoles;
using Extensions;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Wrappers.Outputers.Base;

namespace Wrappers.Outputers
{
    public partial class WrapJsonTable : _BaseOutputTable
    {
        public WrapJsonTable(WrapOutputerOptions options) : base(options) { }
        public WrapJsonTable(ConsoleTable table, WrapOutputerOptions options) : base(table, options) { }
        public WrapJsonTable(List<ConsoleTable> tables, WrapOutputerOptions options) : base(tables, options) { }

        protected override void Init() { }
        protected override void Start() { }
        protected override void PutTable()
        {
            WriteHeadersFooters();
            WriteData();
            WriteHeadersFooters(false);
            Closure();
        }

        protected override void Finish() => _stream.WriteLine($"{{\"Resultset\" : [{_outs.JoinExt(",").TrimEx(",")}]}}");
    }

    partial class WrapJsonTable
    {
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
                _out.Append($"\"{(header? "Headers" : "Footers")}\" : [{_head.ToString().TrimEx(",")}],");
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
            _out.Append($"\"{(isAggregates? "Aggregates" : "Data")}\" : [{_body.ToString().TrimEx(",")}],");
        }
        private void Closure() => _outs.Add(new StringBuilder($"{{{_out.ToString().TrimEx(",")}}}"));
        private string GetText(ConsoleRecord record) => $"{record.Text}{record.MText.JoinExt()}";
        #endregion ~END OF Writing file with formating
    }
}
