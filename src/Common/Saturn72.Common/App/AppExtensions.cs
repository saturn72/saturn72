using System.Linq;

namespace Saturn72.Common.App
{
    public static class AppExtensions
    {
        public static IAppVersion GetLatestVersion(this IApp app)
        {
            return app.Versions.First(v => v.IsLatest);
        }
    }
}