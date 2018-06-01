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
    public class APINodeBase
    {
        public string Name { get; set; }
        public string GenericType { get; set; }
        public string BaseUrl { get; set; }
        public string ApiUri { get; set; }
        public bool RequiredAuthorization { get; set; }
        public APIAuthenticationType AuthenticationType { get; set; }
        public string Token { get; set; }
        public bool TokenAsHeader { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Sepration to be done via semicolon at XML
        /// application/json[;application/jpeg[;...]]
        /// </summary>
        public string ContentType { get; set; }
        public APIMethod Method { get; set; }
    }

    internal class APIXmlNode : APINodeBase
    {       
        internal IAPIProspect Result { get; set; }

        internal APIXmlNode(XElement element, string baseUrl)
        {
            //<!-- Name & Type -->
            if (element.Attribute("Name") != null) Name = element.Attribute("Name").Value;
            if (element.Attribute("GenericType") != null) GenericType = element.Attribute("GenericType").Value;

            //<!-- Urls & Methods -->
            if (element.Attribute("BaseUrl") != null) BaseUrl = element.Attribute("BaseUrl").Value;
            if (element.Attribute("Uri") != null) ApiUri = element.Attribute("Uri").Value;
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
            if (paramss != null) Parameters = CreateDictionary(paramss);

            //<!-- Extra Headers -->
            var headers = element.Element("Headers");
            if (headers != null) Parameters = CreateDictionary(headers);

            //<!-- Content Type (To be seprated by semi colon)-->
            if (element.Attribute("ContentType") != null) ContentType = element.Attribute("ContentType").Value;

            //At the end Perform Validity Check and include Default Values.
            ValidityChecks(baseUrl);
        }

        private Dictionary<string, string> CreateDictionary(XElement element)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach(var item in element.Elements())
            {
                if (item.Attribute("Key") != null && item.Attribute("Value") != null)
                    dict.Add(item.Attribute("Key").Value, item.Attribute("Value").Value );
            }

            return dict;
        }

        private void ValidityChecks(string baseUrl)
        {
            if (baseUrl.Empty()) BaseUrl = baseUrl;
            if (!Token.Empty()) RequiredAuthorization = true;
        }
    }   
}
