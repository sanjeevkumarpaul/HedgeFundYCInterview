using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static partial class ExtType
    {
        public static void AddToDynamicLinqPredefinedType(this Type current)
        {
            var type = typeof(DynamicQueryable).Assembly.GetType("System.Linq.Dynamic.ExpressionParser");

            FieldInfo field = type.GetField("predefinedTypes", BindingFlags.Static | BindingFlags.NonPublic);

            Type[] predefinedTypes = (Type[])field.GetValue(null);

            Array.Resize(ref predefinedTypes, predefinedTypes.Length + 1);
            predefinedTypes[predefinedTypes.Length - 1] = current;

            field.SetValue(null, predefinedTypes);
        }

    }
}
