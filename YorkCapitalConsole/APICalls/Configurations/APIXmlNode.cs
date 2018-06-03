using APICalls.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Extensions;

namespace APICalls.Configurations
{
    internal class APIXmlNode : APINodeBase
    {       
        internal IAPIProspect Result { get; set; }

        internal APIXmlNode(XElement element, APIXmlNode Base)
        {
            //<!-- Name & Type -->
            if (element.Attribute("Name") != null) Name = element.Attribute("Name").Value;
            if (element.Attribute("GenericType") != null) GenericType = element.Attribute("GenericType").Value;

            //<!-- Urls & Methods -->
            if (element.Attribute("BaseUrl") != null) BaseUrl = element.Attribute("BaseUrl").Value;
            if (element.Attribute("Uri") != null) ApiUri = element.Attribute("Uri").Value;
            if (element.Attribute("Key") != null) ApiKey = element.Attribute("Key").Value;
            if (element.Attribute("IncludeKeyFromBase") != null) IncludeKeyFromBase = element.Attribute("IncludeKeyFromBase").Value; 
            if (element.Attribute("Method") != null) Method = element.Attribute("Method").Value.ToEnum<APIMethod>();

            //<!-- Authorization -->
            var auth = element.Element("Authorization");
            if (auth != null )
            {                
                if (auth.Attribute("Type") != null) AuthenticationType = auth.Attribute("Type").Value.ToEnum<APIAuthenticationType>();
                if (auth.Attribute("Token") != null) Token = auth.Attribute("Token").Value;
                if (auth.Attribute("TokenAsHeader") != null) TokenAsHeader = auth.Attribute("TokenAsHeader").Value.ToBool(); 
            }

            //<!-- Api Paramters -->
            var paramss = element.Element("Parameters");
            Parameters = CreateDictionary(paramss, Base);
            if (paramss != null)
            {   
                if (paramss.Element("ContentType") != null) ParamContentType = paramss.Element("ContentType").Value;
                if (paramss.Attribute("QueryString") != null) ParametersAsQueryString = paramss.Attribute("QueryString").Value.ToBool();
            }            

            //<!-- Extra Headers -->
            var headers = element.Element("Headers");
            if (headers != null) Parameters = CreateDictionary(headers);

            //<!-- Additional Content Types (To be seprated by semi colon)-->
            var contents = element.Element("ContentTypes");
            if (contents != null && contents.Attribute("Values") != null) ContentTypes = contents.Attribute("Values").Value;

            //At the end Perform Validity Check and include Default Values.
            ValidityChecks(Base?.BaseUrl);
        }

        private Dictionary<string, string> CreateDictionary(XElement element, APIXmlNode Base = null)
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
            if (!IncludeKeyFromBase.Empty() && Base != null) dict.Add(IncludeKeyFromBase, Base.ApiKey);
            
            return dict;
        }

        private void ValidityChecks(string baseUrl)
        {
            if (BaseUrl.Empty()) BaseUrl = baseUrl;
            if (!Token.Empty()) RequiredAuthorization = true;
        }
    }   
}
