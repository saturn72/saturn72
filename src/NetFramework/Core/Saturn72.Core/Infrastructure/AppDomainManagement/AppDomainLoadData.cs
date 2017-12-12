#region

using System;
using System.Collections.Generic;
using Saturn72.Extensions;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Extensibility;

#endregion

namespace Saturn72.Core.Infrastructure.AppDomainManagement
{
    public class AppDomainLoadData
    {
        #region ctor

        public AppDomainLoadData(string[] assemblies, DynamicLoadingData modulesDynamicLoadData,
            DynamicLoadingData pluginsDynamicLoadData, ModuleInstance[] moduleInstances,
            IDictionary<string, Lazy<IConfigMap>> configMaps)
        {
            Assemblies = assemblies;
            ModulesDynamicLoadingData = modulesDynamicLoadData;
            PluginsDynamicLoadingData = pluginsDynamicLoadData;
            ModuleInstances = moduleInstances;
            if (configMaps.NotNull())
                ConfigMaps = configMaps;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the config section maps
        /// </summary>
        public IDictionary<string, Lazy<IConfigMap>> ConfigMaps { get; }

        /// <summary>
        ///     Gets list of specific assemblies to be loaded to app domain
        /// </summary>
        public string[] Assemblies { get; }

        /// <summary>
        ///     Gets the dynamic download data for plugins <see cref="DynamicLoadingData" />
        /// </summary>
        public DynamicLoadingData PluginsDynamicLoadingData { get; }

        /// <summary>
        ///     Gets the dynamic download data for modules <see cref="DynamicLoadingData" />
        /// </summary>
        public DynamicLoadingData ModulesDynamicLoadingData { get; }


        /// <summary>
        ///     Gets the list of modules instances
        /// </summary>
        public ModuleInstance[] ModuleInstances { get; }

        #endregion
    }
}