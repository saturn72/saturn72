#region

using System.Collections.Generic;
using System.Xml.Linq;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Configuration.Maps
{
    public class KeyValueConfigMap : ConfigFileMapBase
    {
        public const string ConfigMapElementName = "configMap";
        public const string ConfigMapRecordElementName = "config";
        public const string KeyAttributeName = "Key";
        public const string ValueAttributeName = "Value";

        public KeyValueConfigMap(string name, string configFilePath) : base(name, configFilePath)
        {
        }

        protected override void LoadFile()
        {
            AllConfigRecords = new Dictionary<string, object>();

            var xDoc = XDocument.Load(ConfigFilePath);
            var configMapElements = xDoc.Descendants(ConfigMapRecordElementName);

            configMapElements.ForEachItem(x =>
            {
                var key = x.GetAttributeValue(KeyAttributeName);
                var value = x.GetAttributeValue(ValueAttributeName);

                AllConfigRecords.Add(key, value);
            });
        }
    }
}