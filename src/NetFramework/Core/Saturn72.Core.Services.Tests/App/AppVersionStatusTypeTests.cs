using NUnit.Framework;
using Saturn72.Core.Services.App;
using Shouldly;

namespace Saturn72.Core.Services.Tests.App
{
    public class AppVersionStatusTypeTests
    {
        [Test]
        public void AppVersionStatusType_Validate_items()
        {
            AppVersionStatusType.Alpha.Name.ShouldBe("Alpha");
            AppVersionStatusType.Alpha.Code.ShouldBe("4FC974B9-8F54-473E-9540-DA9D171484B3");

            AppVersionStatusType.Beta.Name.ShouldBe("Beta");
            AppVersionStatusType.Beta.Code.ShouldBe("5DB58464-8591-414B-B53C-39F5802455CB");

            AppVersionStatusType.ReleaseCandidate.Name.ShouldBe("ReleaseCandidate");
            AppVersionStatusType.ReleaseCandidate.Code.ShouldBe("C87BA419-69E1-4EA8-A774-6AFDC5C697D9");

            AppVersionStatusType.Stable.Name.ShouldBe("Stable");
            AppVersionStatusType.Stable.Code.ShouldBe("4FA85E17-7A91-41F4-8EF3-597270AF1750");
        }
    }
}