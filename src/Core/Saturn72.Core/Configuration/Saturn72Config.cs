#region

using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Saturn72.Core.Infrastructure;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Configuration
{
    public class Saturn72Config : IConfigurationSectionHandler
    {
        public const string TypeAttributeName = "type";

        private static Saturn72Config _saturn72Config;

        public string EngineDriver { get; private set; }
        public string ContainerManager { get; private set; }
        public string ConfigLoader { get; private set; }
        public IDictionary<string, string> ConfigLoaderData { get; private set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var defaultConfig = DefaultConfig();

            var setup = section.SelectSingleNode("setup");
            if (setup == null)
                return defaultConfig;

            EngineDriver = GetObjectTypeFromConfigSectionElementsAttribute("engineDriver", setup,
                defaultConfig.EngineDriver);
            ContainerManager = GetObjectTypeFromConfigSectionElementsAttribute("containerManager", setup,
                defaultConfig.ContainerManager);
            ConfigLoader = GetObjectTypeFromConfigSectionElementsAttribute("configLoader", setup,
                defaultConfig.ConfigLoader);
            ConfigLoaderData = GetConfigLoaderData(setup);
            return this;
        }

        private IDictionary<string, string> GetConfigLoaderData(XmlNode section)
        {
            var elem = GetElement("configLoaderData", section);
            if (elem.IsNull() || !elem.HasChildNodes)
                return null;

            var result = new Dictionary<string, string>();

            var cn = elem.FirstChild;
            while (cn != null)
            {
                var cnAtts = cn.Attributes;
                result.Add(cnAtts["key"].Value, cnAtts["value"].Value);
                cn = cn.NextSibling;
            }
            return result;
        }

        private string GetObjectTypeFromConfigSectionElementsAttribute(string elementName, XmlNode sectionXmlNode,
            string defaultValue)
        {
            var element = GetElement(elementName, sectionXmlNode);
            if (element != null && element.Attributes != null)
            {
                var attribute = element.Attributes[TypeAttributeName];
                if (attribute != null)
                    defaultValue = attribute.Value;
            }
            return defaultValue;
        }

        private static XmlNode GetElement(string elementName, XmlNode sectionXmlNode)
        {
            return sectionXmlNode.SelectSingleNode(elementName);
        }

        private Saturn72Config DefaultConfig()
        {
            return new Saturn72Config
            {
                //We use autofac by default
                ContainerManager =
                    @"Saturn72.Module.Ioc.Autofac.AutofacIocContainerManager, Saturn72.Module.Ioc.Autofac",
                EngineDriver = CommonHelper.GetCompatibleTypeName<AppEngineDriver>(),
                ConfigLoader = CommonHelper.GetCompatibleTypeName<XmlConfigLoader>()
            };
        }

        public static Saturn72Config GetConfiguration()
        {
            return _saturn72Config ?? (_saturn72Config = ConfigurationManager.GetSection("saturn72Config") as Saturn72Config
                                         ?? new Saturn72Config().DefaultConfig());
        }
    }
}