#region

using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Configuration.Maps
{
    public static class ConfigMapExtensions
    {
        public static object GetValueOrDefault(this IConfigMap configMap, string key)
        {
            return configMap.AllConfigRecords.GetValueOrDefault(key);
        }

        public static string GetValueAsString(this IConfigMap configMap, string key)
        {
            return configMap.GetValue(key).ToString();
        }
    }
}