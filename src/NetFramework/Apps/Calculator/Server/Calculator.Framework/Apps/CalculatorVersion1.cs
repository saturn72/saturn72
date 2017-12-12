#region

using Saturn72.Core.Services.App;

#endregion

namespace Calculator.Framework.Apps
{
    public class CalculatorVersion1 : IAppVersion
    {
        public string Key => "v1";

        public int Index => 1;

        public bool IsLatest => true;

        public bool Publish => Status == AppVersionStatusType.Stable || Status == AppVersionStatusType.ReleaseCandidate;

        public AppVersionStatusType Status => AppVersionStatusType.Alpha;
    }
}