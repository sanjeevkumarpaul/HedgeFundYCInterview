using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrappers.Outputers
{
    public partial class WrapExcelTable : _BaseOutputTable
    {
        public WrapExcelTable(WrapOutputerOptions options) : base(options) { }
        public WrapExcelTable(ConsoleTable table, WrapOutputerOptions options) : base(table, options) { }
        public WrapExcelTable(List<ConsoleTable> tables, WrapOutputerOptions options) : base(tables, options) { }

        protected override void Init() { }
        protected override void Start() { }
        protected override void PutTable()
        { }
        protected override void Finish() { }
    }

    partial class WrapExcelTable
    {
        #region ^Save the HTML output to a file
        private void SaveToDisk()
        {            
            
        }
        #endregion ~END OF Save the HTML output to a file
    }
}
