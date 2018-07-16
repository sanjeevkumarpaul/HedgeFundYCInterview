using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static partial class ExtException
    {
        public static IEnumerable<Exception> ToList(this Exception ex)
        {
            var result = new List<Exception>();
            var current = ex;
            while (current != null)
            {
                result.Add(current);
                current = current.InnerException;
            }
            return result;
        }

        public static IEnumerable<String> ToMessages(this Exception ex)
        {
            var result = ex.ToList().Select(m => m.Message);
            return result;
        }

        public static string Messages(this Exception ex, string separator = "")
        {
            var result = ex.ToList().Select(m => m.Message);
            return result.JoinExt(separator);
        }

    }
}
