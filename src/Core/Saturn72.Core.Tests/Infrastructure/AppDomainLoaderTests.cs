#region

using System;
using System.Linq;
using NUnit.Framework;
using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Extensions;
using Shouldly;

#endregion

namespace Saturn72.Core.Tests.Infrastructure
{
    public class AppDomainLoaderTests
    {
        [Test]
        public void LoadAppDomain_LoadsExternalAssemblies()
        {
            var asm = FileSystemUtil.RelativePathToAbsolutePath("Modules\\DummyModule\\ShouldBeLoaded.dll");

            var modulesDynamicLoadData = new DynamicLoadingData(
                SystemDefaults.DefaultModulesRoot, SystemDefaults.DefaultModulesShadowCopyDirectory,
                SystemDefaults.DeleteModulesShadowDirectoriesOnStartup, SystemDefaults.DefaultModulesConfigFile);

            var pluginsDynamicLoadData = new DynamicLoadingData(
                SystemDefaults.DefaultPluginsRoot, SystemDefaults.DefaultPluginsShadowCopyDirectory,
                SystemDefaults.DeletePluginsShadowDirectoriesOnStartup, SystemDefaults.DefaultPluginsConfigFile);


            var data = new AppDomainLoadData(new[] {asm}, modulesDynamicLoadData, pluginsDynamicLoadData, null, null);

            AppDomainLoader.Load(data);

            var appDomainAsms = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(t => t.GetName().Name)
                .ToArray();

            appDomainAsms.ShouldContain("ShouldBeLoaded");
        }

        [Test]
        public void LoadAppDomain_LoadsModulesToAppDomain()
        {
            var pluginFiles = new[] {"SingleSuspendedPlugin.json", "2Plugins-BothSuspended.json", "2Plugins-ActiveAndSuspended" };
            var expectedLoadedPlugins = new[] {"DummyPlugin1", "DummyPlugin1,DummyPlugin2", "DummyPlugin1,DummyPlugin2" };

            var moduleRoot = FileSystemUtil.RelativePathToAbsolutePath("Modules");
            var shadowCopyDirectory = FileSystemUtil.RelativePathToAbsolutePath(@"Modules\bin");
            var modulesDynamicLoadData = new DynamicLoadingData(moduleRoot, shadowCopyDirectory, false,
                SystemDefaults.DefaultModulesConfigFile);

            var pluginsRoot = FileSystemUtil.RelativePathToAbsolutePath(SystemDefaults.DefaultPluginsRoot);
            var pluginsShadow =
                FileSystemUtil.RelativePathToAbsolutePath(SystemDefaults.DefaultPluginsShadowCopyDirectory);
            for (var i = 0; i < pluginFiles.Length; i++)
            {
                var pluginConfig = FileSystemUtil.RelativePathToAbsolutePath("InstalledPluginFiles\\" + pluginFiles[i]);
                var pluginsDynamicLoadData = new DynamicLoadingData(pluginsRoot, pluginsShadow, false, pluginConfig);

                var data = new AppDomainLoadData(
                    null,
                    modulesDynamicLoadData,
                    pluginsDynamicLoadData,
                    null,
                    null);

                AppDomainLoader.Load(data);

                var appDomainAsms = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(t => t.GetName().Name)
                    .ToArray();

                foreach (var exp in expectedLoadedPlugins[i].Split(','))
                    appDomainAsms.ShouldContain(exp.Trim());
            }
        }


        [Test]
        public void LoadAppDomain_LoadsPluginsToAppDomain()
        {
            //mark plugin as installed if in list
            var modulesDynamicLoadData = new DynamicLoadingData(
                SystemDefaults.DefaultModulesRoot, SystemDefaults.DefaultModulesShadowCopyDirectory,
                SystemDefaults.DeleteModulesShadowDirectoriesOnStartup, SystemDefaults.DefaultModulesConfigFile);

            var pluginsRoot = FileSystemUtil.RelativePathToAbsolutePath("Plugins");
            var shadowCopyDirectory = FileSystemUtil.RelativePathToAbsolutePath(@"Plugins\bin");

            var pluginsDynamicLoadData = new DynamicLoadingData(pluginsRoot, shadowCopyDirectory, true,
                SystemDefaults.DefaultPluginsConfigFile);

            var data = new AppDomainLoadData(
                null,
                modulesDynamicLoadData,
                pluginsDynamicLoadData,
                null,
                null);

            AppDomainLoader.Load(data);

            var appDomainAsms = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(t => t.GetName().Name)
                .ToArray();

            appDomainAsms.ShouldContain("DummyPlugin1");
        }

        [Test]
        public void LoadAppDomain_LoadsPlugins_ThrowsOnSameSystemName()
        {
            var modulesDynamicLoadData = new DynamicLoadingData(
                SystemDefaults.DefaultModulesRoot, SystemDefaults.DefaultModulesShadowCopyDirectory,
                SystemDefaults.DeleteModulesShadowDirectoriesOnStartup, SystemDefaults.DefaultModulesConfigFile);
            var pluginsDynamicLoadData = new DynamicLoadingData(FileSystemUtil.RelativePathToAbsolutePath("Plugins2"),
                FileSystemUtil.RelativePathToAbsolutePath("Plugins2\\bin"), true,
                SystemDefaults.DefaultPluginsConfigFile);

            var data = new AppDomainLoadData(
                null,
                modulesDynamicLoadData,
                pluginsDynamicLoadData,
                null,
                null);

            Should.Throw<Exception>(() => AppDomainLoader.Load(data));
        }
    }
}