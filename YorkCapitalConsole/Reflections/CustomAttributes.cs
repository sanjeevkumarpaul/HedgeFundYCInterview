using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflections
{
    public static class CustomAttributes
    {
        public static A[] Find<A>(Type t) where A : Attribute
        {
            if (t != null)
            {
                var attr = t.GetCustomAttributes(typeof(A), false);

               return (A[])attr;
            }

            return null;
        }

        public static A FindFirst<A>(Type t) where A: Attribute
        {
            var attr = Find<A>(t);
            if (attr != null) return (A)attr[0];
            
            return null;
        }

        public static A[] FindMethod<A>(Type t, string method) where A : Attribute
        {
            if (t != null)
            {
                var met = t.GetMethods().FirstOrDefault(m => m.Name.Equals(method, StringComparison.CurrentCultureIgnoreCase));
                if (met != null)
                {
                    var attr = met.GetCustomAttributes(typeof(A), false);

                    if (attr != null) return (A[])attr;
                }
            }

            return null;
        }

        public static A FindMethodFirst<A>(Type t, string method) where A : Attribute
        {
            var attr = FindMethod<A>(t, method);
            if (attr != null) return (A)attr[0];

            return null;
        }
    }
}
