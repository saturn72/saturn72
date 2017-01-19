using System.Collections.Generic;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Infrastructure;

namespace Saturn72.Core.Configuration
{
    public interface IConfigManager
    {
        IDictionary<string, IConfigMap> ConfigMaps { get; }

        AppDomainLoadData AppDomainLoadData { get; }
    }
}