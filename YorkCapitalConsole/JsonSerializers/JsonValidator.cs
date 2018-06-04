using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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
