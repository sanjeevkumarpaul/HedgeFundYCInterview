using System.Collections.Generic;
using System.IO;
using System.Text;
using Wrappers.Consoles;
using Wrappers.Outputers.Interface;

namespace Wrappers.Outputers.Base
{
    public abstract class _BaseOutputTable : IOutputTable
    {
        protected List<ConsoleTable> _tables;
        protected ConsoleTable _table;
        protected StreamWriter _stream;
        protected List<StringBuilder> _outs;
        protected StringBuilder _out;

        protected WrapOutputerOptions OutOption { get; private set; }

        public _BaseOutputTable(ConsoleTable table, WrapOutputerOptions options) : 
            this(new List<ConsoleTable> { table }, options) { }

        public _BaseOutputTable(List<ConsoleTable> tables, WrapOutputerOptions options)
        {
            (this._tables = tables).ForEach(t => WrapOutputerRadar.CalculateBoundaries(t));
            _outs = new List<StringBuilder>();
            _out = new StringBuilder();
            OutOption = options;
        }


        public abstract void Draw();
    }
}
