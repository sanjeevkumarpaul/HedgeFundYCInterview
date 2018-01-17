using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static partial class ExtIEnumerable
    {
        public static string JoinExt<T>(this IEnumerable<T> objs, string seprator = " ", bool donotPutEmptyElements = false)
        {
            string joinstr = "";
            if (!objs.Null())
                foreach (T obj in objs)
                    joinstr = string.Format("{0}{1}{2}", joinstr, (donotPutEmptyElements) ? (joinstr.Empty() ? "" : seprator) : seprator, obj);

            return joinstr.Trim().TrimEx(seprator);
        }


    }
}
