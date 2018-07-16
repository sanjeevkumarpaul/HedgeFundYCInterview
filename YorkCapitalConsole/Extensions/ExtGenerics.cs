using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Extensions
{
    public static partial class ExtGenerics
    {
        public static T NullToEmpty<T>(this T enu) where T : class, new()
        {
            return enu ?? new T();
        }

        public static string CallerMethodName<T>(this T obj, int level = 3)
        {
            string name = "UnKnown";

            try
            {
                StackTrace stackTrace = new StackTrace();           // get call stack
                StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

                StackFrame callingFrame = stackFrames[level];
                name = callingFrame.GetMethod().Name;
            }
            catch { }

            return name;
        }

        public static Type CallerType<T>(this T obj, int level = 3)
        {
            Type t = null;

            try
            {
                StackTrace stackTrace = new StackTrace();           // get call stack
                StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

                StackFrame callingFrame = stackFrames[level];
                t = callingFrame.GetMethod().DeclaringType;
            }
            catch { }

            return t;
        }

        public static string TextIn<T>(this T item, string prefix = "") where T : class
        {
            try
            {
                var properties = typeof(T).GetProperties();
                //Enforce.That(properties.Length == 1);
                return string.Format("{0} {1}: {2}", prefix.ToEmpty(), properties[0].Name, item);
            }
            catch { return string.Format("{0} - {1}", prefix.ToEmpty(), item); }
        }

        public static IList<T> RemoveEx<T>(this IList<T> elements, IList<T> items)
        {
            if (items != null)
                foreach (T item in items) elements.Remove(item);

            return elements;
        }

        public static string DelimitedValues<T>(this T obj, string delimiter = ",", bool nullToEmpty = true, bool virtuals = false, bool privates = false)
        {
            string value = String.Empty;

            obj.Properties(virtuals, privates).ForEach((p) =>
            {
                if (p.PropertyType.Namespace.ToLower().Equals("system"))
                {
                    string val = obj.GetVal(p.Name);

                    if ((!val.Empty()) || (val.Empty() && nullToEmpty))
                        value += string.Format("{0}{1}", val.ToEmpty(), delimiter);
                }
            });

            return value;
        }

        public static string GetDescription<T>(this T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }


    }
}
