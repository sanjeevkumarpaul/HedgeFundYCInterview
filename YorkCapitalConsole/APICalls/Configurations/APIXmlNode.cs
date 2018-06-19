using APICalls.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Extensions;
using APICalls.Entities.Interfaces;
using APICalls.Bases;
using APICalls.Enum;
using APICalls.Dependents;
using APICalls.Configurations.Filters;

namespace APICalls.Configurations
{
    internal class APIXmlNode : APINodeBase
    {
        private const string Prefix = "__.API.__";
        private const string Postfix = "__.__";

        internal IAPIProspect Result { get; set; }

        internal APIXmlNode(XElement element, APIXmlNode Base)
        {
            //<!-- Name & Type -->
            if (element.Attribute("Name") != null) Name = $"{element.Attribute("Name").Value}";
            if (element.Attribute("ResultType") != null) ResultType = element.Attribute("ResultType").Value;

            //<!-- Urls & Methods -->
            if (element.Attribute("BaseUrl") != null) BaseUrl = element.Attribute("BaseUrl").Value;
            if (element.Attribute("Uri") != null) ApiUri = element.Attribute("Uri").Value;
            if (element.Attribute("Method") != null) Method = element.Attribute("Method").Value.ToEnum<APIMethod>();

            //<!-- Key -->
            if (element.Attribute("Key") != null) ApiKey = element.Attribute("Key").Value;
            if (element.Attribute("IncludeKeyFromBase") != null) IncludeKeyFromBase = element.Attribute("IncludeKeyFromBase").Value;
            if (element.Attribute("KeyPlacement") != null) KeyPlacement = element.Attribute("KeyPlacement").Value.ToUpper().ToEnum(APIKeyPlacement.QUERYSTRING);                               
            
            //<!-- Cache -->
            if (element.Attribute("Cache") != null) Cache = element.Attribute("Cache").Value.ToBool();

            //<!-- Authorization -->
            var auth = element.Element("Authorization");
            if (auth != null )
            {                
                if (auth.Attribute("Type") != null) AuthenticationType = auth.Attribute("Type").Value.ToEnum<APIAuthenticationType>();
                if (auth.Attribute("Token") != null) Token = auth.Attribute("Token").Value;
                if (auth.Attribute("TokenAsHeader") != null) TokenAsHeader = auth.Attribute("TokenAsHeader").Value.ToBool();                
            }
            if (element.Attribute("TokenMaster") != null) TokenMaster = $"__API__{element.Attribute("TokenMaster").Value}__.__";

            //<!-- Paramters -->
            var Parameters = GetParameters(element.Elements("Parameters"), Base);
            if (element.Attribute("ParameterContentType") != null) ParameterContentType = element.Attribute("ParameterContentType").Value;

            //<!-- Extra Headers -->
            var headers = element.Element("Headers");
            if (headers != null) Headers = CreateDictionary(headers, Base, false, true);

            //<!-- Additional Content Types (To be seprated by semi colon)-->
            var contents = element.Element("ContentTypes");
            if (contents != null && contents.Attribute("Values") != null) ContentTypes = contents.Attribute("Values").Value;

            //<!-- Parameter Conditions -->
            var conditions = GetConditions(element.Element("Filters"));


            //At the end Perform Validity Check and include Default Values.
            ValidityChecks(Base?.BaseUrl);
        }

        private Dictionary<string, string> CreateDictionary(XElement element, APIXmlNode Base = null, bool Querystring = false, bool header = false)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            
            if (element != null)
            {                
                foreach (var item in element.Elements())
                {
                    if (item.Attribute("Key") != null && item.Attribute("Value") != null)
                        dict.Add(item.Attribute("Key").Value, item.Attribute("Value").Value);
                }
            }
            if (!IncludeKeyFromBase.Empty() && Base != null)
            {
                if ((KeyPlacement == APIKeyPlacement.QUERYSTRING && Querystring && !header) || 
                    (KeyPlacement == APIKeyPlacement.BODY && !Querystring && !header) || 
                    (KeyPlacement == APIKeyPlacement.HEADER && !Querystring && header) )
                {
                    dict.Add(IncludeKeyFromBase, Base.ApiKey);
                    IncludeKeyFromBase = ""; //empty it there after so that it does not repeat addition.
                }
            }
            
            return dict;
        }

        private List<APIParameter> GetParameters(IEnumerable<XElement> elements, APIXmlNode Base = null)
        {
            List<APIParameter> _parameters = new List<APIParameter>();

            if (elements != null)
            {
                foreach (var pelement in elements)
                {
                    var _isQueryString =  pelement.Attribute("QueryString")?.Value?.ToBool() ?? false;

                    _parameters.Add(new APIParameter
                    {
                        ParametersAsQueryString = _isQueryString,
                        Items = CreateDictionary(pelement, Base, _isQueryString, false)
                    }); 
                }
            }

            _parameters.Where(p => p.ParametersAsQueryString).SelectMany(p => p.Items).All(i => { ParametersQuery.Add(i.Key, i.Value);  return true; }); 
            _parameters.Where(p => !p.ParametersAsQueryString).SelectMany(p => p.Items).All(i => { ParametersBody.Add(i.Key, i.Value); return true; });

            return _parameters;
        }

        /// <summary>
        /// Extract filters from the Prospect Element of XML
        /// </summary>
        /// <param name="element">XElement with Filters tag</param>
        /// <returns>List of APIFilter</returns>
        private List<APIFilter> GetConditions(XElement element)
        {
            if (element != null)
            {
                var wheres = element.Elements("Where");
                if (wheres != null)
                {
                    foreach (var whr in wheres)
                        Filters.Add(new APIFilter
                                    {
                                        ParamterKey = whr.Attribute("ParameterKey")?.Value,
                                        Default = whr.Attribute("Default")?.Value,
                                        Where = new APIWhere
                                        {
                                            AndConditions = BuiltCondition(whr),
                                            OrConditions = BuiltCondition(whr, true)
                                        }
                                    });
                }
                Filters.RemoveAll(c => c.ParamterKey.Empty() || !c.Where.Exists ); //Remove all Filters if found to be empty either Key or Condition.
            }

            //Example of Local function perfectly required at this point to extract condition logic
            List<APICondition> BuiltCondition(XElement whereCond, bool orConditions = false)
            {
                var condElements = whereCond.Element(orConditions ? "Or" : "And")?.Elements("Condition");

                if (condElements != null)
                    return (from ands in whereCond.Element(orConditions ? "Or" : "And")?.Elements("Condition")
                            select new APICondition
                            {
                                Operand = ands.Attribute("Operand")?.Value,
                                Operator = ands.Attribute("Operator")?.Value.ToEnum(APIConditionOperator.EQ),
                                Value = ands.Attribute("Value")?.Value

                            }).ToList();
                else
                    return new List<APICondition>();
            }

            return Filters;
        }

        private void ValidityChecks(string baseUrl)
        {
            if (BaseUrl.Empty()) BaseUrl = baseUrl;
            if (!Token.Empty()) RequiredAuthorization = true;
            if (Name.Empty()) { Name = $"{Prefix}{Guid.NewGuid()}{Postfix}"; Cache = false; } //Caching is not possible without a name

            if (IncludeKeyFromBase.Empty()) IncludeKeyFromBase = null;
            if (!RequiredAuthorization)
            {
                AuthenticationType = APIAuthenticationType.Basic;
                Token = null;
                TokenAsHeader = false;
            }

            if (ParametersQuery != null && ParametersQuery.Count <= 0)            
                ParametersQuery = null;


            if (ParametersBody != null && ParametersBody.Count <= 0)
            {
                ParametersBody = null;
                ParameterContentType = null;
            }

            if (Headers != null && Headers.Count <= 0) Headers = null;
            if (ContentTypes.Empty()) ContentTypes = null;
        }
    }   
}
