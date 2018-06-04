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
using Newtonsoft.Json.Linq;

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
            var initialization = options.Type.Equals("XML", StringComparison.CurrentCultureIgnoreCase) ? InitializeXML(options.PathOrContent) : InitializeJson(options.PathOrContent);
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

        public void UpdateObjectParam(object obj)
        {
            int index = 0;
            objectParams.ObjectParams.ForEach(o => { if (obj.GetType() == o.GetType()) return; else index++; } );

            if (index <= objectParams.ObjectParams.Count) objectParams.ObjectParams[index] = obj;
        }

        public void Dispose()
        {
            objectParams = null;
        }
        #endregion ~Extra Functional methods for Usage intermediatery

        #region ^Private methods
        #region ^Initization of Options/XML/Json to reflect Nodes
        private void Initialize(object[] objectParameters)
        {
            //Keeping track.
            this.objectParams = new APIObjectParameter();
            if (objectParameters != null)
                this.objectParams.ObjectParams.AddRange(objectParameters);
        }

        private bool InitializeXML(string configurationFilePathOrXML)
        {
            //Try to load it from file or Parase xml string itself.
            try
            {
                var all = (WrapIOs.Exists(configurationFilePathOrXML) ? XElement.Load(configurationFilePathOrXML) : XElement.Parse(configurationFilePathOrXML)).Elements();

                Base = new APIXmlNode(all.Where(n => n.Name == "Base").First(), null);

                ApiElements = from elem in all.Where(n => n.Name == "APIProspect")
                              let order = (elem.Attribute("Order")?.Value ?? "0").ToInt()
                              orderby order
                              select elem;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool InitializeJson(string configurationFilePathOrJSON)
        {
            string json = WrapIOs.Exists(configurationFilePathOrJSON) ? WrapIOs.ReadAllLines(configurationFilePathOrJSON).ToList().JoinExt(Environment.NewLine) : configurationFilePathOrJSON;
            if (!JsonValidator.IsValid(json)) return false;

            InitializeXML(JsonToXML(json));

            return true;
        }

        private string JsonToXML(string json)
        {
            var prospects = JsonValidator.Create(json);
            var _nl = Environment.NewLine;

            #region ^Xml Formats
            string xml = $"<?xml version=\"1.0\" ?>{_nl}<APIProspects>{_nl}";
            string _baseXml = $"<Base Name=\"{{0}}\" BaseUrl=\"{{1}}\"  Key=\"{{2}}\" />{_nl}";
            string _prospectXml = $"<APIProspect Name=\"{{0}}\" BaseUrl=\"{{1}}\" Uri=\"{{2}}\" Method=\"{{3}}\" IncludeKeyFromBase=\"{{4}}\" GenericType=\"{{5}}\" Token=\"{{6}}\" ContentTypes=\"{{7}}\">{_nl}";
            string _authXml = $"<Authorization Type=\"{{0}}\" Token=\"{{1}}\" TokenAsHeader=\"{{2}}\" />{_nl}";
            string _headerXml = $"<Header Key=\"{{0}}\" Value=\"{{1}}\" />{_nl}";
            string _paramHeaderXml = $"<Parameters QueryString=\"{{0}}\" ContentType=\"{{1}}\">{_nl}";
            string _paramXml = $"<Parameter Key=\"{{0}}\" Value=\"{{1}}\" />{_nl}";
            #endregion ~Xml Formats

            #region ^Parsing throught the Json
            foreach (var pros in prospects["APIProspects"])
            {
                #region ^Base Element
                if (pros["Base"] != null)
                {
                    var _base = pros["Base"];
                    xml += string.Format(_baseXml, _base["Name"]?.ToString(), _base["BaseUrl"]?.ToString(), _base["Key"]?.ToString());
                }
                #endregion ~Base Element

                #region ^Prospect Element
                else if (pros["APIProspect"] != null)
                {
                    var _prospect = pros["APIProspect"];
                    xml += string.Format(_prospectXml, _prospect["Name"]?.ToString(),
                                                       _prospect["BaseUrl"]?.ToString(),
                                                       _prospect["Uri"]?.ToString(),
                                                       _prospect["Method"]?.ToString(),
                                                       _prospect["IncludeKeyFromBase"]?.ToString(),
                                                       _prospect["GenericType"]?.ToString(),
                                                       _prospect["Token"]?.ToString(),
                                                       _prospect["ContentTypes"].ToString());
                    #region ^Authorization Element
                    if (_prospect["Authorization"] != null)
                    {
                        var _auth = _prospect["Authorization"];
                        xml += string.Format(_authXml, _auth["Type"]?.ToString(), _auth["Token"]?.ToString(), _auth["TokenAsHeader"]?.ToString());
                    }
                    #endregion ~Authrization Element

                    #region ^Header(s) Element
                    if (_prospect["Headers"] != null)
                    {
                        xml += $"<Headers>{_nl}";
                        foreach (var header in _prospect["Headers"])
                        {
                            xml += string.Format(_headerXml, header["Key"]?.ToString(), header["Value"]?.ToString());
                        }
                        xml += $"</Headers>{_nl}";
                    }
                    #endregion ~Header(s) Element

                    #region ^Parameter(s) Element
                    if (_prospect["Parameters"] != null)
                    {
                        if (_prospect["Parameters"]["ParamProperties"] != null)
                        {
                            var _props = _prospect["Parameters"]["ParamProperties"];
                            xml += string.Format(_paramHeaderXml, _props["QueryString"]?.ToString(), _props["ContentType"]?.ToString());
                        }
                        else
                            xml += $"<Parameters>{_nl}";
                        if (_prospect["Parameters"]["ParamValues"] != null)
                        {
                            foreach (var param in _prospect["Parameters"]["ParamValues"])
                            {
                                xml += string.Format(_paramXml, param["Key"]?.ToString(), param["Value"]?.ToString());
                            }
                        }
                        xml += $"</Parameters>{_nl}";
                    }
                    #endregion ~Parameters(s) Element

                    xml += $"</APIProspect>{_nl}";
                }
                #endregion ~Prospect Element
            }
            xml += $"</APIProspects>{_nl}";
            #endregion ~Parsing throught the Json

            return xml;

        }
        #endregion ~Initization of Options/XML/Json to reflect Nodes

        #region ^End Subcription to Raise. Post and Final
        /// <summary>
        /// At the end, Post and Final Subscription to be posted back to the caller.
        /// </summary>
        /// <param name="apiResults"></param>
        private void PostEvents(IAPIResult apiResults)
        {
            Task.Factory.StartNew(() => Dispose())
                        .ContinueWith(antecendent => apiResults.Post(this.ProspectResults))
                        .ContinueWith(antecendent => apiResults.Final(Apis.Last().Result))
                        .Wait();
        }
        #endregion

        #region ^API related information, Extract
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
        #endregion ~End of methods

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
                prosBase.BaseUrl = node.BaseUrl;
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
