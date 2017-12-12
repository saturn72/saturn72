using System.Threading.Tasks;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using Saturn72.Common.WebApi.Controllers;
using Saturn72.Common.WebApi.Models.Account;
using Saturn72.Core;
using Saturn72.Core.Services.User;
using Saturn72.Core.Services.Validation;
using Shouldly;

namespace Saturn72.Common.WebApi.Tests.Controllers
{
    public class AccountControllerTests
    {
        [Test]
        public void AccountController_Register_ReturnsBadResponse()
        {
            var userRegSrv = new Mock<IUserRegistrationService>();
            var wc = new Mock<IWorkContext>();

            var ac = new AccountController(userRegSrv.Object, wc.Object);
            var res1 = ac.Register(null).Result as BadRequestErrorMessageResult;
            res1.ShouldNotBeNull();

            wc.Setup(w => w.CurrentUserIpAddress).Returns("ip-address");
            var badRegResponse = new UserRegistrationResponse();
            badRegResponse.Errors.Add(new SystemErrorCode("123", "dadad", "dada", "fff"));
            userRegSrv.Setup(u => u.RegisterAsync(It.IsAny<UserRegistrationRequest>()))
                .Returns(Task.FromResult(badRegResponse));

            var res2 = ac.Register(new UserRegistrationApiModel()).Result as BadRequestErrorMessageResult;
            res2.ShouldNotBeNull();
        }

        [Test]
        public void AccountController_Register_ReturnOKResponse()
        {
            var userRegSrv = new Mock<IUserRegistrationService>();
            var wc = new Mock<IWorkContext>();
            wc.Setup(w => w.CurrentUserIpAddress).Returns("ip-address");
            userRegSrv.Setup(u => u.RegisterAsync(It.IsAny<UserRegistrationRequest>()))
                .Returns(Task.FromResult(new UserRegistrationResponse()));
            var ac = new AccountController(userRegSrv.Object, wc.Object);

            var res = ac.Register(new UserRegistrationApiModel()).Result as OkResult;
            res.ShouldNotBeNull();
        }
    }
}