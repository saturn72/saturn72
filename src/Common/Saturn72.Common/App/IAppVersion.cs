
namespace Saturn72.Common.App
{
    /// <summary>
    /// Represents application version
    /// </summary>
    public interface IAppVersion
    {
        /// <summary>
        /// Gets the version code
        /// </summary>
        string Key { get;}

        /// <summary>
        /// Version index (relative to other versions)
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets value indicating if the version is the latest
        /// </summary>
        bool IsLatest { get; }

        bool Publish { get; }

        AppVersionStatus Status { get; }
    }
}
