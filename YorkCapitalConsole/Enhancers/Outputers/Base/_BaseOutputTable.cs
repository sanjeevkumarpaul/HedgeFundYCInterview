using Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wrappers.Consoles;
using Wrappers.Consoles.Enums;
using Wrappers.Outputers.Interface;

namespace Wrappers.Outputers.Base
{
    public abstract class _BaseOutputTable : IOutputTable
    {
        #region  ^Protected member variables
        protected List<ConsoleTable> _tables;
        protected ConsoleTable _table;
        protected StreamWriter _stream;
        protected List<StringBuilder> _outs;
        protected StringBuilder _out;
        protected bool _InProgress = false;
        protected string _path = string.Empty;
        #endregion  ~Protected member variables

        #region ^Properties
        protected WrapOutputerOptions OutOption { get; private set; }        
        #endregion ~Properties

        #region ^Constructor
        public _BaseOutputTable(WrapOutputerOptions options) : 
            this(new List<ConsoleTable>(), options) {  }

        public _BaseOutputTable(ConsoleTable table, WrapOutputerOptions options) : 
            this(new List<ConsoleTable> { table }, options) { }

        public _BaseOutputTable(List<ConsoleTable> tables, WrapOutputerOptions options)
        {
            (this._tables = tables).ForEach(t => WrapOutputerRadar.CalculateBoundaries(t));
            _outs = new List<StringBuilder>();
            _out = new StringBuilder();
            OutOption = options;
            Init();
        }
        #endregion ~Constructor

        #region ^Abstract Methods
        protected abstract void Init();
        protected abstract void Start();
        protected abstract void Finish();
        protected abstract void PutTable();
        #endregion ~Abstract Methods

        #region ^Public Methods
        public IOutputTable Add(ConsoleTable table)
        {
            WrapOutputerRadar.CalculateBoundaries(table);
            _tables.Add(table);
            return this;
        }

        public IOutputTable Remove(ConsoleTable table)
        {
            try
            {
                _tables.Remove(table);                
            }
            catch { }
            return this;
        }

        public IOutputTable Remove(int index = -1)
        {
            try
            {
                if (index < 0)
                    _tables.RemoveAll(r => r!=null); //Basically removes all.
                else
                    _tables.RemoveAt(index);
            }
            catch { }
            return this;
        }

        public void Draw()
        {
            try
            {
                _InProgress = true;
                using (OpenStream())
                {
                    Start();
                    foreach (var tab in _tables)
                    {
                        _table = tab;
                        PutTable();
                    }
                    Finish();
                    Close();
                }
            }
            finally { _InProgress = false; }
        }
        #endregion ~Public Methods

        #region ^Protected Methods        
        #endregion ~Protected Methods

        #region ^Private Methods
        private StreamWriter OpenStream()
        {
            switch (OutOption.Output.Style)
            {
                case ConsoleOutputType.CONSOLE:
                case ConsoleOutputType.EXCEL:
                case ConsoleOutputType.XL:
                case ConsoleOutputType.XLS:
                case ConsoleOutputType.XSLX: break;

                default:
                    _stream = WrapIOs.CreateStreamWriterForAppend(OutOption.Output.Path, OutOption.Output.Extension);
                    break;
            }

            return _stream;
        }

        private void Close()
        {
            if (_stream != null) _stream.Close();
            _stream = null;
        }
        #endregion ~Private Methods
    }
}
