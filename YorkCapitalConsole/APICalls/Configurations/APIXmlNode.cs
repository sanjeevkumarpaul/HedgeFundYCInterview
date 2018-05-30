using APICalls.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace APICalls.Configurations
{
    internal class APIXmlNode
    {
        internal string BaseUrl { get; set; }
        internal string ApiUri { get; set; }
        internal bool RequiredAuthorization { get; set; }
        internal string Token { get; set; }
        internal bool TokenAsHeader { get; set; }
        /// <summary>
        /// Sepration to be done via semicolon at XML
        /// application/json[;application/jpeg[;...]]
        /// </summary>
        internal string ContentType { get; set; }
        internal APIMethod Method { get; set; }

        internal APIXmlNode(XElement elements)
        {

        }
    }
}
