using System.Collections.Generic;

namespace Extensions
{
    public static partial class ExtDictionary
    {
        public static T Val<T>(this Dictionary<string, T> dict, string key)
        {
            if (dict.ContainsKey(key))
                return (T)dict[key];
            else
                return default(T);
        }
    }
}
