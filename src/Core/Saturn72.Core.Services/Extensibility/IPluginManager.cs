#region

using System.Collections.Generic;
using Saturn72.Core.Extensibility;

#endregion

namespace Saturn72.Core.Services.Extensibility
{
    public interface IPluginManager
    {
        void UpdatePluginDescriptor(PluginDescriptor pluginDescriptor, PluginState newState);
        IEnumerable<PluginDescriptor> GetAll();
        PluginDescriptor GetBySystemName(string systemName);
    }
}