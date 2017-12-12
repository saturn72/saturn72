#region

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

#endregion

namespace Saturn72.Core.Configuration.Maps
{
    public class DefaultConfigMap : IConfigMap
    {
        public const string AppSettingsKey = "AppSettings";
        public const string ConnectionStringsKey = "ConnectionStrings";

        private IDictionary<string, object> _allConfigurations;


        public NameValueCollection AppSettings => AllConfigRecords[AppSettingsKey] as NameValueCollection;

        public ConnectionStringSettingsCollection ConnectionStrings => AllConfigRecords[ConnectionStringsKey] as ConnectionStringSettingsCollection;

        public IDictionary<string, object> AllConfigRecords => _allConfigurations ?? (_allConfigurations = GetAllConfigurations());


        public object GetValue(string key)
        {
            return AllConfigRecords[key];
        }

        private IDictionary<string, object> GetAllConfigurations()
        {
            return new Dictionary<string, object>
            {
                {AppSettingsKey, ConfigurationManager.AppSettings},
                {ConnectionStringsKey, ConfigurationManager.ConnectionStrings}
            };
        }
    }
}