using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static partial class ExtConfiguration
    {
        public static string Get(string name)
        {
            return IsExists(name) ? ConfigurationManager.AppSettings[name] : "";
        }

        public static T Get<T>(string name, T defaultValue)
        {
            T val = Get<T>(name);

            if (typeof(T).IsValueType)
                return val.Empty(true) ? defaultValue : val;
            else
                return (val.Null() || val.Empty()) ? defaultValue : val;
        }

        public static T Get<T>(string name)
        {
            try
            {
                string value = Get(name);

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch { return default(T); }

        }

        private static bool IsExists(string name)
        {
            return ConfigurationManager.AppSettings.AllKeys.Any(a => a.Equals(name));
        }
    }

    public static partial class Configurations
    {
        public static class Connections
        {
            public static string Get(string name)
            {
                return IsExists(name) ? ConfigurationManager.ConnectionStrings[name].ConnectionString : "";
            }

            private static bool IsExists(string name)
            {
                return ConfigurationManager.ConnectionStrings[name] != null;
            }
        }
    }
}
