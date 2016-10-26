#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Services.Extensibility;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Extensibility
{
    public class PluginService : IPluginService
    {
        private readonly IPluginManager _pluginManager;

        public PluginService(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public IEnumerable<PluginDescriptor> GetPluginDescriptors(
            PluginLoadMode loadMode = PluginLoadMode.All, string pluginGroup = null)
        {
            return _pluginManager.GetAll().Where(p => CheckLoadModel(p, loadMode) && CheckGroup(p, pluginGroup));
        }

        public async Task ModifyPluginState(string pluginSystemName, PluginState newState)
        {
            await Task.Run(() =>
            {
                var pluginDescriptor = _pluginManager.GetBySystemName(pluginSystemName);
                Guard.NotNull(pluginDescriptor);

                if (pluginDescriptor.State == newState)
                    return;

                _pluginManager.UpdatePluginDescriptor(pluginDescriptor, newState);
            });
        }

        #region Utilities

        private bool CheckGroup(PluginDescriptor pluginDescriptor, string pluginGroup)
        {
            Guard.NotNull(pluginDescriptor);

            if (!pluginGroup.HasValue())
                return true;

            return pluginGroup.Equals(pluginDescriptor.Group, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool CheckLoadModel(PluginDescriptor pluginDescriptor, PluginLoadMode loadMode)
        {
            Guard.NotNull(pluginDescriptor);

            var isInstalled = pluginDescriptor.State == PluginState.Installed ||
                              pluginDescriptor.State == PluginState.Suspended;
            switch (loadMode)
            {
                case PluginLoadMode.All:
                    return true;
                case PluginLoadMode.InstalledOnly:
                    return isInstalled;
                case PluginLoadMode.NotInstalledOnly:
                    return !isInstalled;
                default:
                    throw new ArgumentOutOfRangeException("Not supported PluginLoadMode");
            }
        }

        #endregion
    }
}