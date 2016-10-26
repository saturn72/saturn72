
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Extensibility;

namespace Saturn72.Core.Services.Extensibility
{
    public interface IPluginService
    {
        IEnumerable<PluginDescriptor> GetPluginDescriptors(PluginLoadMode loadMode = PluginLoadMode.All, string pluginGroup = null);
        Task ModifyPluginState(string pluginSystemName, PluginState newState);
    }
}
