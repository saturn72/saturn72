    

#region

using Saturn72.Common.App;

#endregion

namespace Calculator.Framework.Apps
{
    public class CalculatorVersion1 : IAppVersion
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
            get { return Status == AppVersionStatusType.Stable || Status == AppVersionStatusType.ReleaseCandidate; }
        }

        public AppVersionStatusType Status
        {
            get { return AppVersionStatusType.Alpha; }
        }
    }
}