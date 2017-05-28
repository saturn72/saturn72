using Saturn72.Core.Configuration.Maps;

namespace Saturn72.Modules.MongoDb.Config
{
    public class MongoDbConfigMap : XmlConfigSectionConfigMapBase<MongoDbConfig>
    {
        public MongoDbConfigMap(string name, string configFilePath) : base(name, configFilePath)
        {
        }
    }
}