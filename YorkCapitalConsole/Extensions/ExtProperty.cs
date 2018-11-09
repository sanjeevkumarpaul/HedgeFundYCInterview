using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extensions
{
    public static partial class ExtProperty
    {
        public static void SetProperty<T>(this T obj, string propertyName, object propertyvalue) where T : class
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, propertyvalue, null);
            }
        }
        
        public static void SetProperties<T>(this T obj, char separator='=', params string[] values) where T : class
        {
            values.ToList().ForEach(vl =>
            {
                var items = vl.SplitEx(separator);
                if (items.Length == 2)
                {
                    var _prop = items[0];
                    var _val = string.Join("", items.Skip(1));

                    try
                    {
                        SetProperty(obj, _prop, _val);
                    }
                    catch { }
                }
            });
        }

        public static void SetProperties<T>(this T obj, object dynvalues)
        {
            if (dynvalues == null) return;

            var dynType = dynvalues.GetType();
            var insType = obj.GetType();

            foreach (var prop in dynType.GetProperties())
            {
                try
                {
                    PropertyInfo property = insType.GetProperty(prop.Name);
                    if (property != null)
                        property.SetValue(obj, prop.GetValue(dynvalues, null).ToString(), null);
                }
                catch { }
            }
        }

        public static dynamic GetPropertyValueViaAttribute<A, T>(this object obj, T argument) where A : Attribute

        {
            foreach (var property in obj.Properties())
            {
                CustomAttributeData attribute = property.GetCustomAttributesData().FirstOrDefault(a => a.GetType() == typeof(A));
                if (attribute != null)
                {
                    if (argument != null)
                    {
                        CustomAttributeTypedArgument typeArgument = attribute.ConstructorArguments.FirstOrDefault(c => c.GetType() == argument.GetType());
                        if (typeArgument != null)
                        {
                            if (((T)typeArgument.Value).Equals(argument)) return property.GetValue(obj, null);
                        }
                    }
                    else return property.GetValue(obj, null);
                }
            }

            return null;
        }


        internal static Dictionary<string, PropertyInfo> BaseProperties<T>(this Type type, bool virtuals = false, bool privates = false)
        {
            Dictionary<string, PropertyInfo> pros = new Dictionary<string, PropertyInfo>();

            Type _type = type.BaseType;
            if (_type == null || _type.FullName.StartsWith("System")) return pros;

            BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance |
                                 (privates ? BindingFlags.NonPublic : BindingFlags.Instance);

            foreach (PropertyInfo property in _type.GetProperties(Flags))
            {
                if (!virtuals)
                    if (_type.GetProperty(property.Name).GetGetMethod().IsVirtual) continue;

                pros.Add(property.Name, property);
            }

            foreach (var item in _type.BaseProperties<T>(virtuals, privates))
            {
                if (!pros.ContainsKey(item.Key)) pros.Add(item.Key, item.Value);
            }

            return pros;
        }

        internal static List<PropertyInfo> PropertiesTree<T>(this T obj, bool virtuals = false, bool privates = false)
        {
            var type = obj.GetType();

            Dictionary<string, PropertyInfo> pros = type.BaseProperties<T>(virtuals, privates);

            foreach (var property in obj.Properties(virtuals, privates))
            {
                if (!pros.ContainsKey(property.Name)) pros.Add(property.Name, property);
            }

            List<PropertyInfo> info = new List<PropertyInfo>();
            foreach (var item in pros) info.Add(item.Value);

            return info;
        }

        public static void Fill<T>(this T obj) where T : class
        {
            obj.Properties().ForEach(p =>

            {
                try
                {
                    if (p.PropertyType.Name.ToUpper() == "STRING")
                        obj.SetProperty(p.Name, "1");
                    else
                        obj.SetProperty(p.Name, 999);
                }
                catch { }
            }
                );
        }


        public static List<PropertyInfo> PublicFields(this Type t)
        {
            return t.GetTypeInfo().DeclaredProperties.ToList();
        }

        public static List<PropertyInfo> Properties<T>(this T obj, bool virtuals = false, bool privates = false)
        {
            List<PropertyInfo> pros = new List<PropertyInfo>();

            Type _type = obj.GetType();
            BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | (privates ? BindingFlags.NonPublic : BindingFlags.Instance);

            foreach (PropertyInfo property in _type.GetProperties(Flags))
            {
                if (!virtuals)
                    if (_type.GetProperty(property.Name).GetGetMethod().IsVirtual) continue;

                pros.Add(property);

            }

            return pros;
        }

        public static List<string> PropertyNames<T>(this T obj, bool virtuals = false, bool privates = false)
        {
            List<string> props = new List<string>();

            obj.Properties(virtuals, privates).ForEach(p => props.Add(p.Name));

            return props;
        }

        public static List<string> PropertyNamesTree<T>(this T obj, bool virtuals = false, bool privates = false)
        {
            List<string> props = new List<string>();

            var tree = obj.PropertiesTree(virtuals, privates);

            tree.ForEach(p =>
            {
                if (p.PropertyType != null && !p.PropertyType.Namespace.ToLower().Equals("system"))
                {
                    var instance = p.CreateInstance();
                    if (instance != null)
                        props.AddRange(instance.PropertyNamesTree(virtuals, privates));
                }
                else props.Add(p.Name);
            });

            return props;
        }

    }
}
