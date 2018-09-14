﻿using System.IO;
using Wrappers.Consoles;
using Extensions;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Wrappers
{
    public partial class WrapXmlTable
    {
        private List<ConsoleTable> _tables;
        private ConsoleTable _table;
        private StreamWriter _stream;
        private StringBuilder _xml;
        private List<StringBuilder> _xmls;

        public WrapXmlTable(ConsoleTable table) : this(new List<ConsoleTable> { table }) { }
        public WrapXmlTable(List<ConsoleTable> tables)
        {
            this._tables = tables;
            this._xml = new StringBuilder();
        }

        public void Draw()
        {
            using (Create())
            {
                if (_stream.Null()) return;
                Process();
                Close();
            }
        }
    }

    partial class WrapXmlTable
    {
        #region ^Handlign Stream
        private StreamWriter Create()
        {
            _table = _tables[0]; //Always take the first one since this is the file all of the output will be saved.
            _stream = WrapIOs.CreateStreamWriterForAppend(_table.OtherOptions.Output.Path, "xml");            
            return _stream;
        }
        private void Close()
        {
            if (!_stream.Null())
            {
                var _results = _xmls.Select(x => x.ToString()).ToList().JoinExt();
                _stream.WriteLine($"<?xml version='1.0' encoding='UTF-8' ?><Results>{_results}</Results>");
                _stream.Close();
            }
            _stream = null;
        }
        #endregion ~END OF Handlign Stream

        #region ^Writing file with formating     
        private void Process()
        {
            _xmls = new List<StringBuilder>();
            _tables.ForEach(t => 
            {
                _table = t;
                _xml = new StringBuilder();
                WriteHeadersFooters();
                WriteData();
                WriteHeadersFooters(false);
                WriteClosure();
            });
           
        }

        private void WriteClosure()
        {
            _xmls.Add( new StringBuilder($"<ResultSet>{_xml.ToString()}</ResultSet>") );
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
                _xml.Append($"<{_tag}>{_head.ToString()}</{_tag}>");
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
            _xml.Append($"<{_tag}>{_body.ToString()}</{_tag}>");
        }

        private string GetText(ConsoleRecord record)
        {
            return $"{record.Text.EscapeXmlNotations()}{record.MText.JoinExt().EscapeXmlNotations()}";
        }
        #endregion ~END OF Writing file with formating
    }
}
