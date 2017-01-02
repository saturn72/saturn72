using System;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.Impl.User;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Impl.Tests.User
{
    public class UserActivityLogServiceTests
    {
        [Test]
        public void UserActivityLogService_CreateUserActivityLogAsync_throws()
        {
            var srv = new UserActivityLogService(null);
            //on null activityType
            typeof(NullReferenceException).ShouldBeThrownBy(() => srv.AddUserActivityLogAsync(null, null));
            typeof(NullReferenceException).ShouldBeThrownBy(
                () => srv.AddUserActivityLogAsync(UserActivityType.Login, null));
        }

        [Test]
        public void UserActivityLogService_CreateUserActivityLogAsync_AddUserActivityLog()
        {
            var testStartedOnUtc = DateTime.UtcNow;

            var ualRepo = new Mock<IUserActivityLogRepository>();
            ualRepo.Setup(u => u.AddUserActivityLog(It.IsAny<UserActivityLogDomainModel>()))
                .Callback<UserActivityLogDomainModel>(c=>c.Id = 123)
                .Returns<UserActivityLogDomainModel>(ual => ual);

            var wc = new Mock<IWorkContext<long>>();
            var clientid = "clientId";
            wc.Setup(w => w.ClientId)
                .Returns(clientid);
            var ipaddress = "ipaddress";
            wc.Setup(w => w.CurrentUserIpAddress)
               .Returns(ipaddress);

            var srv = new UserActivityLogService(ualRepo.Object);
            var user = new UserDomainModel
            {
                Id = 100,
                UserGuid = new Guid("EA57D1C7-4575-4961-8A0C-7085E562B4A7")
            };

            var actual = srv.AddUserActivityLogAsync(UserActivityType.Login, user).Result;
            actual.Id.ShouldEqual(123);
            actual.UserId.ShouldEqual(user.Id);
            actual.ActivityDateUtc.ShouldBeSmallerThan(DateTime.UtcNow);
            actual.ActivityDateUtc.ShouldBeGreaterThan(testStartedOnUtc);
            actual.ActivityTypeCode.ShouldEqual(UserActivityType.Login.Code);
            actual.ClientAppId.ShouldEqual(clientid);
            actual.UserIpAddress.ShouldEqual(ipaddress);
        }
    }
}