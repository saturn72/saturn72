using System.Linq;

namespace Saturn72.Core.Services.App
{
    public static class AppExtensions
    {
        public static IAppVersion GetLatestVersion(this IApp app)
        {
            return app.Versions.First(v => v.IsLatest);
        }
    }
}