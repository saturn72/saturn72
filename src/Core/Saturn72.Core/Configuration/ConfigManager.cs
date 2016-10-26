#region

using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Patterns;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Configuration
{
    public class ConfigManager : IConfigManager
    {
        public static IConfigManager Current
        {
            get { return Singleton<IConfigManager>.Instance ?? (Singleton<IConfigManager>.Instance = Initialize()); }
        }

        private static IConfigManager Initialize()
        {
            var saturn72Config = Saturn72Config.GetConfiguration();

            var loader = CommonHelper.CreateInstance<IConfigLoader>(saturn72Config.ConfigLoader);
            if (saturn72Config.ConfigLoaderData.IsNull() || !saturn72Config.ConfigLoaderData.Any())
                throw new Exception(
                    "ConfigLoadData appears to be empty. Please specify IConfigLoader required attributes in App.Config");

            loader.Load(saturn72Config.ConfigLoaderData);

            return new ConfigManager
            {
                ConfigMaps = loader.ConfigMaps,
                AppDomainLoadData = loader.AppDomainLoadData
            };
        }

        public static TConfigMap GetConfigMap<TConfigMap>() where TConfigMap : class, IConfigMap
        {
            return Current.ConfigMaps.Values.FirstOrDefault(cm => (cm as TConfigMap).NotNull()) as TConfigMap;
        }

        public static TConfigMap GetConfigMap<TConfigMap>(string key) where TConfigMap : class, IConfigMap
        {
            return Current.ConfigMaps[key] as TConfigMap;
        }

        #region Properties

        public IDictionary<string, IConfigMap> ConfigMaps { get; private set; }
        public AppDomainLoadData AppDomainLoadData { get; private set; }

        #endregion
    }
}