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
    public interface IAPIResult
    {
        void Reponses(IAPIProspect resultProspect, ApiXmlConfiguration config);        
    }
    
    public class APIXmlConfiguration
    {
        private List<APIXmlNode> Apis = new List<APIXmlNode>();
        private IEnumerable<XElement> ApiElements;
        private string BaseUrl;
        private APIObjectParameter objectParams;

        public APIXmlConfiguration(string configurationFilePath, APIObjectParameter parameters = null)
        {
            objectParams = parameters;
            XElement xml = XElement.Load(filename);
            var all = xml.Elements();

            BaseUrl = all.Where(n => n.Name == "Base").First().Attribute("Url").Value;

            ApiElements = from elem in all.Where(n => n.Name == "APIProspect")
                          let order = elem.Attribute("Order").Value.ToInt()
                          orderby order
                          select elem;
        }

        #region ^API Calling Sequences
        /// <summary>
        /// Sequencial call to all API in a Enumerable.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IAPIProspect> ExecuteApis()
        {            
            foreach (var api in ApiElements)            
                yield return CallForResult(api);          
        }

        /// <summary>
        /// Reactive Observable way. Subscription to be precisely done via IAPIResult as responses will be thrown to Its method "Responses"
        /// </summary>
        /// <param name="apiResults">IAPIResult object to get Reults there Asynchronously</param>
        public void ExecuteApisObservable(IAPIResult apiResults)
        {
            ApiElements
                .Select(api => CallForResult(api))
                .ToObservable(NewThreadScheduler.Default)
                .Subscribe( (result) => { apiResults.Reponses(result, this); });            
        }
        #endregion ~API Calling Sequences
            
        #region ^Extra Functional methods for Usage intermediatery
        /// <summary>
        /// Push intermediatry objects to that sequence of API calls can use it for Parameter references.
        /// When more than one API is called, one of the object within the result of first API call, might be used in second API call. this is where it is handy.
        /// </summary>
        /// <param name="obj">Any Object</param>
        public void InsertObjectParam(object obj)
        {
            objectParams.ObjectParams.Add(obj);
        }
        #endregion ~Extra Functional methods for Usage intermediatery
        
        private IAPIProspect CallForResult(XElement api)
        {
            var node = new ApiXmlNode(api, BaseUrl);

            var resultNode = ExecuteApi(node.GenericType, node);
            Apis.Add(resultNode);

            return resultNode.Result;
        }
            
        private APIAuthorization Authorization(APIXmlNode node)
        {
            if (node.Token.Empty()) return null;

            var auth = new APIAuthorization {  IsTokenAHeader = node.TokenAsHeader, Token = LocateDynamicParamValue(node.Token, false) };
            
            return auth;
        }
        
        private Dictionary<string, string> InjectObjectParams(APIXmlNode node )
        {
            var parameters = node.Parameters?.Keys.ToList();

            return parameters == null ?
                        node.Parameters :
                        ( ( Func<Dictionary<string, string>>)( () =>
                        {
                              Dictionary<string, string> dictReplacers = new Dictionary<string, string>();
                              parameters.ForEach(k => dictReplacers.Add(k, LocateDynamicParamValue(node.Parameters[k])));

                              return dictReplacers;
                        }))();
        }

        private string LocateDynamicParamValue(string placeholderStr, bool locateFromObjectParams = true)
        {
            //Find all matches with {...}
            Regex.Matches(placeholderStr, "{(.*)}").Cast<Match>().All(m =>
            {
                var item = m.Groups[1].Value.SplitEx('.'); //Groups[1] stores the group value like (.*). If there are more, more groups are created.

                if (item.Length > 1)
                    placeholderStr = GetDynamicParamObjectValue(item[0], placeholderStr, m.ToString(), item[1], locateFromObjectParams);

                return true;
            });
            
            return placeholderStr;            
        }

        private string GetDynamicParamObjectValue(string typeName, string placeholderStr, string pattern, string propertyName, bool locateFromObjectParams = true)
        {
            var val = (locateFromObjectParams ?
                        objectParams.ObjectParams.Find(o => o.GetType().Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase)) :
                        Apis.Where(n => n.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault()?.Result)
                      ?.GetVal(propertyName);
            
            return placeholderStr.Replace(pattern, val);
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
                prosBase.APIUri = LocateDynamicParamValue(node.ApiUri);
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
