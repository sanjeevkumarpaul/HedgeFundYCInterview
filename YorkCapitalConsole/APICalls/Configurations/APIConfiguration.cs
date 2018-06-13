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
using APICalls.Entities.Interfaces;
using Extensions;
using Wrappers;
using JsonSerializers;
using System.Threading;
using APICalls.Entities.Contracts;
using APICalls.Dependents;
using APICalls.Bases;
#endregion ~Custom Namespaces

namespace APICalls.Configurations
{
    /// <summary>
    /// Public class
    /// </summary>
    public sealed partial class APIConfiguration : IDisposable
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
        private object lockobj = new object();
        #endregion ~Required Objects for the App.

        #region ^Required variables for the App.
        private bool _isCancelled = false;
        private bool _isCancelledRepeat = false;
        private bool _isParallel = false;
        private CancellationTokenSource _apiCancellation = new CancellationTokenSource();
        #endregion ^Required variables for the App.
    }
    /// <summary>
    /// Required Public Properties
    /// </summary>
    public partial class APIConfiguration
    {
        #region ^Public Properties
        public IEnumerable<IAPIProspect> ProspectResults { get { return ProspectResultsFiltered().Select(a => a.Result); } }

        public IAPIResult Subscriber { get { return Options.Subscriber; } }
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
                yield return res.Result;
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
            if (Options.Subscriber == null) throw new Exception("Subscriber must be passed for Observable Execution");

            ApiElements
                .Select(api => ExecuteApi(api))
                .ToObservable(NewThreadScheduler.Default)    
                //.Catch( Observable.Return( this.Current?.Result ) )   //This actually starts a second sequence where first sequence stops in this case .Select(api => ... stops                
                .Finally(() => PostFinalEvents())                
                .Subscribe((output) => PostSubscription(output), 
                           //((exp)=> Options.Subcriber.Error<APIException>(exp as APIException, this)),   //This Stops the whole process not even second sequence is considered.
                           _apiCancellation.Token);           
        }

        /// <summary>
        /// All of the API(s) listed at XML/JSON will run parallely 
        /// AND ...
        /// 1. It will not adhere any REPEATS  
        /// 2. It will only try to input parameter fields frm the OBJECT PARAMS passed at APICONFIGOPTIONS at Constructor
        /// </summary>
        /// <returns>Task, which is kind of a Void.</returns>        
        public async Task<List<string>> ExecuteApisParallel()
        {
            _isParallel = true;
            List<Task<IAPIProspect>> taskProspects = new List<Task<IAPIProspect>>();
            var progress = SubscribeProgress();

            await ExecuteParallel(progress);
            
            PostFinalEvents();

            return new List<string>();
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
            if (obj != null && !Palleled() && !Cancelled()) obj.ToList().ForEach(o => { if (!UpdateObjectParam(o)) this.objectParams.Params.Add(o); });
        }

        /// <summary>
        /// Updates object param. Replaces an obj of same type with the argument type passed.
        /// </summary>
        /// <param name="obj">Any object</param>
        public bool UpdateObjectParam(object obj)
        {
            if (!Palleled() && !Cancelled() && this.objectParams.Params.RemoveWhere(r => r.GetType() == obj.GetType()) > 0)
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
            if (!Palleled() && !Cancelled() && this.objectParams.Params.Remove(findObj)) this.objectParams.Params.Add(findObj);
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
            Options.Validate();
            
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
                
                ApiElements = OrderProspectNodes(all);

                InsertRepeats(); //Taking ownership of repeat.
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reassigns the correct order for APIs
        /// </summary>
        /// <param name="all">IEnum of XElement containg API</param>
        /// <returns></returns>
        private IEnumerable<XElement> OrderProspectNodes(IEnumerable<XElement> all)
        {
            var _prospects = all.Where(n => n.Name == "APIProspect").OrderBy(p => (p.Attribute("Order")?.Value ?? "0").ToFloat());            
            var _orderList = _prospects.Where(p => (p.Attribute("Order")?.Value ?? "0").ToFloat() > 0.0).Select(p => (p.Attribute("Order")?.Value ?? "0").ToFloat());
            var _start = _orderList.Count() <= 0 ? 0 : _orderList.Max();

            if (_orderList.Count() < _prospects.Count()) /*only when there is no order for one of the candidate.*/
            {
                if (_start <= 0) _start++;
                var _count = _prospects.Count() + _start;
              
                for (var index = _start; index < _count; index++)
                {
                    var _elem = _prospects.ElementAt(0); //Always Zero(0) since _prospect is sorted IEnum and once Order Attribute is filled it adjusts automatically.
                    if (!_orderList.Contains(index)) //_ordereList fills up as and when 'Order' attribute gets an value.
                        SetOrderElement(_elem, index);
                }

                //only if Token master is assigned check through elements and assign it to be the first.
                if (!Base.TokenMaster.Empty())
                {
                    var _elem = _prospects.FirstOrDefault(p => (p.Attribute("Name")?.Value ?? "").Equals(Base.TokenMaster, StringComparison.CurrentCultureIgnoreCase));                    
                    SetOrderElement(_elem, 0);
                }
            }
            return _prospects;
        }

        /// <summary>
        /// Sets/Assigns Order Attribute for XElement to a float value
        /// </summary>
        /// <param name="element">XElement</param>
        /// <param name="value">float value</param>
        private void SetOrderElement(XElement element, float value)
        {
            if (element != null)
            {
                if (element.Attribute("Order") != null)
                    element.Attribute("Order").Value = value.ToString();
                else
                    element.Add(new XAttribute("Order", value.ToString()));
            }
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
                var _elem = new XElement(_ins.Element); //Deep Copy
                var _order =_ins.Element.Attribute("Order").Value;
                for (int r = 1; r < _ins.Repeat; r++)
                {
                    SetOrderElement(_elem, ($"{_order}.{r}").ToFloat());
                    allElements.Insert(allElements.IndexOf(_ins.Element), _elem);
                }
                return true;
            });

            ApiElements = from element in allElements.OrderBy(a => a.Attribute("Order").Value.ToFloat()) select element;
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
            string _baseXml = $"<Base Name=\"{{0}}\" BaseUrl=\"{{1}}\"  Key=\"{{2}}\" TokenMaster=\"{{3}}\" />{_nl}";
            string _prospectXml = $"<APIProspect Name=\"{{0}}\" BaseUrl=\"{{1}}\" Uri=\"{{2}}\" Method=\"{{3}}\" IncludeKeyFromBase=\"{{4}}\" GenericType=\"{{5}}\" Token=\"{{6}}\" ContentTypes=\"{{7}}\" Repeat=\"{{8}}\" Order=\"{{9}}\" Cache=\"{{10}}\">{_nl}";
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
                    xml += string.Format(_baseXml, _base["Name"]?.ToString(), 
                                                   _base["BaseUrl"]?.ToString(), 
                                                   _base["Key"]?.ToString(), 
                                                   _base["TokenMaster"]?.ToString());
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
                                                       _prospect["Repeat"]?.ToString(),
                                                       _prospect["Order"]?.ToString(),
                                                       _prospect["Cache"]?.ToString());
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
        /// Parallel execution needs little bit of extra functionality. Where Parallel Loop is executed under Task.Run() in a awaited fashion to  
        /// let progress client do its other duties and Subscription Events and emited in each process along with progress event
        /// to let client know the processing and if required accept cancelation switch.
        /// </summary>
        /// <param name="progress">IProgress type to let progress event fire at client after each Api execution</param>
        /// <returns>List Of all IAPIProspects Results</returns>
        private async Task<List<IAPIProspect>> ExecuteParallel(IProgress<IAPIParallelProgress> progress)
        {
            IAPIParallelResult _result = Options.SubscriberParallel;

            await Task.Run(() =>
            {
                Parallel.ForEach<XElement>(ApiElements, (api) =>
                {
                    var parameters = _result?.ParallelStart();
                    var res = ExecuteApiAsync(api, parameters);

                    lock (lockobj) //Critcal Area.
                    {
                        Apis.Add(res);
                    }                    
                    PostSubscription(res);                    
                });
            });
            #region ^example of exxception handlin via tasks.
            //To catch any exeception within Tasks we need to do like below
            /*
                  try { var results = await Task.WhenAll(taskProspects); }
                  catch(AggregateException ae)
                  {
                     e.Handle(x) =>
                     {
                         if (x is UnauthorizedAccessException) // This we know how to handle.
                         {
                             Console.WriteLine("You do not have permission to access all folders in this path.");
                             Console.WriteLine("See your network administrator or try another path.");
                             return true;
                          }
                          return false; // Let anything else stop the application.
                      });

                      //   OR    //

                      var x = e.Flatten(); //This way it flattens all the exception from all taks and then can be re thrown like - throw x;
                  }
             */
            #endregion ~example of exxception handlin via tasks.

            return ProspectResults.ToList();
        }

        /// <summary>
        /// Creates Progress object and Assigns Event Raiser to it.
        /// </summary>
        /// <returns>IProgress object</returns>
        private Progress<IAPIParallelProgress> SubscribeProgress()
        {
            if (Options.Progessor != null)
            {
                Progress<IAPIParallelProgress> progress = new Progress<IAPIParallelProgress>();
                progress.ProgressChanged += (sender, target) => { Options.SubscriberParallel?.ParallelProgress(target); }; //anaymously handle the event

                return progress;
            }
            return null;
        }

        /// <summary>
        /// Raises Progress event at the Parallelism, only after checking if progress is not null.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="progress"></param>
        private void RaiseProgress(APIXmlNode node, IProgress<IAPIParallelProgress> progress)
        {
            if (progress == null) return;

            var _progress = Options.Progessor;
            _progress.Url = node.BaseUrl;
            _progress.Percentage = ((Apis.Count * 100) / ApiElements.Count());
            progress.Report(_progress);
        }
        
        /// <summary>
        /// After every API Execution, this method is called to post back responses back to Subcribed Class (which user has provided)
        /// </summary>
        private void PostSubscription(APIXmlNode node, IProgress<IAPIParallelProgress> progress = null)
        {
            RaiseProgress(node, progress);

            if (Options.Subscriber != null && !node.Name.Equals(Base.TokenMaster, StringComparison.CurrentCultureIgnoreCase))
            {
                var _config = _isParallel ? null : this; //ApiConfiguraion is only passed if performed non Parallelled.

                if (node.Result is APIException)
                    Options.Subscriber.Error(node.Result as APIException, _config);
                else
                    Options.Subscriber.Reponses(node.Result, _config); 
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
                        .ContinueWith(antecendent => Options.Subscriber.Post(this.ProspectResults))
                        .ContinueWith(antecendent => Options.Subscriber.Final(ProspectResultsFiltered().Last().Result))
                        .Wait();
        }

        /// <summary>
        /// Retrieves all APIProspect rsults except the ones with TokenMater is labled to.
        /// </summary>
        /// <returns>Enumerable of ApiXmlNode</returns>
        private IEnumerable<APIXmlNode> ProspectResultsFiltered()
        {
            return Apis.
                   Where(a => !a.Name.Equals(Base.TokenMaster, StringComparison.CurrentCultureIgnoreCase));
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
        private APIXmlNode ExecuteApi(XElement api)
        {
            if (CreateApiNode(api) != null)
            {
                var node = Current;
                Apis.Add(node);
                return ExecuteApiAsync(api, null, node);
            }
            return null;
        }
        /// <summary>
        /// Same as above with soem Asyn Consideration.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="otherParmams"></param>
        /// <param name="createdNode"></param>
        /// <returns></returns>
        private APIXmlNode ExecuteApiAsync(XElement api, object[] otherParmams = null, APIXmlNode createdNode = null)
        {
            var node = createdNode ?? new APIXmlNode(api, Base);
            var prospect = CreateAndInstantiateProspectNode(node, otherParmams);
            object apiUtil = null;
            var method = GetApiCallMethod(node, prospect, ref apiUtil);

            InvokeApiMethod(node, method, apiUtil);

            return node;
        }
        /// <summary>
        /// Creates instances of APIProspect<IAPIProspect>, IAPIProspect being any Type Derived from it.
        /// </summary>
        /// <param name="node">APIXmlNode</param>
        /// <param name="otherParmams">any object, to be used as Object Parameters. Genrally passed during Parallel Processing.</param>
        /// <returns>APIProspect<IAPIProspect></returns>
        private object CreateAndInstantiateProspectNode(APIXmlNode node, object[] otherParmams = null)
        {
            var prospect = CreateInstance(node.GenericType, typeof(APIProspect<>));

            using (var prosBase = (APIProspectOptionBase)prospect)
            {
                prosBase.BaseUrl = node.BaseUrl;
                prosBase.APIUri = LocateDynamicParamValue(node.ApiUri, otherParmams: otherParmams);
                prosBase.Method = node.Method;
                prosBase.Parameters = InjectObjectParams(node, otherParmams: otherParmams);
                prosBase.ParametersIsQueryString = node.ParametersAsQueryString;
                prosBase.Authorization = Authorization(node);
                prosBase.RequestHeaders = ContentTypes(node);
            }

            return prospect;
        }
        /// <summary>
        /// Creates APIUtil<IAPIProspect>, IAPIProspect being any Type Derived from it.
        /// </summary>
        /// <param name="node">APIXmlNode</param>
        /// <param name="prospect">IAPIProspect</param>
        /// <param name="apiUtil">object reference to APIUtil is created and passes the pointer back to caller.</param>
        /// <returns>Returns 'CALL' method refernce for APIUtil<IAPIProspect></returns>
        private System.Reflection.MethodInfo GetApiCallMethod(APIXmlNode node, object prospect, ref object apiUtil)
        {
            var constructedType = CreateGenericType(node.GenericType, typeof(APIUtil<>));
            apiUtil = CreateInstance(node.GenericType, typeof(APIUtil<>), prospect);

            var method = constructedType.GetMethod("Call");

            return method;
        }
        /// <summary>
        /// Safely invokes APIUtil<IAPIProspect> CALL method and result stored in APIXmlNode.Results
        /// Also catches any exception and stores at the same location.
        /// </summary>
        /// <param name="node">APIXmlNode</param>
        /// <param name="method">MethodInfo for 'CALL' of the object APIUtil<IAPIProspect></param>
        /// <param name="apiUtil">APIUtil<IAPIProspect> itself.</param>
        private void InvokeApiMethod(APIXmlNode node, System.Reflection.MethodInfo method, object apiUtil)
        {
            try
            {
                var res = method.Invoke(apiUtil, null);
                node.Result = (IAPIProspect)res;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is APIException) node.Result = (APIException)ex.InnerException;
            }
        }
        /// <summary>
        /// Checks Cancellation and Creates APIXmlNode from XElement(Api reference from either Json/Xml)
        /// </summary>
        /// <param name="api">XElement</param>
        /// <returns>APIXmlNode</returns>
        private APIXmlNode CreateApiNode(XElement api)
        {
            if (Cancelled()) return null;                    //Cancellation Token
            var node = new APIXmlNode(api, Base);
            if (CancelledRepeat(node)) return null;          //Cancellation for Repeat

            return node;
        }
        /// <summary>
        /// APIAuthorization type is created and passed on to the caller from APIXmlNode type.
        /// </summary>
        /// <param name="node">APIXmlNode</param>
        /// <returns>APIAuthorization</returns>
        private APIAuthorization Authorization(APIXmlNode node)
        {
            if (node.Token.Empty()) return null;

            var auth = new APIAuthorization { IsTokenAHeader = node.TokenAsHeader, Token = LocateDynamicParamValue(node.Token, false) };

            return auth;
        }
        /// <summary>
        /// Creates APIRequestHeaders from APIXmlNode off the XElement
        /// This consits of ContentTypes, Content Type for Parameters, and any other Header information.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Any {Type.Property} under XElement (Json/Xml) needs to be parsed to actual Object Parameters
        /// and so as this method used for.
        /// Either ObjectParameters or otherParams are considered to create a Dictionary<string,string>
        ///   and calls LocatedDynamicParamValue to replace the placeholder {Type.Proeprty}
        /// </summary>
        /// <param name="node">APIXmlNode</param>
        /// <param name="otherParmams">Array of objects, usually passed during parallel processing</param>
        /// <returns></returns>
        private Dictionary<string, string> InjectObjectParams(APIXmlNode node, object[] otherParmams = null)
        {
            var parameters = node.Parameters?.Keys.ToList();

            return parameters == null ?
                        node.Parameters :
                        ((Func<Dictionary<string, string>>)(() =>
                     {
                         Dictionary<string, string> dictReplacers = new Dictionary<string, string>();
                         parameters.ForEach(k => dictReplacers.Add(k, LocateDynamicParamValue(node.Parameters[k], otherParmams: otherParmams)));

                         return dictReplacers;
                     }))();
        }
        /// <summary>
        /// Finds {Type.Property} from place holder strings and loads values into it via GetDynamicParamObjectValue.
        /// </summary>
        /// <param name="placeholderStr">Placeholder string, usually Token or Parameters of an API</param>
        /// <param name="locateFromObjectParams">To make sure to be found from Object Params</param>
        /// <param name="otherParmams">Array of objects, usually passed during parallel processing</param>
        /// <returns>Replaced Placeholder string with right values. If no values find blanks out Placeholder string and returns back.</returns>
        private string LocateDynamicParamValue(string placeholderStr, bool locateFromObjectParams = true, object[] otherParmams = null)
        {
            //Find all matches with {...}
            Regex.Matches(placeholderStr, "{(.*)}").Cast<Match>().All(m =>
            {
                var item = m.Groups[1].Value.SplitEx('.'); //Groups[1] stores the group value like (.*). If there are more, more groups are created.

                if (item.Length > 1)
                    placeholderStr = GetDynamicParamObjectValue(item[0], placeholderStr, m.ToString(), item[1], locateFromObjectParams, otherParmams);

                return true;
            });

            return placeholderStr;
        }
        /// <summary>
        /// Replaces placeholder string ({Type.Property}) with right values.
        /// </summary>
        /// <param name="typeName">Typename towards which .Property to be extracted within {Type.Property}</param>
        /// <param name="placeholderStr">Placeholder string, usually Token or Parameters of an API</param>
        /// <param name="pattern">string of patter which contains exactly {Type.Property}</param>
        /// <param name="propertyName"></param>
        /// <param name="locateFromObjectParams">To make sure to be found from Object Params</param>
        /// <param name="otherParams">Array of objects, usually passed during parallel processing</param>
        /// <returns>Replaced Placeholder string with right values. If no values find blanks out Placeholder string and returns back.</returns>
        private string GetDynamicParamObjectValue(string typeName, string placeholderStr, string pattern, string propertyName, bool locateFromObjectParams = true, object[] otherParams = null)
        {
            var _objects = GetParamObjects(otherParams);

            var val = (locateFromObjectParams ?
                        _objects.FirstOrDefault(o => o.GetType().Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase)) :
                        Apis.Where(n => n.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault()?.Result)
                      ?.GetVal(propertyName);

            return placeholderStr.Replace(pattern, val);
        }
        /// <summary>
        /// Evaluates Object Parameters from ObjectParams object and otherParams, eleminats duplicate(s) and gives right objects.
        /// </summary>
        /// <param name="otherParams">Array of objects, usually passed during parallel processing</param>
        /// <returns>List of mapped distinct Array of objects.</returns>
        public List<object> GetParamObjects(object[] otherParams = null)
        {
            List<object> objects = new List<object>();
            objects.AddRange(objectParams.Params.ToArray());

            if (otherParams != null)
            {
                foreach (var other in otherParams)
                {
                    object obj = null;
                    if ((obj = objects.Find(o => o.GetType() == other.GetType())) != null)
                        objects.Remove(obj);
                    objects.Add(other);
                }
            }
            return objects;
        }
        /// <summary>
        /// Create Generic Type from fully qualified Namespace.Type sent. Either APProspect<IAPIProspect> or APIUtil<IAPIProspect>
        /// </summary>
        /// <param name="prospectType">fuly qualified Namespace.Type</param>
        /// <param name="genericType">Typeof GenericType to be qualifeid against </param>
        /// <returns>Type of Generics constructed</returns>
        private Type CreateGenericType(string prospectType, Type genericType)
        {
            Type customType = Type.GetType(prospectType);
            Type constructedType = genericType.MakeGenericType(customType);

            return constructedType;
        }
        /// <summary>
        /// Creates Generic Type and Actual instance of the same type by calling parameterised constructors.
        /// </summary>
        /// <param name="prospectType">Either APProspect<IAPIProspect> or APIUtil<IAPIProspect></param>
        /// <param name="genericType">Typeof GenericType to be qualifeid against</param>
        /// <param name="parameters"></param>
        /// <returns>Object Generics constructed</returns>
        private object CreateInstance(string prospectType, Type genericType, params object[] parameters)
        {
            Type constructedType = CreateGenericType(prospectType, genericType);
            var realtype = Activator.CreateInstance(constructedType, parameters);

            return realtype;
        }
        #endregion ~API realted information, Extract
    }

    /// <summary>
    /// Private Cancellation/Parallel Check methods.
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

        private bool Palleled()
        {
            return _isParallel;
        }
    }

    #endregion ~End of Private methods.
}
