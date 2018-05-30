﻿using System;
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

        private ApiXmlNode CallApi(ApiXmlNode node)
        {
            switch (node.Type)
            {
                case "Tokens":
                    return ExecuteApi<Tokens>(node);
                case "ProestEstimate":
                    return ExecuteApi<ProestEstimate>(node);
                default: break;
            }

            return null;
        }
        private ApiXmlNode ExecuteApi<T>(ApiXmlNode node) where T : IAPIProspect, new()
        {
            var pros1 = new APIProspect<T>()
            {
                BaseUrl = BaseUrl,
                APIUri = node.Uri,
                Method = node.Method,
                Parameters = node.parameters,
                Authorization = Authorization(node)
            };

            node.Result = (new APIClient<T>(pros1)).CallAsync().Result;

            return node;
        }

        private APIAuthorization Authorization(ApiXmlNode node)
        {
            if (node.Token.Empty()) return null;

            var auth = new APIAuthorization { Mandatory = true, IsTokenAHeader = node.TokenAsHeader, Token = node.Token };

            var match = Regex.Match(node.Token, "[{].*[}]");
            if (match.Length > 0)
            {
                var item = match.Value.TrimEx("{").TrimEx("}");
                var _node = Apis.Where(n => n.Name.Equals(item, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                if (_node != null && _node.Result is Tokens) //challenge to find out if Result is f type Tokens
                    auth.Token = auth.Token.Replace(match.Value, (_node.Result as Tokens).Token);
            }

            return auth;
        }

    }
}
