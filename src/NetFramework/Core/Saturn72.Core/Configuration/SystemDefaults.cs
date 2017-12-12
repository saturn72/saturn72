namespace Saturn72.Core.Configuration
{
    public class SystemDefaults
    {
        //Modules data
        public const string DefaultModulesRoot = "Modules";
        public const string DefaultModulesShadowCopyDirectory = "bin";
        public const bool DeleteModulesShadowDirectoriesOnStartup = true;
        public const string DefaultModulesConfigFile = "App_Data\\installedModules.json";

        //Plugins data
        public const string DefaultPluginsRoot = "Plugins";
        public const string DefaultPluginsShadowCopyDirectory = "bin";
        public const bool DeletePluginsShadowDirectoriesOnStartup = true;
        public const string DefaultPluginsConfigFile = "App_Data\\installedPlugins.json";

    }
}