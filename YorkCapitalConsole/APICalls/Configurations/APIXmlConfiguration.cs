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
        private IEnumerable<XElement> ApiElements;
        private string BaseUrl;
        private object[] placeholderElements;

        public APIXmlConfiguration(string configurationFilePath)
        {
            XElement xml = XElement.Load(configurationFilePath);
            var all = xml.Elements();

            BaseUrl = all.Where(n => n.Name == "Base").First().Attribute("Url").Value;

            ApiElements =  from elem in ApiElements.Where(n => n.Name == "APIProspect")
                           let order = elem.Attribute("Order").Value.ToInt()
                           orderby order
                           select elem;
        }

        public IEnumerable<IAPIProspect> ExecuteApis(params object[] apiParameterElements)
        {
            placeholderElements = apiParameterElements;
            foreach (var api in ApiElements)
            {
                var node = new APIXmlNode(api, BaseUrl);

                var resultNode = ExecuteApi(node.GenericType, node);
                Apis.Add(resultNode);

                yield return resultNode.Result;
            }

        }
        
        private APIAuthorization Authorization(ApiXmlNode node)
        {
            if (node.Token.Empty()) return null;

            var auth = new APIAuthorization { Mandatory = true, IsTokenAHeader = node.TokenAsHeader, Token = LocateDynamicParamValue(node.Token, false) };
            
            return auth;
        }

        private Dictionary<string, string> InjectObjectParams(ApiXmlNode node )
        {
            var parameters = node.Parameters;

            if (parameters != null)
            {
                Dictionary<string, string> dictReplacers = new Dictionary<string, string>();
                foreach (var para in parameters)
                {                   
                    dictReplacers.Add(para.Key, LocateDynamicParamValue(para.Value) );
                }

                return dictReplacers;
            }
            return parameters;
        }

        private string LocateDynamicParamValue(string placeholderStr, bool locateFromObjectParams = true)
        {
            //Find all matches with {...}
            var matches = Regex.Matches(placeholderStr, "[{].*[}]");
            if (matches.Count > 0)
            {
                //Lets make sure all the matches are taken care of.
                foreach (var match in matches)
                {
                    var item = match.ToString().TrimEx("{").TrimEx("}").SplitEx('.');

                    placeholderStr = GetDynamicParamObjectValue(item[0], placeholderStr, match.ToString(), item[1], locateFromObjectParams);                    
                }
            }

            return placeholderStr;
        }

        private string GetDynamicParamObjectValue(string typeName, string placeholderStr, string pattern, string propertyName, bool locateFromObjectParams = true)
        {
            object obj = null;

            if (locateFromObjectParams)
            {
                foreach (var o in objectParams.ObjectParams)
                {
                    if (o.GetType().Name.Equals(typeName)) { obj = o; break; }                   
                }
            }
            else
            {
                var _node = Apis.Where(n => n.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                if (_node != null) //challenge to find out if Result is f type Tokens
                {
                    obj = _node.Result;                    
                }
            }

            placeholderStr = placeholderStr.Replace(pattern, obj.GetVal(propertyName));
            
            return placeholderStr;
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
                prosBase.APIUri = LocateObjectParamValue(node.ApiUri);
                prosBase.Parameters = InjectObjectParams(node);
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
