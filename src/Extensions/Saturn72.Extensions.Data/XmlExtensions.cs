#region

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

            return att == null ? null : att.Value;
        }

        public static string GetAttributeValue(this XElement element, string attributeName)
        {
            return element.Attribute(attributeName).Value;
        }

        public static string GetInnerElementValue(this XElement source, string innerElementName)
        {
            if (source == null)
                return string.Empty;

            var innerElement = source.Element(innerElementName);
            return innerElement == null ? string.Empty : innerElement.Value;
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