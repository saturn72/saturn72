
using System.Configuration;
using System.Xml;
using Saturn72.Extensions;

namespace Saturn72.Core.Configuration
{
    public abstract class ConfigurationSectionHandlerBase : IConfigurationSectionHandler
    {
        public abstract object Create(object parent, object configContext, XmlNode section);

        protected virtual T GetAttributeValueOrDefault<T>(XmlNode node, string attributeName, T defaultValue)
        {
            XmlAttribute att;
            if (node.IsNull()
                || node.Attributes.IsNull()
                || (att = node.Attributes[attributeName]).IsNull())
                return defaultValue;

            return Convert<T>(att.Value);
        }

        protected virtual T GetNodeValueOrDefault<T>(XmlNode node, T defaultValue)
        {
            return node.IsNull() ? defaultValue : Convert<T>(node.InnerText);
        }

        private static T Convert<T>(string value)
        {
            var converter = CommonHelper.GetCustomTypeConverter(typeof(T));
            return (T)converter.ConvertFrom(value);
        }

        protected virtual TConfigSectionHandler LoadConfigSection<TConfigSectionHandler>(object parent,
            object configContext, XmlNode section, bool throwOnNull = false)
            where TConfigSectionHandler : class, IConfigurationSectionHandler, new()
        {
            if (section.NotNull())
                return new TConfigSectionHandler().Create(parent, configContext, section) as TConfigSectionHandler;

            if (throwOnNull)
                throw new ConfigurationErrorsException("Failed to find configuration section related to type: " +
                                                       typeof(TConfigSectionHandler));
            return null;
        }
    }
}