using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Extensions;

namespace APICalls.Configurations
{
    public class APIXmlConfiguration
    {
        private List<APIXmlNode> Apis = new List<APIXmlNode>();
        private IEnumerable<XElement> Elements;
        private string BaseUrl;

        public APIXmlConfiguration(string configurationFilePath)
        {
            XElement xml = XElement.Load(configurationFilePath);
            Elements = xml.Elements();

            BaseUrl = Elements.Where(n => n.Name == "Base").First().Attribute("Url").Value;
        }

        public void ExecuteApis()
        {
            var _prospects = from elem in Elements.Where(n => n.Name == "APIProspect")
                             let order = elem.Attribute("Order").Value.ToInt()
                             orderby order
                             select elem;

        }



    }
}
