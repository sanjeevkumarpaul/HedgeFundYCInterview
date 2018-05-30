using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using APICalls.Entities;
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
        
        private APIAuthorization Authorization(APIXmlNode node)
        {
            if (node.Token.Empty()) return null;

            var auth = new APIAuthorization { IsTokenAHeader = node.TokenAsHeader, Token = node.Token };

            var match = Regex.Match(node.Token, "[{].*[}]");
            if (match.Length > 0)
            {
                var item = match.Value.TrimEx("{").TrimEx("}").SplitEx('.');
                var _node = Apis.Where(n => n.Name.Equals(item[0], StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                if (_node != null) //challenge to find out if Result is f type Tokens
                {
                    var value = _node.Result.GetVal(item[1]);

                    auth.Token = auth.Token.Replace(match.Value, value);
                }
            }

            return auth;
        }
        
        private Type CreateGenericType(string prospectType, Type genericType)
        {
            Type customType = Type.GetType(prospectType);                
            Type constructedType = genericType.MakeGenericType(customType);

            return constructedType;
        }

        private object CreateInstance(string prospectType, Type genericType, params object[] parameters)
        {
            Type constructedType = CreateGenericType(prospectType, genericType);
            var realtype = Activator.CreateInstance(constructedType, parameters);

            return realtype;
        }
        
        private APIXmlNode ExecuteApi(string prospectType, APIXmlNode node)
        {
            var prospect = CreateInstance(prospectType, typeof(APIProspect<>));

            using (var prosBase = (APIProspectOptionBase)prospect)
            {
                prosBase.BaseUrl = BaseUrl;
                prosBase.APIUri = node.ApiUri;
                prosBase.Parameters = node.Parameters;
                prosBase.Authorization = Authorization(node);
            }

            var constructedType = CreateGenericType(prospectType, typeof(APIUtil<>));
            var apiObject = CreateInstance(prospectType, typeof(APIUtil<>), prospect);

            var method = constructedType.GetMethod("Call");
            var res = method.Invoke(apiObject, null);

            node.Result = (IAPIProspect)res;

            return node;
        } 

        /*
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
        */

        

    }
}
