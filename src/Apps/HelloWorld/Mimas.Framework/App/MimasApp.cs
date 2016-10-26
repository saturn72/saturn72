#region

using System.Collections.Generic;
using System.Linq;
using Saturn72.Common.App;
using Saturn72.Extensions;

#endregion

namespace Mimas.Framework.App
{
    public class MimasApp : Saturn72AppBase
    {
        private const string AppName = "mimas_server";
        private readonly IAppVersion _latestVersion;
        private readonly IEnumerable<IAppVersion> _versions;

        public MimasApp() : base(AppName)
        {
            _versions = LoadVersions();
            _latestVersion = _versions.First(v => v.IsLatest);
        }

        public override string Name
        {
            get { return AppName; }
        }

        public override IEnumerable<IAppVersion> Versions
        {
            get { return _versions; }
        }

        public override IAppVersion LatestVersion
        {
            get { return _latestVersion; }
        }

        private IEnumerable<IAppVersion> LoadVersions()
        {
            var versions = new IAppVersion[]
            {
                new RheaVersion1(),
            };

            Guard.MustFollow(() => versions.Count(v => v.IsLatest) == 1, "Multiple versions marked as latest, Or noversion is marked as latest");

            return versions;
        }
    }
}