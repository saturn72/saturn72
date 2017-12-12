using System;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Impl.User;
using Saturn72.Core.Services.User;
using Saturn72.Core.Services.Validation;
using Shouldly;

namespace Saturn72.Core.Services.Impl.Tests.User
{
    public class UserRegistrationServiceTests
    {
        [Test]
        public void UserRegistrationService_RegisterAsync_ThrowsOnNullRequest()
        {
            var urs = new UserRegistrationService(null, null, null, null, null, null, null, null, null);
            Should.Throw<NullReferenceException>(() =>
            {
                try
                {
                    var t = urs.RegisterAsync(null).Result;
                    ;
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            });
        }

        [Test]
        public void UserRegistrationService_RegisterAsync_InvalidRequestData()
        {
            var urv = new Mock<IUserRegistrationRequestValidator>();
            var erroCode = new SystemErrorCode("code", "msg", "category", "sub-cate");

            urv.Setup(o => o.ValidateRequest(It.IsAny<UserRegistrationRequest>())).Returns(new[] {erroCode});
            var urs = new UserRegistrationService(null, null, null, urv.Object, null, null, null, null, null);
            var req = new UserRegistrationRequest("un", "eml", "pw", PasswordFormat.Clear, "clientIp");

            var res = urs.RegisterAsync(req).Result;
            res.Success.ShouldBeFalse();
            res.Errors.Count.ShouldBe(1);
        }

        [Test]
        public void UserRegistrationService_RegisterAsync_ValidRequestData()
        {
            var uRepo = new Mock<IUserRepository>();
            var urv = new Mock<IUserRegistrationRequestValidator>();
            var ep = new Mock<IEventPublisher>();
            var ual = new Mock<IUserActivityLogService>();
            var ah = new Mock<AuditHelper>(null);
            var us = new Mock<UserSettings>();
            var wc = new Mock<IWorkContext>();
            wc.Setup(s => s.ClientId).Returns("clientId");
            wc.Setup(s => s.CurrentUserIpAddress).Returns("CurrentUserIpAddress");
            var urs = new UserRegistrationService(uRepo.Object, null, us.Object, urv.Object, ep.Object, null,
                ual.Object, ah.Object, wc.Object);
            var req = new UserRegistrationRequest("un", "eml", "pw", PasswordFormat.Clear, "clientIp");

            var res = urs.RegisterAsync(req).Result;
            res.Success.ShouldBeTrue();
            res.Errors.Count.ShouldBe(0);

            ah.Verify(a => a.PrepareForCreateAudity(It.IsAny<UserModel>()), Times.Once);
            uRepo.Verify(a => a.Create(It.IsAny<UserModel>()), Times.Once);
            ual.Verify(a => a.AddUserActivityLogAsync(UserActivityType.UserRegistered, It.IsAny<UserModel>()),
                Times.Once);
            ep.Verify(e => e.Publish(It.IsAny<CreatedEvent<UserModel>>()), Times.Once);
        }
    }
}