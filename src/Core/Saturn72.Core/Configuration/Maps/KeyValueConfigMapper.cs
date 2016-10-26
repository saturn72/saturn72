#region

using System.Collections.Generic;
using System.Xml.Linq;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Configuration.Maps
{
    public class KeyValueConfigMap : ConfigMapBase
    {
        public const string ConfigMapElementName = "configMap";
        public const string ConfigMapRecordElementName = "config";
        public const string KeyAttributeName = "Key";
        public const string ValueAttributeName = "Value";
        private IDictionary<string, object> _allConfigRecords;
        private bool _alreadyLoaded;

        public KeyValueConfigMap(string name, string configFilePath) : base(name, configFilePath)
        {
        }

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
            if (_alreadyLoaded)
                return;
            _allConfigRecords = new Dictionary<string, object>();

            var xDoc = XDocument.Load(ConfigFilePath);
            var configMapElements = xDoc.Descendants(ConfigMapRecordElementName);

            configMapElements.ForEachItem(x =>
            {
                var key = x.GetAttributeValue(KeyAttributeName);
                var value = x.GetAttributeValue(ValueAttributeName);

                AllConfigRecords.Add(key, value);
            });

            _alreadyLoaded = true;
        }
    }
}