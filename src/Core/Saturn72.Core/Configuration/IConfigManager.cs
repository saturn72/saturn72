#region

using System.Collections.Generic;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Infrastructure.AppDomainManagement;

#endregion

namespace Saturn72.Core.Configuration
{
    public interface IConfigManager
    {
        IDictionary<string, IConfigMap> ConfigMaps { get; }

        AppDomainLoadData AppDomainLoadData { get; }
    }
}