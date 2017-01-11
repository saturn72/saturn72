#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Saturn72.Extensions;
using Saturn72.UnitTesting.Framework;
using Saturn72.Core.Configuration;

#endregion

namespace Saturn72.Core.Tests.Configuration
{
    public class XmlConfigManagerTests
    {
        [Test]
        public void Load_ThrowsOnMissingConfigFile()
        {
            var nonExistsFile = Path.GetTempPath() + DateTime.Now.ToTimeStamp();

            var xmlConfigManager = new XmlConfigLoader();
            typeof (FileNotFoundException).ShouldBeThrownBy(
                () => xmlConfigManager.Load(GetLoadDataDictionary(nonExistsFile)));
        }

        private IDictionary<string, string> GetLoadDataDictionary(string configPath)
        {
            return new Dictionary<string, string> {{"FilePath", configPath}};
        }

        [Test]
        public void Load_UsesSystemDefaults()
        {
            var xmlConfigManager = new XmlConfigLoader();

            xmlConfigManager.Load(GetLoadDataDictionary(@"Config\ShouldUseDefaultValues.xml"));

            //modules
            var moduleDynamicData = xmlConfigManager.AppDomainLoadData.ModulesDynamicLoadingData;
            moduleDynamicData.DeleteShadowCopyOnStartup.ShouldEqual(
                SystemDefaults.DeleteModulesShadowDirectoriesOnStartup);

            var expectedModuleRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                SystemDefaults.DefaultModulesRoot);
            moduleDynamicData.RootDirectory.ShouldEqual(expectedModuleRootDirectory);

            var expectedModulesShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                SystemDefaults.DefaultModulesRoot, SystemDefaults.DefaultModulesShadowCopyDirectory);
            moduleDynamicData.ShadowCopyDirectory.ShouldEqual(expectedModulesShadowCopyDir);

            //plugins
            var pluginsDynamicData = xmlConfigManager.AppDomainLoadData.PluginsDynamicLoadingData;
            pluginsDynamicData.DeleteShadowCopyOnStartup.ShouldEqual(
                SystemDefaults.DeletePluginsShadowDirectoriesOnStartup);


            var expectedPluginsModuleRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                SystemDefaults.DefaultPluginsRoot);
            pluginsDynamicData.RootDirectory.ShouldEqual(expectedPluginsModuleRootDirectory);

            var expectedPluginsShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                SystemDefaults.DefaultPluginsRoot, SystemDefaults.DefaultPluginsShadowCopyDirectory);
            pluginsDynamicData.ShadowCopyDirectory.ShouldEqual(expectedPluginsShadowCopyDir);
        }

        [Test]
        public void Load_UsesFileValues()
        {
            var xmlConfigManager = new XmlConfigLoader();
            xmlConfigManager.Load(GetLoadDataDictionary(@"Config/ShouldUseConfigFileValues1.xml"));

            //modules
            var moduleDynamicData = xmlConfigManager.AppDomainLoadData.ModulesDynamicLoadingData;
            moduleDynamicData.DeleteShadowCopyOnStartup.ShouldBeFalse();

            var expectedModuleRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "ModulesRootDir");
            moduleDynamicData.RootDirectory.ShouldEqual(expectedModuleRootDirectory);

            var expectedModulesShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "ModulesRootDir\\ModulesShadowCopy");
            moduleDynamicData.ShadowCopyDirectory.ShouldEqual(expectedModulesShadowCopyDir);

            //plugins
            var pluginsDynamicData = xmlConfigManager.AppDomainLoadData.PluginsDynamicLoadingData;
            pluginsDynamicData.DeleteShadowCopyOnStartup.ShouldBeFalse();

            var expectedPluginsModuleRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "PluginsRootDir");
            pluginsDynamicData.RootDirectory.ShouldEqual(expectedPluginsModuleRootDirectory);

            var expectedPluginsShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "PluginsRootDir\\PluginsShadowCopy");
            pluginsDynamicData.ShadowCopyDirectory.ShouldEqual(expectedPluginsShadowCopyDir);
        }

        [Test]
        public void Load_UsesFileValuesUsingDotAsDirectory()
        {
            var xmlConfigManager = new XmlConfigLoader();
            xmlConfigManager.Load(GetLoadDataDictionary(@"Config\ShouldUseConfigFileValues2.xml"));

            //modules
            var moduleDynamicData = xmlConfigManager.AppDomainLoadData.ModulesDynamicLoadingData;
            moduleDynamicData.DeleteShadowCopyOnStartup.ShouldBeFalse();

            var expectedModuleRootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            moduleDynamicData.RootDirectory.ShouldEqual(expectedModuleRootDirectory);

            var expectedModulesShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "ModulesShadowCopy");
            moduleDynamicData.ShadowCopyDirectory.ShouldEqual(expectedModulesShadowCopyDir);

            //plugins
            var pluginsDynamicData = xmlConfigManager.AppDomainLoadData.PluginsDynamicLoadingData;
            pluginsDynamicData.DeleteShadowCopyOnStartup.ShouldBeFalse();

            var expectedPluginsModuleRootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            pluginsDynamicData.RootDirectory.ShouldEqual(expectedPluginsModuleRootDirectory);

            var expectedPluginsShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "PluginsShadowCopy");
            pluginsDynamicData.ShadowCopyDirectory.ShouldEqual(expectedPluginsShadowCopyDir);
        }

        [Test]
        public
        void Load_LoadsAssemblyPaths
            ()
        {
            var xmlConfigManager = new XmlConfigLoader();
            xmlConfigManager.Load(GetLoadDataDictionary(@"Config\ShouldUseConfigFileValues1.xml"));

            var asms = xmlConfigManager.AppDomainLoadData.Assemblies;
            var expected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"Assemblies\ShouldBeLoaded.dll");
            asms.ShouldCount(1);
            asms.ShouldContainInstance(expected);
        }


        [Test]
        public void Load_CreateModuleInstances()
        {
            var xmlConfigManager = new XmlConfigLoader();
            xmlConfigManager.Load(GetLoadDataDictionary(@"Config\ShouldUseConfigFileValues1.xml"));

            var moduleInstances = xmlConfigManager.AppDomainLoadData.ModuleInstances;
            moduleInstances.ShouldCount(3);

            var mi = moduleInstances.ElementAt(0);
            mi.Type.ShouldEqual("MyModuleType1");
            mi.Active.ShouldBeTrue();
            mi.StartupOrder.ShouldEqual(30);
            mi.StopOrder.ShouldEqual(130);

            mi = moduleInstances.ElementAt(1);
            mi.Type.ShouldEqual("MyModuleType2");
            mi.Active.ShouldBeFalse();
            mi.StartupOrder.ShouldEqual(100);
            mi.StopOrder.ShouldEqual(100);

            mi = moduleInstances.ElementAt(2);
            mi.Type.ShouldEqual("MyModuleType3");
            mi.Active.ShouldBeTrue();
            mi.StartupOrder.ShouldEqual(100);
            mi.StopOrder.ShouldEqual(100);
        }

        [Test]
        public
        void Load_LoadsConfigMaps
            ()
        {
            var xmlConfigManager = new XmlConfigLoader();
            xmlConfigManager.Load(GetLoadDataDictionary(@"Config/ShouldUseConfigFileValues1.xml"));


            var configMaps = xmlConfigManager.AppDomainLoadData.ConfigMaps;
            configMaps.ToList().ShouldCount(3);

            var elem = configMaps["ConfigMap1"];
            elem.GetValue("Key1").ShouldEqual("Value1");
            elem.GetValue("Key2").ShouldEqual("Value2");


            elem = configMaps["ConfigMap2"];
            elem.GetValue("Key1").ShouldEqual("Value1");
            elem.GetValue("Key2").ShouldEqual("Value2");
        }
    }
}