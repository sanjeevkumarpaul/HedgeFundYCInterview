using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfUtility.Entities
{
    public class PageRange
    {
        public int Start {internal get; set; }
        public int End { internal get; set; }

        public PageRange(int start, int end) { if (end >= start || end == 0) { Start = start; End = end; } }
    }
}
