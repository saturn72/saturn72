using Saturn72.Core.Domain.ActivityLog;
using Saturn72.Core.Domain.Identity;
using Shouldly;
using Xunit;

namespace Saturn72.Core.Tests.Domain.ActivityLog
{
    public class ActivityLogEntityTypeTests
    {
        [Fact]
        public void ActivityLogEntityType_AllEntries()
        {
            var et = ActivityLogEntityType.User;
            et.Code.ShouldBe(10);
            et.EntityType.FullName.ShouldBe(typeof(UserModel).FullName);
        }
    }
}
