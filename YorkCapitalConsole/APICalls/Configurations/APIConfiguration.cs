#region ^System Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
#endregion ~System Namespaces

#region ^Custom Namespaces
using APICalls.Entities;
using Extensions;
using Wrappers;
using JsonSerializers;
using System.Threading;
#endregion ~Custom Namespaces

namespace APICalls.Configurations
{
    /// <summary>
    /// Public class
    /// </summary>
    public partial class APIConfiguration : IDisposable
    {
        public void Dispose()
        {
            objectParams = null;
        }
    }
    /// <summary>
    /// Requried Variables
    /// </summary>
    public partial class APIConfiguration
    {
        private List<APIXmlNode> Apis = new List<APIXmlNode>();
        private IEnumerable<XElement> ApiElements;

        #region ^Required Objects for the App.
        private APIConfigurationOptions Options;
        private APIXmlNode Base;
        private APIXmlNode Current;
        private APIObjectParameter objectParams;
        #endregion ~Required Objects for the App.

        #region ^Required variables for the App.
        private bool _isCancelled = false;
        private bool _isCancelledRepeat = false;
        private CancellationTokenSource _apiCancellation = new CancellationTokenSource();
        #endregion ^Required variables for the App.
    }
    /// <summary>
    /// Required Public Properties
    /// </summary>
    public partial class APIConfiguration
    {
        #region ^Public Properties
        public IEnumerable<IAPIProspect> ProspectResults { get { return Apis.Count > 0 ? Apis.Select(a => a.Result) : null; } }

        public IAPIResult Subscriber { get { return Options.Subcriber; } }
        #endregion ~Public Properties
    }
    /// <summary>
    /// Mandatory User Interactive Public Methods
    /// </summary>
    public partial class APIConfiguration
    {
        #region ^Constructor
        public APIConfiguration(APIConfigurationOptions options)
        {
            Options = options;
            Initialize(() => { var inz = options.Type.Equals("XML", StringComparison.CurrentCultureIgnoreCase) ? InitializeXML() : InitializeJson(); });
        }
        #endregion ~Constructor

        #region ^API Calling Sequences
        /// <summary>
        /// Sequencial call to all API in a Enumerable.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IAPIProspect> ExecuteApis()
        {
            foreach (var api in ApiElements)
            {
                var res = ExecuteApi(api);
                if (_isCancelled || _isCancelledRepeat) continue;
                PostSubscription(res);
                yield return res;
            }

            //Post back POST and FINAL with IAPIResults
            PostFinalEvents();
        }

        /// <summary>
        /// Reactive Observable way. Subscription to be precisely done via IAPIResult as responses will be thrown to Its method "Responses"
        /// </summary>
        /// <param name="apiResults">IAPIResult object to get Reults there Asynchronously</param>
        public void ExecuteApisObservable()
        {
            if (Options.Subcriber == null) throw new Exception("Subscriber must be passed for Observable Execution");

            ApiElements
                .Select(api => ExecuteApi(api))
                .ToObservable(NewThreadScheduler.Default)    
                //.Catch( Observable.Return( this.Current?.Result ) )   //This actually starts a second sequence where first sequence stops in this case .Select(api => ... stops               
                .Finally(() => PostFinalEvents())                
                .Subscribe((result) => PostSubscription(result), 
                           //((exp)=> Options.Subcriber.Error<APIException>(exp as APIException, this)),   //This Stops the whole process not even second sequence is considered.
                           _apiCancellation.Token);

            //Options.Subcriber.Error<APIException>(null, this)
        }

        #endregion ~API Calling Sequences
    }
    /// <summary>
    /// Other public methods, for Users to interact
    /// </summary>
    public partial class APIConfiguration
    {
        #region ^Extra Functional methods for Usage intermediatery
        #region ^Object Param Methods
        /// <summary>
        /// Push intermediatry objects to that sequence of API calls can use it for Parameter references.
        /// When more than one API is called, one of the object within the result of first API call, might be used in second API call. this is where it is handy.
        /// </summary>
        /// <param name="obj">Any Object</param>
        public void InsertObjectParam(params object[] obj)
        {
            if (obj != null && !Cancelled()) obj.ToList().ForEach(o => { if (!UpdateObjectParam(o)) this.objectParams.Params.Add(o); });
        }

        /// <summary>
        /// Updates object param. Replaces an obj of same type with the argument type passed.
        /// </summary>
        /// <param name="obj">Any object</param>
        public bool UpdateObjectParam(object obj)
        {
            if (!Cancelled() && this.objectParams.Params.RemoveWhere(r => r.GetType() == obj.GetType()) > 0)
                return this.objectParams.Params.Add(obj);

            return false;
        }

        /// <summary>
        /// Updates object param. Finds findObj and replaces iwth replaceObj.
        /// </summary>
        /// <param name="findObj">Any object</param>
        /// <param name="replaceObj">Any object</param>
        public void UpdateObjectParam(object findObj, object replaceObj)
        {
            if (!Cancelled() && this.objectParams.Params.Remove(findObj)) this.objectParams.Params.Add(findObj);
        }

        /// <summary>
        /// Takes many objects at a time and updates based on data.
        /// </summary>
        /// <param name="objs">Any Object</param>
        public void UpdateObjectParams(params object[] objs)
        {
            foreach (var obj in objs) UpdateObjectParam(obj);
        }
        #endregion ~Object Param Methods

        #region ^Cancel Token Methods
        public void Cancel()
        {
            _isCancelled = true;
            if (_apiCancellation != null) _apiCancellation.Cancel();
        }

        public void CancelCurrentRepeat()
        {
            _isCancelledRepeat = true;
        }
        #endregion ^Cancel Token Methods

        #endregion ~Extra Functional methods for Usage intermediatery
    }

    #region ^Private methods
    /// <summary>
    /// Initiating Private methods
    /// </summary>
    public partial class APIConfiguration
    {        
        #region ^Initization of Options/XML/Json to reflect Nodes
        private void Initialize(Action apiInvoker)
        {
            //Keeping track.
            this.objectParams = new APIObjectParameter();
            InsertObjectParam(Options.ObjectParams);

            apiInvoker();
        }
        private bool InitializeXML(string xml = null)
        {
            //Try to load it from file or Parase xml string itself.
            try
            {
                xml = xml ?? Options.PathOrContent;

                var all = (WrapIOs.Exists(xml) ? XElement.Load(xml) : XElement.Parse(xml)).Elements();

                Base = new APIXmlNode(all.Where(n => n.Name == "Base").First(), null);

                ApiElements = from elem in all.Where(n => n.Name == "APIProspect")
                              let order = (elem.Attribute("Order")?.Value ?? "0").ToInt()
                              orderby order
                              select elem;

                InsertRepeats(); //Taking ownership of repeat.
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Inserts XElement of the same type where attribute "Repeat" is faciliated. The API will be called that many times yet you can control the results based on parameters on Subscribed events.
        /// </summary>
        private void InsertRepeats()
        {
            if (Options.NoRepeat) return;

            var _inserts = from elem in ApiElements
                           let repeat = elem.Attribute("Repeat")?.Value.ToInt()
                           where repeat > 0 select new { Repeat = repeat, Element = elem };

            var allElements = ApiElements.ToList();
            _inserts.All(_ins =>
            {
                for (int r = 1; r < _ins.Repeat; r++) allElements.Insert(allElements.IndexOf(_ins.Element), _ins.Element);
                return true;
            });

            ApiElements = from element in allElements select element;
        }
        private bool InitializeJson()
        {
            string json = WrapIOs.Exists(Options.PathOrContent) ? WrapIOs.ReadAllLines(Options.PathOrContent).ToList().JoinExt(Environment.NewLine) : Options.PathOrContent;
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
            string _prospectXml = $"<APIProspect Name=\"{{0}}\" BaseUrl=\"{{1}}\" Uri=\"{{2}}\" Method=\"{{3}}\" IncludeKeyFromBase=\"{{4}}\" GenericType=\"{{5}}\" Token=\"{{6}}\" ContentTypes=\"{{7}}\" Repeat=\"{{8}}\">{_nl}";
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
                                                       _prospect["ContentTypes"]?.ToString(),
                                                       _prospect["Repeat"]?.ToString());
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
    }

    /// <summary>
    /// Subscriber/Subscription Private methods.
    /// </summary>
    public partial class APIConfiguration
    {
        /// <summary>
        /// After every API Execution, this method is called to post back responses back to Subcribed Class (which user has provided)
        /// </summary>
        private void PostSubscription(IAPIProspect result)
        {
            if (Options.Subcriber != null)
            {
                if (result is APIException)
                    Options.Subcriber.Error(result as APIException, this);
                else
                    Options.Subcriber.Reponses(result, this);
            }
        }
        
        
        #region ^End Subcription to Raise. Post and Final
        /// <summary>
        /// At the end, Post and Final Subscription to be posted back to the caller.
        /// </summary>
        /// <param name="apiResults"></param>
        private void PostFinalEvents()
        {
            Task.Factory.StartNew(() => Dispose())
                        .ContinueWith(antecendent => Options.Subcriber.Post(this.ProspectResults))
                        .ContinueWith(antecendent => Options.Subcriber.Final(Apis.Last().Result))
                        .Wait();
        }
        #endregion
    }

    /// <summary>
    /// API Extraction and Execution private methods
    /// </summary>
    public partial class APIConfiguration
    {
        #region ^API related information, Extract
        /// <summary>
        /// Executes API
        /// First creates XML NODE object, Creates instances of APIProspect<> and APIUti<> and then calls 'Call' method of APIUtil for synchronous call.
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        private IAPIProspect ExecuteApi(XElement api)
        {
            if (CreateApiNode(api) != null)
            {
                var node = Current;
                Apis.Add(node);
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

                try
                {
                    var method = constructedType.GetMethod("Call");
                    var res = method.Invoke(apiObject, null);                    
                    node.Result = (IAPIProspect)res;
                }
                catch(Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is APIException) node.Result = (APIException)ex.InnerException;                    
                }
                

                return node.Result;
            }
            return null;
        }

        private APIXmlNode CreateApiNode(XElement api)
        {
            if (Cancelled()) return null;                    //Cancellation Token
            var node = new APIXmlNode(api, Base);
            if (CancelledRepeat(node)) return null;          //Cancellation for Repeat

            return node;
        }

        private APIAuthorization Authorization(APIXmlNode node)
        {
            if (node.Token.Empty()) return null;

            var auth = new APIAuthorization { IsTokenAHeader = node.TokenAsHeader, Token = LocateDynamicParamValue(node.Token, false) };

            return auth;
        }

        private APIRequestHeaders ContentTypes(APIXmlNode node)
        {
            var content = new APIRequestHeaders
                            {
                                AcceptContentTypes = node.ContentTypes?.SplitEx(';'),
                                ParameterContentType = node.ParamContentType,
                                Headers = node.Headers?.Select(h => new APINamePareMedia { Key = h.Key, Value = h.Value })?.ToArray()
                            };

            return content;
        }

        private Dictionary<string, string> InjectObjectParams(APIXmlNode node)
        {
            var parameters = node.Parameters?.Keys.ToList();

            return parameters == null ?
                        node.Parameters :
                        ((Func<Dictionary<string, string>>)(() =>
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
                        objectParams.Params.FirstOrDefault(o => o.GetType().Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase)) :
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
        #endregion ~API realted information, Extract
    }

    /// <summary>
    /// Private Cancellation Check methods.
    /// </summary>
    public partial class APIConfiguration
    {
        #region ^Cancellation Token Notification.
        /// <summary>
        /// Check to see if User has requested for Cancellation.
        /// </summary>
        /// <returns></returns>
        private bool Cancelled()
        {
            return _isCancelled;
        }

        /// <summary>
        /// Checks if Cancellation for Repeated API is requested. 
        /// If YES, it would check the current APINODE and compare the name against it, found to be same Cancells the Call.
        ///    If happen to be different Name, it would negate the cancellation for future and puts executing APINODE to Current.
        /// </summary>
        /// <param name="node">APIXML Node object</param>
        /// <returns></returns>
        private bool CancelledRepeat(APIXmlNode node)
        {
            if (_isCancelledRepeat)
                if (node.Name == Current?.Name) return true;

            Current = node; _isCancelledRepeat = false;
            return false;
        }
        #endregion ~End of Cancellation Token Notification.
    }

    #endregion ~End of Private methods.
}
