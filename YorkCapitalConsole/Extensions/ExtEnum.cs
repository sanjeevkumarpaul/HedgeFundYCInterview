using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Extensions
{
    public static partial class ExtEnum
    {
        public static string Description<T>(this T en) where T : struct
        {
            FieldInfo fi = en.GetType().GetField(en.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return en.ToString();
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

        public static T ToEnum<T>(this string str, T defaultValue = default(T)) where T : struct
        {
            T res = defaultValue;
            Enum.TryParse<T>(str, out res);

            return res;
        }
        
    }
}
