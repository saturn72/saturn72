#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Saturn72.Extensions;
using Shouldly;
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
            Should.Throw<FileNotFoundException>(
                () => xmlConfigManager.Load(GetLoadDataDictionary(nonExistsFile)));
        }

        private IDictionary<string, string> GetLoadDataDictionary(string configPath)
        {
            return new Dictionary<string, string> {{ "ConfigRootPath", configPath}};
        }

        [Test]
        public void Load_UsesSystemDefaults()
        {
            var xmlConfigManager = new XmlConfigLoader();

            xmlConfigManager.Load(GetLoadDataDictionary(@"Config\ShouldUseDefaultValues.xml"));

            //modules
            var moduleDynamicData = xmlConfigManager.AppDomainLoadData.ModulesDynamicLoadingData;
            moduleDynamicData.DeleteShadowCopyOnStartup.ShouldBe(
                SystemDefaults.DeleteModulesShadowDirectoriesOnStartup);

            var expectedModuleRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                SystemDefaults.DefaultModulesRoot);
            moduleDynamicData.RootDirectory.ShouldBe(expectedModuleRootDirectory);

            var expectedModulesShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                SystemDefaults.DefaultModulesRoot, SystemDefaults.DefaultModulesShadowCopyDirectory);
            moduleDynamicData.ShadowCopyDirectory.ShouldBe(expectedModulesShadowCopyDir);

            //plugins
            var pluginsDynamicData = xmlConfigManager.AppDomainLoadData.PluginsDynamicLoadingData;
            pluginsDynamicData.DeleteShadowCopyOnStartup.ShouldBe(
                SystemDefaults.DeletePluginsShadowDirectoriesOnStartup);


            var expectedPluginsModuleRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                SystemDefaults.DefaultPluginsRoot);
            pluginsDynamicData.RootDirectory.ShouldBe(expectedPluginsModuleRootDirectory);

            var expectedPluginsShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                SystemDefaults.DefaultPluginsRoot, SystemDefaults.DefaultPluginsShadowCopyDirectory);
            pluginsDynamicData.ShadowCopyDirectory.ShouldBe(expectedPluginsShadowCopyDir);
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
            moduleDynamicData.RootDirectory.ShouldBe(expectedModuleRootDirectory);

            var expectedModulesShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "ModulesRootDir\\ModulesShadowCopy");
            moduleDynamicData.ShadowCopyDirectory.ShouldBe(expectedModulesShadowCopyDir);

            //plugins
            var pluginsDynamicData = xmlConfigManager.AppDomainLoadData.PluginsDynamicLoadingData;
            pluginsDynamicData.DeleteShadowCopyOnStartup.ShouldBeFalse();

            var expectedPluginsModuleRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "PluginsRootDir");
            pluginsDynamicData.RootDirectory.ShouldBe(expectedPluginsModuleRootDirectory);

            var expectedPluginsShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "PluginsRootDir\\PluginsShadowCopy");
            pluginsDynamicData.ShadowCopyDirectory.ShouldBe(expectedPluginsShadowCopyDir);
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
            moduleDynamicData.RootDirectory.ShouldBe(expectedModuleRootDirectory);

            var expectedModulesShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "ModulesShadowCopy");
            moduleDynamicData.ShadowCopyDirectory.ShouldBe(expectedModulesShadowCopyDir);

            //plugins
            var pluginsDynamicData = xmlConfigManager.AppDomainLoadData.PluginsDynamicLoadingData;
            pluginsDynamicData.DeleteShadowCopyOnStartup.ShouldBeFalse();

            var expectedPluginsModuleRootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            pluginsDynamicData.RootDirectory.ShouldBe(expectedPluginsModuleRootDirectory);

            var expectedPluginsShadowCopyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "PluginsShadowCopy");
            pluginsDynamicData.ShadowCopyDirectory.ShouldBe(expectedPluginsShadowCopyDir);
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
            asms.Length.ShouldBe(1);
            asms.ShouldContain(expected);
        }


        [Test]
        public void Load_CreateModuleInstances()
        {
            var xmlConfigManager = new XmlConfigLoader();
            xmlConfigManager.Load(GetLoadDataDictionary(@"Config\ShouldUseConfigFileValues1.xml"));

            var moduleInstances = xmlConfigManager.AppDomainLoadData.ModuleInstances;
            moduleInstances.Length.ShouldBe(3);

            var mi = moduleInstances.ElementAt(0);
            mi.Type.ShouldBe("MyModuleType1");
            mi.Active.ShouldBeTrue();
            mi.StartupOrder.ShouldBe(30);
            mi.StopOrder.ShouldBe(130);

            mi = moduleInstances.ElementAt(1);
            mi.Type.ShouldBe("MyModuleType2");
            mi.Active.ShouldBeFalse();
            mi.StartupOrder.ShouldBe(100);
            mi.StopOrder.ShouldBe(100);

            mi = moduleInstances.ElementAt(2);
            mi.Type.ShouldBe("MyModuleType3");
            mi.Active.ShouldBeTrue();
            mi.StartupOrder.ShouldBe(100);
            mi.StopOrder.ShouldBe(100);
        }

        [Test]
        public
        void Load_LoadsConfigMaps
            ()
        {
            var xmlConfigManager = new XmlConfigLoader();
            xmlConfigManager.Load(GetLoadDataDictionary(@"Config/ShouldUseConfigFileValues1.xml"));


            var configMaps = xmlConfigManager.AppDomainLoadData.ConfigMaps;
            configMaps.ToList().Count.ShouldBe(3);

            var elem = configMaps["ConfigMap1"];
            elem.Value.AllConfigRecords["Key1"].ShouldBe("Value1");
            elem.Value.AllConfigRecords["Key2"].ShouldBe("Value2");


            elem = configMaps["ConfigMap2"];
            elem.Value.AllConfigRecords["Key1"].ShouldBe("Value1");
            elem.Value.AllConfigRecords["Key2"].ShouldBe("Value2");
        }
    }
}