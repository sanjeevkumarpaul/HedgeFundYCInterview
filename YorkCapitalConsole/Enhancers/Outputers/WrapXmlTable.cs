using System.IO;
using Wrappers.Consoles;
using Extensions;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Wrappers.Outputers.Base;

namespace Wrappers.Outputers
{
    public partial class WrapXmlTable : _BaseOutputTable
    {
        public WrapXmlTable(WrapOutputerOptions options) : base(options) { }
        public WrapXmlTable(ConsoleTable table, WrapOutputerOptions options) : base(table, options) { }
        public WrapXmlTable(List<ConsoleTable> tables, WrapOutputerOptions options) : base(tables, options) { }

        protected override void Init() { }
        protected override void Start() { }
        protected override void PutTable()
        {
            if (_stream.Null()) return;
            Process();
        }
        protected override void Finish()
        {
            var _results = _outs.Select(x => x.ToString()).ToList().JoinExt();
            _stream.WriteLine($"<?xml version='1.0' encoding='UTF-8' ?><Results>{_results}</Results>");
        }
    }

    partial class WrapXmlTable
    {
        #region ^Writing file with formating     
        private void Process()
        {            
            _out = new StringBuilder();
            WriteHeadersFooters();
            WriteData();
            WriteHeadersFooters(false);
            WriteClosure();            
        }
        private void WriteClosure()
        {
            _outs.Add( new StringBuilder($"<ResultSet>{_out.ToString()}</ResultSet>") );
        }
        private void WriteHeadersFooters(bool header = true)
        {
            var rows = header ? _table.Headers : _table.Footers;
            var _tag = header ? "Headers" : "Footers";
            var _head = new StringBuilder();
            if (!rows.Null() && rows.Any())
            {
                rows.ForEach(rw =>
                {
                    var _htext = rw.Heading.Text.EscapeXmlNotations();
                    var _vtext = rw.Value.Text.EscapeXmlNotations();
                    _head.Append($"<Row><Title>{_htext}</Title><Value>{_vtext}</Value></Row>");
                });
                _out.Append($"<{_tag}>{_head.ToString()}</{_tag}>");
            }            
        }
        private void WriteData()
        {
            List<string> _heads = new List<string>();
            if (_table.OtherOptions.IsFirstRowAsHeader)
                _heads = _table.Rows.ElementAt(0).Column.Select(c => GetText(c).RemoveSpecialCharacters().ToEmpty("flag")).ToList();
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
            _out.Append($"<{_tag}>{_body.ToString()}</{_tag}>");
        }
        private string GetText(ConsoleRecord record)
        {
            return $"{record.Text.EscapeXmlNotations()}{record.MText.JoinExt().EscapeXmlNotations()}";
        }
        #endregion ~END OF Writing file with formating
    }
}
