#region

using System;
using System.Xml;
using System.Xml.Linq;

#endregion

namespace Saturn72.Extensions
{
    public static class XmlExtensions
    {
        public static string GetAttributeValueOrDefault(this XElement element, string attributeName)
        {
            var att = element.Attribute(attributeName);

            return att?.Value;
        }

        public static string GetAttributeValue(this XElement element, string attributeName)
        {
            return element.Attribute(attributeName).Value;
        }

        public static string GetAttributeValue(this XmlNode node, string attributeName)
        {
            return node.Attributes[attributeName].Value;
        }

        public static string GetAttributeValueOrDefault(this XmlNode node, string attributeName)
        {
            var att = node.Attributes[attributeName];

            return att?.Value;
        }

        public static string GetInnerElementValue(this XElement source, string innerElementName)
        {
            if (source == null)
                throw new NullReferenceException("");

            var innerElement = source.Element(innerElementName);
            if (innerElement == null)
                throw new NullReferenceException();

            return innerElement.Value;
        }
        public static string GetInnerElementValue(this XmlNode source, string innerElementName)
        {
            if (source == null)
                throw new NullReferenceException("");

            var innerElement = source.SelectSingleNode(innerElementName);
            if (innerElement == null)
                throw new NullReferenceException();

            return innerElement.InnerText;
        }

        public static XmlNode ToXmlNode(this XElement source, XmlDocument xmlDoc = null)
        {
            using (var xmlReader = source.CreateReader())
            {
                if (xmlDoc == null) xmlDoc = new XmlDocument();
                return xmlDoc.ReadNode(xmlReader);
            }
        }
    }
}