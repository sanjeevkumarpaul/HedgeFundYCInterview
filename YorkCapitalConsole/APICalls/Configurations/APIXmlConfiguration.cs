using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using APICalls.Entities;
using Extensions;

namespace APICalls.Configurations
{
    public class APIXmlConfiguration : IDisposable
    {
        private List<APIXmlNode> Apis = new List<APIXmlNode>();
        private IEnumerable<XElement> ApiElements;
        private string BaseUrl;
        private APIObjectParameter objectParams;

        public IEnumerable<IAPIProspect> ProspectResults { get { return Apis.Count > 0 ? Apis.Select(a => a.Result) : null; } }

        public APIXmlConfiguration(string configurationFilePath, params object[] objectParameters)
        {
            //Keeping track.
            (objectParams = new APIObjectParameter()).ObjectParams.AddRange(objectParameters);

            XElement xml = XElement.Load(configurationFilePath);
            var all = xml.Elements();

            BaseUrl = all.Where(n => n.Name == "Base" && n.Attribute("Url") != null).First()?.Attribute("Url").Value;

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
                yield return ExecuteApi(api);          
        }

        /// <summary>
        /// Reactive Observable way. Subscription to be precisely done via IAPIResult as responses will be thrown to Its method "Responses"
        /// </summary>
        /// <param name="apiResults">IAPIResult object to get Reults there Asynchronously</param>
        public void ExecuteApisObservable(IAPIResult apiResults)
        {
            ApiElements
                .Select(api => ExecuteApi(api))
                .ToObservable(NewThreadScheduler.Default)
                .Finally(() => { Dispose();  apiResults.Post( ProspectResults ); })
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

        public void Dispose()
        {
            objectParams = null;
        }
        #endregion ~Extra Functional methods for Usage intermediatery

        #region ^Private methods
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
        
        private IAPIProspect ExecuteApi( XElement api)
        {
            var node = new APIXmlNode(api, BaseUrl);
            
            var prospect = CreateInstance(node.GenericType, typeof(APIProspect<>));

            using (var prosBase = (APIProspectOptionBase)prospect)
            {
                prosBase.BaseUrl = BaseUrl;
                prosBase.APIUri = LocateDynamicParamValue(node.ApiUri);
                prosBase.Method = node.Method;
                prosBase.Parameters = InjectObjectParams(node);
                prosBase.Authorization = Authorization(node);
            }

            var constructedType = CreateGenericType(node.GenericType, typeof(APIUtil<>));
            var apiObject = CreateInstance(node.GenericType, typeof(APIUtil<>), prospect);

            var method = constructedType.GetMethod("Call");
            var res = method.Invoke(apiObject, null);

            node.Result = (IAPIProspect)res;
            Apis.Add(node);


            return node.Result;
        }

        #endregion ~Private methods       
    }
}
