using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.Impl.User;
using Saturn72.Core.Services.User;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Impl.Tests.User
{
    public class UsernameAndEmailRegistrationRequestValidatorTests
    {
        [Test]
        public void ValidateRequest()
        {
            var userSrv = new Mock<IUserService>();
            var userSettings = new Mock<UserSettings>();

            var validator = new UsernameAndEmailRegistrationRequestValidator(userSrv.Object, userSettings.Object);
            //empty username
            var req = new UserRegistrationRequest("", "", null, PasswordFormat.Clear, null);
            var errMsg = validator.ValidateRequest(req);
            errMsg.Count().ShouldEqual(1);
            errMsg.Last().ShouldEqual("Please specify user email or username");

            req = new UserRegistrationRequest(null, "", null, PasswordFormat.Clear, null);
            errMsg = validator.ValidateRequest(req);
            errMsg.Count().ShouldEqual(1);
            errMsg.Last().ShouldEqual("Please specify user email or username");

            //username already exists
            req = new UserRegistrationRequest("dadada", "", null, PasswordFormat.Clear, null);
            userSettings.Setup(u => u.ValidateByEmail).Returns(false);
            userSrv.Setup(u => u.GetUserByUsername(It.IsAny<string>())).Returns(Task.FromResult(new UserModel()));
            errMsg = validator.ValidateRequest(req);
            errMsg.Count().ShouldEqual(1);
            errMsg.Last().ShouldEqual("Username already exists");

            //email already exists
            req = new UserRegistrationRequest("dadada", "", null, PasswordFormat.Clear, null);
            userSettings.Setup(u => u.ValidateByEmail).Returns(true);
            userSrv.Setup(u => u.GetUserByEmail(It.IsAny<string>())).Returns(Task.FromResult(new UserModel()));
            errMsg = validator.ValidateRequest(req);
            errMsg.Count().ShouldEqual(1);
            errMsg.Last().ShouldEqual("Email already exists");
        }
    }
}