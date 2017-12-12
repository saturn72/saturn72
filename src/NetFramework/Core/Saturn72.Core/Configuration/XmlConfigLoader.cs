#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Extensions;
#endregion

namespace Saturn72.Core.Configuration
{
    public class XmlConfigLoader : ConfigLoaderBase
    {
        private const string ConfigRootPathElementName = "ConfigRootPath";
        private AppDomainLoadData _appDomainLoadData;

        /// <summary>
        ///     Loads all config nodes
        /// </summary>
        /// <param name="data">object contains data for config loading</param>
        public override void Load(IDictionary<string, string> data)
        {
            
            var configPath = data[ConfigRootPathElementName];
            Guard.HasValue(configPath);

            configPath = FileSystemUtil.RelativePathToAbsolutePath(configPath);
            Guard.FileExists(configPath);

            var configRoot = XDocument.Load(configPath).FirstNode;
            GetAppDomainLoadData(configRoot);
        }

        #region Utilities

        private void GetAppDomainLoadData(XNode configRoot)
        {
            var assemblies = GetAssembliesPathsFromConfig(configRoot);
            var modulesDynamicLoadData = GetEnvironmentDynamicLoadData(
                configRoot,
                "modules",
                SystemDefaults.DeleteModulesShadowDirectoriesOnStartup,
                SystemDefaults.DefaultModulesRoot,
                SystemDefaults.DefaultModulesShadowCopyDirectory,
                SystemDefaults.DefaultModulesConfigFile);

            var pluginsDynamicLoadData = GetEnvironmentDynamicLoadData(
                configRoot,
                "plugins",
                SystemDefaults.DeletePluginsShadowDirectoriesOnStartup,
                SystemDefaults.DefaultPluginsRoot,
                SystemDefaults.DefaultPluginsShadowCopyDirectory,
                SystemDefaults.DefaultPluginsConfigFile);


            var moduleInstances = GetModuleInstances(configRoot);
            var configMaps = LoadConfigMaps(configRoot);

            _appDomainLoadData = new AppDomainLoadData(assemblies, modulesDynamicLoadData, pluginsDynamicLoadData,
                moduleInstances, configMaps);
        }

        protected virtual IDictionary<string, Lazy<IConfigMap>> LoadConfigMaps(XNode configRoot)
        {
            var configMapsNode = configRoot.XPathSelectElement("configMaps");
            var configMapFile = configMapsNode.NotNull()
                ? configMapsNode.GetAttributeValueOrDefault("Path")
                : "Config/ConfigMaps.xml";
            var absPath = FileSystemUtil.RelativePathToAbsolutePath(configMapFile);

            if (!FileSystemUtil.FileExists(absPath))
                return null;

            var configMapDoc = XDocument.Load(absPath).FirstNode;

            var configMapData = configMapDoc.XPathSelectElements("configMap");

            var configMaps = configMapData.Select(GetConfigMap).ToDictionary(x => x.Key, x => x.Value);

            configMaps.Add("Default", new Lazy<IConfigMap>(() => new DefaultConfigMap()));
            return configMaps;
        }

        private KeyValuePair<string, Lazy<IConfigMap>> GetConfigMap(XElement configMapElement)
        {
            var name = configMapElement.GetAttributeValue("Name");
            var path = FileSystemUtil.RelativePathToAbsolutePath(configMapElement.GetAttributeValue("Path"));
            Guard.FileExists(path);

            var parserType = configMapElement.GetAttributeValue("ParserType");
            var cm = new Lazy<IConfigMap>(()=> CommonHelper.CreateInstance<ConfigFileMapBase>(parserType, name, path));
            return new KeyValuePair<string, Lazy<IConfigMap>>(name, cm);
        }


        private ModuleInstance[] GetModuleInstances(XNode configRoot)
        {
            var elements = configRoot.XPathSelectElements("modules/module");
            var result = new List<ModuleInstance>(elements.Count());
            elements.ForEachItem(m =>
            {
                var type = m.GetAttributeValue("Type");
                var active = m.GetAttributeValue("Active").ToBoolean();
                var startupOrder = GetModuleActionOrder(m, "StartupOrder");
                var stopOrder = GetModuleActionOrder(m, "StopOrder");

                result.Add(new ModuleInstance(type, active, startupOrder, stopOrder));
            });

            return result.ToArray();
        }

        private int GetModuleActionOrder(XElement xElement, string attributeName)
        {
            var value = xElement.GetAttributeValueOrDefault(attributeName);

            var r = CommonHelper.ToInt(value);
            return r == 0 ? 100 : r;
        }

        private string[] GetAssembliesPathsFromConfig(XNode configRoot)
        {
            const string xPath = "assemblies/assembly";
            var assemblyElements = configRoot.XPathSelectElements(xPath);

            return assemblyElements
                .Select(elem => FileSystemUtil.RelativePathToAbsolutePath(elem.GetAttributeValue("Path")))
                .ToArray();
        }

        private DynamicLoadingData GetEnvironmentDynamicLoadData(XNode configRoot, string subXPath,
            bool defaultValueForShaodwDeletion, string defaultValueForBaseDirecotry, 
            string defaultValueForShadowCopyRelativePath, string defaultValueForConfigFile)
        {
            const string envConfigNode = "env";

            //Module data
            var xPath = "{0}/{1}".AsFormat(envConfigNode, subXPath);
            var configData = configRoot.XPathSelectElement(xPath);

            //Should delete shadow copy on startup
            var xAtt = configData.NotNull() ? configData.Attribute("DeleteShadowDirectoryOnStartup") : null;
            var deleteShadowCopyOnStartup = xAtt.IsNull()
                ? defaultValueForShaodwDeletion
                : xAtt.Value.ToBoolean();

            xAtt = configData.NotNull() ? configData.Attribute("Directory") : null;
            var rootDirectory = xAtt.NotNull()
                ? xAtt.Value
                : defaultValueForBaseDirecotry;
            rootDirectory = FileSystemUtil.RelativePathToAbsolutePath(rootDirectory);

            xAtt = configData.NotNull() ? configData.Attribute("ShadowCopyDirectory") : null;
            var subDir = xAtt.NotNull() ? xAtt.Value : defaultValueForShadowCopyRelativePath;
            var shadowCopyDirectory = Path.Combine(rootDirectory, subDir);

            xAtt = configData.NotNull() ? configData.Attribute("ConfigFile") : null;
            var fileName = xAtt.NotNull() ? xAtt.Value : defaultValueForConfigFile;
            var configFile = FileSystemUtil.RelativePathToAbsolutePath(fileName);

            return new DynamicLoadingData(rootDirectory, shadowCopyDirectory, deleteShadowCopyOnStartup, configFile);
        }

        #endregion

        #region Properties

        public override AppDomainLoadData AppDomainLoadData
        {
            get { return _appDomainLoadData; }
        }

        #endregion
    }
}