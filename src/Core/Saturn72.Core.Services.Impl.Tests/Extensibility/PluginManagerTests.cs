using System;
using System.Linq;
using NUnit.Framework;
using Saturn72.Core.Configuration;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Impl.Extensibility;
using Saturn72.Extensions;
using Shouldly;

namespace Saturn72.Core.Services.Impl.Tests.Extensibility
{
    public class PluginManagerTests
    {
        [Test]
        public void PluginManager_GetByType_Throws()
        {
            var pm = new PluginManager();
            Should.Throw<NullReferenceException>(() => pm.GetByType(null));
        }

        [Test]
        public void PluginManager_GetByType_ReturnsPluginDescirptor()
        {
            //Load all plugins first
            LoadAppDomain();
            
            var type = CommonHelper.GetTypeFromAppDomain("DummyPlugin1.Plugin, DummyPlugin1");
            var pm = new PluginManager();
            var res = pm.GetByType(type);
            res.PluginType.ShouldBeOfType(type.GetType());
            res.State.ShouldBe(PluginState.Active);
        }

        private void LoadAppDomain()
        {
            var moduleRoot = FileSystemUtil.RelativePathToAbsolutePath("Modules");
            var shadowCopyDirectory = FileSystemUtil.RelativePathToAbsolutePath(@"Modules\bin");
            var modulesDynamicLoadData = new DynamicLoadingData(moduleRoot, shadowCopyDirectory, false,
                SystemDefaults.DefaultModulesConfigFile);

            var pluginsRoot = FileSystemUtil.RelativePathToAbsolutePath(SystemDefaults.DefaultPluginsRoot);
            var pluginsShadow =
                FileSystemUtil.RelativePathToAbsolutePath(SystemDefaults.DefaultPluginsShadowCopyDirectory);
            var pluginConfig =
                FileSystemUtil.RelativePathToAbsolutePath("App_Data\\SingleSuspendedPlugin.json");
            var pluginsDynamicLoadData = new DynamicLoadingData(pluginsRoot, pluginsShadow, false, pluginConfig);

            var data = new AppDomainLoadData(
                null,
                modulesDynamicLoadData,
                pluginsDynamicLoadData,
                null,
                null);

            AppDomainLoader.Load(data);

        }
    }
}
