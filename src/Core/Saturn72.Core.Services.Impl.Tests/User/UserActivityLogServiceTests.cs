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
                () => srv.AddUserActivityLogAsync(UserActivityType.UserLoggedIn, null));
        }

        [Test]
        public void UserActivityLogService_CreateUserActivityLogAsync_AddUserActivityLog()
        {
            var ualRepo = new Mock<IUserActivityLogRepository>();
            ualRepo.Setup(u => u.AddUserActivityLog(It.IsAny<UserActivityLogModel>()))
                .Callback<UserActivityLogModel>(c => c.Id = 123);
            var wc = new Mock<IWorkContext>();
            var clientid = "clientId";
            var ipaddress = "ipaddress";
            wc.Setup(w => w.CurrentUserIpAddress)
               .Returns(ipaddress);

            var srv = new UserActivityLogService(ualRepo.Object);
            var user = new UserModel
            {
                Id = 100,
                UserGuid = new Guid("EA57D1C7-4575-4961-8A0C-7085E562B4A7"),
                LastClientAppId = clientid,
                LastIpAddress = ipaddress
            };

            srv.AddUserActivityLogAsync(UserActivityType.UserLoggedIn, user).Wait();
            user.Id.ShouldEqual(123);
        }
    }
}