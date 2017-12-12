#region

using System.Collections.Generic;
using System.Configuration;
using System.Xml;

#endregion

namespace Saturn72.Core.Configuration.Maps
{
    public abstract class XmlConfigSectionConfigMapBase<TConfigSection> : ConfigFileMapBase
        where TConfigSection : class, IConfigurationSectionHandler, new()
    {
        protected XmlConfigSectionConfigMapBase(string name, string configFilePath) : base(name, configFilePath)
        {
        }

        public TConfigSection Config { get; protected set; }
        
        protected override void LoadFile()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(ConfigFilePath);

            var config = new TConfigSection().Create(null, null, xmlDoc) as TConfigSection;
            AllConfigRecords = new Dictionary<string, object>
            {
                {Name, config}
            };

            Config = config;
        }
    }
}