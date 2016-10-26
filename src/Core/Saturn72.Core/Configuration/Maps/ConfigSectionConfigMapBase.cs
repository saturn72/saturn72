#region

using System.Collections.Generic;
using System.Configuration;
using System.Xml;

#endregion

namespace Saturn72.Core.Configuration.Maps
{
    public abstract class ConfigSectionConfigMapBase<TConfigSection> : ConfigMapBase
        where TConfigSection : class, IConfigurationSectionHandler, new()
    {
        private IDictionary<string, object> _allConfigRecords;

        protected ConfigSectionConfigMapBase(string name, string configFilePath) : base(name, configFilePath)
        {
        }

        public TConfigSection Config { get; protected set; }

        public override IDictionary<string, object> AllConfigRecords
        {
            get { return _allConfigRecords; }
        }

        public override object GetValue(string key)
        {
            return AllConfigRecords[key];
        }

        public override void Load()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(ConfigFilePath);

            var config = new TConfigSection().Create(null, null, xmlDoc) as TConfigSection;
            _allConfigRecords = new Dictionary<string, object>
            {
                {Name, config}
            };

            Config = config;
        }
    }
}