#region

using System.Collections.Generic;

#endregion

namespace Saturn72.Core.Configuration.Maps
{
    public abstract class ConfigMapBase : IConfigMap
    {
        protected ConfigMapBase(string name, string configFilePath)
        {
            Name = name;
            ConfigFilePath = configFilePath;
        }


        public string Name { get; private set; }

        public string ConfigFilePath { get; private set; }

        public abstract IDictionary<string, object> AllConfigRecords { get; }

        /// <summary>
        ///     Gets config Map Element by key
        /// </summary>
        /// <param name="key">config element key</param>
        /// <returns>object contains the config element data</returns>
        public abstract object GetValue(string key);

        /// <summary>
        ///     Loads config file content
        /// </summary>
        public abstract void Load();
    }
}