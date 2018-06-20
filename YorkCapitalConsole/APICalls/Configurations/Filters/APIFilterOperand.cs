using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations.Filters
{
    internal class APIFilterOperand
    {
        internal string Object { get; set; }
        internal List<string> Properties { get; set; } = new List<string>();
        internal string Constant { get; set; }

        internal static bool FindAndReplace(List<APIFilterOperand> objects, string obj, string prop)
        {
            var _item = objects.FirstOrDefault(o => o.Object == obj);
            if (_item != null)
            {
                if (!_item.Properties.Any(p => p.Equals(prop))) //Check if property also exists.
                {
                    _item.Properties.Add(prop);
                    return true;
                }
            }
            return false;
        }
    }
}
