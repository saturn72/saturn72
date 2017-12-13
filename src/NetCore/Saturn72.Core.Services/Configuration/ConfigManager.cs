using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Saturn72.Core.Configuration;

namespace Saturn72.Core.Services.Configuration
{
    public class ConfigManager:IConfigManager
    {
        private static readonly string SettingFileName = Path.Combine("Settings", "settings.json");
        private static readonly IDictionary<Type, object> Settings = new Dictionary<Type, object>();
        public object GetConfig(Type configType)
        {
            if (Settings.ContainsKey(configType))
                return Settings[configType];
            var content = System.IO.File.ReadAllText(SettingFileName);
            var config = JObject.Parse(content).ToObject(configType);
            Settings[configType] = config;
            return config;
        }
    }
}
