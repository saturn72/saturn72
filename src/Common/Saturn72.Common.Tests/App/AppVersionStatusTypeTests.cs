using NUnit.Framework;
using Saturn72.Common.App;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Common.Tests.App
{
    public class AppVersionStatusTypeTests
    {
        [Test]
        public void AppVersionStatusType_Validate_items()
        {
            AppVersionStatusType.Alpha.Name.ShouldEqual("Alpha");
            AppVersionStatusType.Alpha.Code.ShouldEqual("4FC974B9-8F54-473E-9540-DA9D171484B3");

            AppVersionStatusType.Beta.Name.ShouldEqual("Beta");
            AppVersionStatusType.Beta.Code.ShouldEqual("5DB58464-8591-414B-B53C-39F5802455CB");

            AppVersionStatusType.ReleaseCandidate.Name.ShouldEqual("ReleaseCandidate");
            AppVersionStatusType.ReleaseCandidate.Code.ShouldEqual("C87BA419-69E1-4EA8-A774-6AFDC5C697D9");

            AppVersionStatusType.Stable.Name.ShouldEqual("Stable");
            AppVersionStatusType.Stable.Code.ShouldEqual("4FA85E17-7A91-41F4-8EF3-597270AF1750");
        }
    }
}