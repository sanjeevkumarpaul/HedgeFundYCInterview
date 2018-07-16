using Newtonsoft.Json.Linq;
using System;
using System.Json;

namespace JsonSerializers
{
    public static class JsonValidator
    {
        public static bool IsValid(string jsonStr)
        {
            try
            {
                JsonValue.Parse(jsonStr);
            }
            catch (FormatException)
            {
                return false;                
            }

            return true;
        }

        public static JObject Create(string jsonStr)
        {
            JObject json = JObject.Parse(jsonStr);


            return json;

        }
    }
}
