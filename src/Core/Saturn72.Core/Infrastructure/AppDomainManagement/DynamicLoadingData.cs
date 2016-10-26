namespace Saturn72.Core.Infrastructure.AppDomainManagement
{
    /// <summary>
    ///     Contains appdomain dynamic loading data
    /// </summary>
    public class DynamicLoadingData
    {
        /// <summary>
        ///     Creates new instance of <see cref="DynamicLoadingData" />
        /// </summary>
        /// <param name="rootDirectory">Root directory as absolute or relative path.</param>
        /// <param name="shadowCopyDirectory">Shadow copy directory as absolute or relative path.</param>
        /// <param name="deleteShadowCopyOnStartup">Specifies if the shadow copy should delete on startup</param>
        public DynamicLoadingData(string rootDirectory, string shadowCopyDirectory, bool deleteShadowCopyOnStartup)
        {
            RootDirectory = rootDirectory;
            ShadowCopyDirectory = shadowCopyDirectory;
            DeleteShadowCopyOnStartup = deleteShadowCopyOnStartup;
        }

        public string RootDirectory { get; private set; }
        public string ShadowCopyDirectory { get; private set; }
        public bool DeleteShadowCopyOnStartup { get; set; }
    }
}