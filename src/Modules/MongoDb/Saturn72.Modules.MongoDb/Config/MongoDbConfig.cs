using System.Configuration;
using System.Xml;
using Saturn72.Extensions;

namespace Saturn72.Modules.MongoDb.Config
{
    public class MongoDbConfig : IConfigurationSectionHandler
    {
        public string DatabaseName { get; private set; }

        public string ConnectionString { get; private set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var configMap = section.SelectSingleNode("configMap");


            var dbName = configMap.GetInnerElementValue("dbName");
            Guard.NotEmpty(dbName);

            var mongoDbConfig = new MongoDbConfig
            {
                DatabaseName = dbName,
                ConnectionString = configMap.GetInnerElementValue("serverIp")
            };


            return mongoDbConfig;
        }
    }
}