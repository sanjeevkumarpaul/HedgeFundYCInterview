using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extensions
{

    public static partial class ExtObject
    {
        public static bool Empty<T>(this T obj, bool considerZeroAsNull = false)
        {
            string value = string.Format("{0}", obj);
            if (considerZeroAsNull && value.Equals("0")) value = "";

            return value.Empty();
        }

        public static bool Null(this object obj)
        {
            return obj == null;
        }

        public static string Name(this object obj)
        {
            return obj.GetType().Name;
        }

        public static string ToStringExt(this object obj, bool minMaxDateToEmpty = false)
        {
            if (!obj.Null())
            {
                return (obj.ToString().ToLower() == "system.byte[]") ?
                    string.Format("content-length: {0}", ((byte[])obj).Length) :
                    (minMaxDateToEmpty && obj.GetType().Equals(typeof(DateTime)) ? obj.ToString().MaxMinDateToEmpty() : obj.ToString());
            }

            return null;
        }

        public static DataTypes DataType(this PropertyInfo property)
        {
            string _name = property
                                .PropertyType
                                .ToString()
                                .ToUpper()
                                .Replace("SYSTEM.", "")
                                .Replace("NULLABLE`1", "")
                                .Replace("[", "")
                                .Replace("]", "");

            switch (_name.Trim())
            {
                case "BYTE":
                case "SBYTE":
                case "SHORT":
                case "USHORT":
                case "INT":
                case "UINT":
                case "LONG":
                case "ULONG":
                case "FLOAT":
                case "DOUBLE":
                case "DECIMAL":
                case "MONEY":
                case "INT32":
                case "INT64":
                case "SINGLE":
                case "CURRENCY":
                    return DataTypes.NUMERIC;
                case "DATE":
                case "DATETIME":
                    return DataTypes.DATE;
                default:
                    return DataTypes.STRING;
            }
        }

        public static bool IsString(this PropertyInfo property)
        {
            return property.DataType() == DataTypes.STRING;
        }

        public static bool IsNumeric(this PropertyInfo property)
        {
            return property.DataType() == DataTypes.NUMERIC;
        }

        public static bool IsDate(this PropertyInfo property)
        {
            return property.DataType() == DataTypes.DATE;
        }

        public static object ConvertToPropertyType(this PropertyInfo prop, string value)
        {
            try
            {
                return Convert.ChangeType(value, prop.PropertyType);
            }
            catch
            {
                return value;
            }
        }
        

        public static object CreateInstance(this PropertyInfo prop)
        {
            if (prop.PropertyType == null) return null;

            var assembly = prop.PropertyType.GetTypeInfo().Assembly.ToString();
            var nspace = prop.PropertyType.GetTypeInfo().FullName;

            var handle = Activator.CreateInstance(assembly, nspace);

            return handle.Unwrap();
        }


        public static string DelimitedValuesTree(object obj, string delimiter = ",", bool virtuals = false, bool privates = false)
        {
            string value = String.Empty;

            var tree = obj.PropertiesTree(virtuals, privates);

            tree.ForEach((p) =>
            {
                if (p.PropertyType.Namespace.ToLower().Equals("system"))
                {
                    string val = obj.GetVal(p.Name);

                    value += string.Format("{0}{1}", val.ToEmpty("<EMPTY>"), delimiter);

                }
                else
                {
                    var partialValue = DelimitedValuesTree(p.GetValue(obj), delimiter, virtuals, privates);
                    value += partialValue;
                }
            });

            return value;
        }

        public static string DelimitedValuesTree<T>(this T obj, string delimiter = ",", bool nullToEmpty = true, bool virtuals = false, bool privates = false)
        {
            string value = DelimitedValuesTree(obj, delimiter, virtuals, privates);

            return value.Substring(0, value.Length - 1);
        }
        
        private static Dictionary<string, string> DictionaryValuesTrees(object obj, string parentName, bool virtuals = false, bool privates = false)
        {
            Dictionary<string, string> value = new Dictionary<string, string>();

            if (obj != null)
            {
                var tree = obj.PropertiesTree(virtuals, privates);

                tree.ForEach((p) =>
                {
                    if (p.PropertyType.Namespace.ToLower().Equals("system"))
                    {
                        string val = obj.GetVal(p.Name);
                        value.Add($"{parentName}{(parentName.Empty() ? "" : ".")}{p.Name}", val.ToEmpty());
                    }
                    else
                    {                        
                        var _name = !parentName.Empty() ? $"{parentName}.{p.Name}" : p.Name;

                        var partialValue = DictionaryValuesTrees(p.GetValue(obj), _name, virtuals, privates);
                        foreach (var dic in partialValue) value.Add(dic.Key, dic.Value);
                    }
                });
            }
            return value;
        }

        public static Dictionary<string, string> DictionaryValuesTree<T>(this T obj,  bool virtuals = false, bool privates = false)
        {
            return DictionaryValuesTrees(obj, "", virtuals, privates);            
        }


        public static string ClosestFieldName<T>(this T obj, string unAccountedfieldName)
        {
            if (obj == null) return null;

            unAccountedfieldName = unAccountedfieldName.ToLower();
            foreach (var pname in obj.PropertyNames())
            {
                if (unAccountedfieldName.Equals(pname.ToLower())) return pname;
            }

            return null;
        }

        public static string ClosestFieldName(this List<PropertyInfo> props, string unAccountedfieldName)
        {
            if (props == null) return null;

            unAccountedfieldName = unAccountedfieldName.ToLower();
            foreach (var pname in props.Select(p => p.Name))
            {
                if (unAccountedfieldName.Equals(pname.ToLower())) return pname;
            }

            return null;
        }

        public static T GetValue<T>(this object obj, string property, object defaultValue = null)
        {
            return (T)obj.GetType().GetProperty(property).GetValue(obj, null);
        }

        public static string GetVal(this object obj, string property)
        {
            try
            {
                return obj.GetType().GetProperty(property).GetValue(obj, null).ToString();
            }
            catch { }

            return string.Empty;
        }
        
        public static bool IsGeneric(this object obj)
        {
            return !obj.Null() && ( obj.GetType().IsConstructedGenericType && obj.GetType().IsGenericTypeDefinition );
        }

        public static bool ContainsItems(this object obj)
        {
            if (obj.IsGeneric())
            {
                Type t = obj.GetType();
                PropertyInfo prop = t.GetProperties().FirstOrDefault(p => p.Name.EqualsIgnoreCase("count"));
                if (prop != null)
                    return obj.GetVal(prop.Name).ToInt() > 0;
                return false;
            }
            else
                return !obj.Null();            
        }              
       
    }
}
