#region

using System.Collections.Generic;

#endregion

namespace Saturn72.Core.Configuration.Maps
{
    public abstract class ConfigFileMapBase : IConfigMap
    {
        protected ConfigFileMapBase(string name, string configFilePath)
        {
            Name = name;
            ConfigFilePath = configFilePath;
            LoadFile();
        }

        public string Name { get; private set; }

        public string ConfigFilePath { get; private set; }

        public IDictionary<string, object> AllConfigRecords { get; protected set; }

        /// <summary>
        ///     Loads config file content
        /// </summary>
        protected abstract void LoadFile();
    }
}