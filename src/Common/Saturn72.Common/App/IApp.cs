#region

using System.Collections.Generic;

#endregion

namespace Saturn72.Common.App
{
    /// <summary>
    /// Represent applicaiton 
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// Application name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Application versions
        /// </summary>
        IEnumerable<IAppVersion> Versions { get; }

        /// <summary>
        /// Gets the latest app version
        /// </summary>
        IAppVersion LatestVersion { get; }
    }
}