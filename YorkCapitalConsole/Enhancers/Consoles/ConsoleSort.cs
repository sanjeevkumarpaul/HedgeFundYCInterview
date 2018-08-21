using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles.Enums;

namespace Wrappers.Consoles
{
    public class ConsoleSort
    {
        public int SortColumnIndex { get; set; } = -1;
        public WrapSort SortType { get; set; } = WrapSort.ASCENDING;
        public WrapSortDataType DataType { get; set; } = WrapSortDataType.STRING;
    }
}
