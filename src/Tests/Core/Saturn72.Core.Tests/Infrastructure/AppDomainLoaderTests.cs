#region

using System;
using System.Linq;
using NUnit.Framework;
using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Extensions;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Tests.Infrastructure
{
    public class AppDomainLoaderTests
    {
        [Test]
        public void LoadAppDomain_LoadsExternalAssemblies()
        {
            var asm = FileSystemUtil.RelativePathToAbsolutePath("Resources\\ShouldBeLoaded.dll");

            var modulesDynamicLoadData = new DynamicLoadingData(
                SystemDefaults.DefaultModulesRoot, SystemDefaults.DefaultModulesShadowCopyDirectory,
                SystemDefaults.DeleteModulesShadowDirectoriesOnStartup);

            var pluginsDynamicLoadData = new DynamicLoadingData(
                SystemDefaults.DefaultPluginsRoot, SystemDefaults.DefaultPluginsShadowCopyDirectory,
                SystemDefaults.DeletePluginsShadowDirectoriesOnStartup);


            var data = new AppDomainLoadData(new[] {asm}, modulesDynamicLoadData, pluginsDynamicLoadData, null, null);

            AppDomainLoader.Load(data);

            var appDomainAsms = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(t => t.GetName().Name).ToArray();

            appDomainAsms.ShouldContainInstance("ShouldBeLoaded");
        }

        [Test]
        [Ignore("Non deterministic test. To get true result it must run in new ApDomain.")]
        [Category("ignored")]
        public void LoadAppDomain_LoadsModulesToAppDomain()
        {
            var moduleRoot = FileSystemUtil.RelativePathToAbsolutePath("Assemblies");
            var shadowCopyDirectory = FileSystemUtil.RelativePathToAbsolutePath(@"Assemblies\bin");

            var modulesDynamicLoadData = new DynamicLoadingData(moduleRoot, shadowCopyDirectory, true);
            var pluginsDynamicLoadData = new DynamicLoadingData(
                SystemDefaults.DefaultPluginsRoot, SystemDefaults.DefaultPluginsShadowCopyDirectory,
                SystemDefaults.DeletePluginsShadowDirectoriesOnStartup);

            var data = new AppDomainLoadData(
                null,
                modulesDynamicLoadData,
                pluginsDynamicLoadData,
                null,
                null);

            AppDomainLoader.Load(data);

            var appDomainAsms = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(t => t.GetName().Name).ToArray();

            appDomainAsms.ShouldContainInstance("ShouldBeLoaded");
        }


        [Test]
        public void LoadAppDomain_LoadsPluginsToAppDomain()
        {
            //mark plugin as installed if in list
            var modulesDynamicLoadData = new DynamicLoadingData(
                SystemDefaults.DefaultModulesRoot, SystemDefaults.DefaultModulesShadowCopyDirectory,
                SystemDefaults.DeleteModulesShadowDirectoriesOnStartup);
            var pluginsDynamicLoadData = new DynamicLoadingData("Plugins", "Plugins\\bin", true);

            var data = new AppDomainLoadData(
                null,
                modulesDynamicLoadData,
                pluginsDynamicLoadData,
                null,
                null);

            AppDomainLoader.Load(data);

            var appDomainAsms = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(t => t.GetName().Name).ToArray();

            appDomainAsms.ShouldContainInstance("ShouldBeLoaded");
        }

        [Test]
        public void LoadAppDomain_LoadsPlugins_ThrowsOnSameSystemName()
        {
            var modulesDynamicLoadData = new DynamicLoadingData(
                SystemDefaults.DefaultModulesRoot, SystemDefaults.DefaultModulesShadowCopyDirectory,
                SystemDefaults.DeleteModulesShadowDirectoriesOnStartup);
            var pluginsDynamicLoadData = new DynamicLoadingData(FileSystemUtil.RelativePathToAbsolutePath("Plugins2"),
                FileSystemUtil.RelativePathToAbsolutePath("Plugins2\\bin"), true);

            var data = new AppDomainLoadData(
                null,
                modulesDynamicLoadData,
                pluginsDynamicLoadData,
                null,
                null);

            typeof(Exception).ShouldBeThrownBy(() => AppDomainLoader.Load(data));
        }
    }
}