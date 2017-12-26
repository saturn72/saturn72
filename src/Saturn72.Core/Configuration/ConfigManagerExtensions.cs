namespace Saturn72.Core.Configuration
{
    public static class ConfigManagerExtensions
    {
        public static TConfig GetConfig<TConfig>(this IConfigManager configManager) where TConfig : class
        {
            return configManager.GetConfig(typeof(TConfig)) as TConfig;
        }
    }
}