using System;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Services.Impl.Security;
using Shouldly;

namespace Saturn72.Core.Services.Impl.Tests.Security
{
    public class AuthenticationServiceTests
    {
        [Test]
        public void AuthenticationService_AddRefreshTokenAsync_ThrowsOnNullToekn()
        {
            var srv = new AuthenticationService(null);
            Should.Throw<NullReferenceException>(() => srv.AddRefreshTokenAsync(null));
        }

        [Test]
        public void AuthenticationService_AddRefreshTokenAsync_AddsToekn()
        {
            var i = 0;
            var repo = new Mock<IRefreshTokenRepository>();
            repo.Setup(r => r.Create(It.IsAny<RefreshTokenDomainModel>()))
                .Callback(() => i++);

            var srv = new AuthenticationService(repo.Object);
            srv.AddRefreshTokenAsync(new RefreshTokenDomainModel()).Wait();
            i.ShouldBe(1);
        }
    }
}