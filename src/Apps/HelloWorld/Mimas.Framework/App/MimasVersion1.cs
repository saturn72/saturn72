#region

using Saturn72.Common.App;

#endregion

namespace Mimas.Framework.App
{
    public class RheaVersion1 : IAppVersion
    {
        public string Key
        {
            get { return "v1"; }
        }

        public int Index
        {
            get { return 1; }
        }

        public bool IsLatest
        {
            get { return true; }
        }

        public bool Publish
        {
            get { return Status == AppVersionStatus.Stable || Status == AppVersionStatus.ReleaseCandidate; }
        }

        public AppVersionStatus Status
        {
            get { return AppVersionStatus.Alpha; }
        }
    }
}