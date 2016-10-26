#region

using System.Collections.Generic;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Infrastructure.AppDomainManagement;

#endregion

namespace Saturn72.Core.Configuration
{
    public interface IConfigLoader
    {
        /// <summary>
        ///     Gets app domain load data <see cref="AppDomainLoadData" />
        /// </summary>
        AppDomainLoadData AppDomainLoadData { get; }

        IDictionary<string, IConfigMap> ConfigMaps { get; }

        /// <summary>
        ///     Loads configuration data
        /// </summary>
        /// <param name="data">KeyValue collectino contains required data for config loading</param>
        void Load(IDictionary<string, string> data);
    }
}