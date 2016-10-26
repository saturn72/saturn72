#region

using System.Collections.Generic;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Infrastructure.AppDomainManagement;

#endregion

namespace Saturn72.Core.Configuration
{
    public abstract class ConfigLoaderBase : IConfigLoader
    {
        public abstract AppDomainLoadData AppDomainLoadData { get; }

        public abstract void Load(IDictionary<string, string> data);

        public IDictionary<string, IConfigMap> ConfigMaps
        {
            get { return AppDomainLoadData.ConfigMaps; }
        }
    }
}