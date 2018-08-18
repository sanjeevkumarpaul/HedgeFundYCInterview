using System;
using System.Xml;
using System.Xml.Linq;

namespace Extensions
{
    public static partial class ExtXml
    {
        public static string Text(this XmlNode node, string xpath)
        {
            return ((XmlElement)node).Text(xpath);
        }

        public static string Text(this XmlElement node, string xpath)
        {
            if (node == null) return "";

            var element = (xpath == node.LocalName) ? node : (XmlElement)node.SelectSingleNode(xpath);
            return (element != null ? element.InnerXml : "").Trim().UnescapeXmlNotations();
        }

        public static XmlElement Element(this XmlElement node, string xpath, string ExceptionIfNullOrNoChilden = null)
        {
            return node.SelectSingleNode(xpath).GetElement(ExceptionIfNullOrNoChilden);
        }

        public static XmlElement Element(this XmlDocument doc, string xpath, string ExceptionIfNullOrNoChilden = null)
        {
            return doc.SelectSingleNode(xpath).GetElement(ExceptionIfNullOrNoChilden);
        }

        public static XmlNodeList Elements(this XmlElement node, string xpath, string ExceptionIfNullOrNoChilden = null)
        {
            return node.SelectNodes(xpath);
        }

        public static XmlNodeList Elements(this XmlNode node, string xpath, string ExceptionIfNullOrNoChilden = null)
        {
            var nodes = node.SelectNodes(xpath);

            if (ExceptionIfNullOrNoChilden != null)
                if (!nodes.HasAny()) throw new Exception(ExceptionIfNullOrNoChilden);

            return nodes;
        }

        public static bool HasAny(this XmlElement element)
        {
            return (element != null && element.HasChildNodes);
        }


        public static string Sum(this XmlNode node, string xpath)
        {
            int sum = 0;
            foreach (XmlNode elm in node.Elements(xpath))
            {
                sum += elm.InnerXml.ToInt();
            }
            return sum.ToString();
        }

        public static bool HasAny(this XmlNodeList nodes)
        {
            return (nodes != null && nodes.Count > 0);
        }
        
        public static T Map<T>(this XElement elements) where T: class
        {
            var _t = Activator.CreateInstance<T>();

            _t.PropertyNames().ForEach(p => 
            {
                var _val = elements.Element(p)?.Value;
                if (_val != null) _t.SetProperty(p, _val);
            });

            return _t;
        }

        /////
        private static XmlElement GetElement(this XmlNode node, string ExceptionIfNullOrNoChilden = null)
        {
            if (node == null) return null;

            if (!ExceptionIfNullOrNoChilden.Empty() && (node == null || !node.HasChildNodes)) throw new Exception(ExceptionIfNullOrNoChilden);
            return (node != null) ? (XmlElement)node : null;
        }

    }
}
