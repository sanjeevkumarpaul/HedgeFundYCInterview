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
using Wrappers;
using JsonSerializers;

namespace APICalls.Configurations
{
    public class APIConfiguration : IDisposable
    {
        private List<APIXmlNode> Apis = new List<APIXmlNode>();
        private IEnumerable<XElement> ApiElements;
        //private string BaseUrl;
        private APIXmlNode Base;
        private APIObjectParameter objectParams;

        private IEnumerable<IAPIProspect> ProspectResults { get { return Apis.Count > 0 ? Apis.Select(a => a.Result) : null; } }

        public APIConfiguration(APIConfigurationOptions options)
        {
            Initialize(options.ObjectParams);
            var initialization = options.Type.Equals("XML", StringComparison.CurrentCultureIgnoreCase) ? InitializeXML(options.Path) : InitializeJson(options.Path);
        }

        private void Initialize(object[] objectParameters)
        {
            //Keeping track.
            (objectParams = new APIObjectParameter()).ObjectParams.AddRange(objectParameters);
        }

        private bool InitializeXML(string configurationFilePathOrXML)
        {
            //Try to load it from file or Parase xml string itself.
            var all = (WrapIOs.Exists(configurationFilePathOrXML) ? XElement.Load(configurationFilePathOrXML) : XElement.Parse(configurationFilePathOrXML)).Elements();

            Base = new APIXmlNode(all.Where(n => n.Name == "Base").First(), null);

            ApiElements = from elem in all.Where(n => n.Name == "APIProspect")
                          let order = (elem.Attribute("Order")?.Value ?? "0").ToInt()
                          orderby order
                          select elem;

            return true;
        }

        private bool InitializeJson(string configurationFilePathOrJSON)
        {
            string json = WrapIOs.Exists(configurationFilePathOrJSON) ? WrapIOs.ReadAllLines(configurationFilePathOrJSON).ToList().JoinExt(Environment.NewLine) : configurationFilePathOrJSON;
            if (!JsonValidator.IsValid(json)) return false;

            var elements = JsonValidator.Create(json);



            return true;
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
                .Finally(() => PostEvents(apiResults ) )
                .Subscribe( (result) => apiResults.Reponses(result, this) );            
        }
        #endregion ~API Calling Sequences
            
        #region ^Extra Functional methods for Usage intermediatery
        /// <summary>
        /// Push intermediatry objects to that sequence of API calls can use it for Parameter references.
        /// When more than one API is called, one of the object within the result of first API call, might be used in second API call. this is where it is handy.
        /// </summary>
        /// <param name="obj">Any Object</param>
        public void InsertObjectParam(params object[] obj)
        {
            objectParams.ObjectParams.AddRange(obj);
        }

        public void Dispose()
        {
            objectParams = null;
        }
        #endregion ~Extra Functional methods for Usage intermediatery

        #region ^Private methods
        private void PostEvents(IAPIResult apiResults)
        {
            Task.Factory.StartNew(() => Dispose())
                        .ContinueWith(antecendent => apiResults.Post(this.ProspectResults))
                        .ContinueWith(antecendent => apiResults.Final(Apis.Last().Result))
                        .Wait();
        }
        
        private APIAuthorization Authorization(APIXmlNode node)
        {
            if (node.Token.Empty()) return null;

            var auth = new APIAuthorization {  IsTokenAHeader = node.TokenAsHeader, Token = LocateDynamicParamValue(node.Token, false) };
            
            return auth;
        }

        private APIRequestHeaders ContentTypes(APIXmlNode node)
        {
            var content = new APIRequestHeaders {   AcceptContentTypes = node.ContentTypes?.SplitEx(';'),
                                                    ParameterContentType = node.ParamContentType,
                                                    Headers  = node.Headers?.Select(h => new APINamePareMedia { Key = h.Key, Value = h.Value })?.ToArray()
                                                };

            return content;
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
        
        /// <summary>
        /// Executes API
        /// First creates XML NODE object, Creates instances of APIProspect<> and APIUti<> and then calls 'Call' method of APIUtil for synchronous call.
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        private IAPIProspect ExecuteApi( XElement api)
        {
            var node = new APIXmlNode(api, Base);
            
            var prospect = CreateInstance(node.GenericType, typeof(APIProspect<>));

            using (var prosBase = (APIProspectOptionBase)prospect)
            {
                prosBase.BaseUrl = Base.BaseUrl;
                prosBase.APIUri = LocateDynamicParamValue(node.ApiUri);
                prosBase.Method = node.Method;
                prosBase.Parameters = InjectObjectParams(node);
                prosBase.ParametersIsQueryString = node.ParametersAsQueryString;
                prosBase.Authorization = Authorization(node);
                prosBase.RequestHeaders = ContentTypes(node);
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
