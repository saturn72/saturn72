using System;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Services.Impl.Security;
using Saturn72.Core.Services.Security;
using Shouldly;

namespace Saturn72.Core.Services.Impl.Tests.Security
{
    public class PermissionRecordServiceTests
    {
        [Test]
        public void PermissionRecordService_CreatePErmissionRecordIfNotExists_Throws()
        {
            var srv = new PermissionRecordService(null);

            Should.Throw<NullReferenceException>(() => srv.CreatePermissionRecordIfNotExists(null));
            Should.Throw<ArgumentException>(() => srv.CreatePermissionRecordIfNotExists(new PermissionRecordModel()));
            Should.Throw<ArgumentException>(() => srv.CreatePermissionRecordIfNotExists(new PermissionRecordModel { Description = "123" }));
            Should.Throw<ArgumentException>(() => srv.CreatePermissionRecordIfNotExists(new PermissionRecordModel {UniqueKey = "123"}));
        }

        [Test]
        public void PermissionRecordService_CreatePermissionRecordIfNotExists_AlreadyExists()
        {
            var prr = new Mock<IPermissionRecordRepository>();
            prr.Setup(p => p.PermissionRecordExists(It.IsAny<string>())).Returns(true);
            var srv = new PermissionRecordService(prr.Object);

            srv.CreatePermissionRecordIfNotExists(new PermissionRecordModel {UniqueKey = "123", Description = "134" });
            prr.Verify(p => p.CreatePermissionRecord(It.IsAny<PermissionRecordModel>()), Times.Never);
        }

        [Test]
        public void PermissionRecordService_CreatePermissionRecordIfNotExists_NotExists()
        {
            var prr = new Mock<IPermissionRecordRepository>();
            prr.Setup(p => p.PermissionRecordExists(It.IsAny<string>())).Returns(false);
            var srv = new PermissionRecordService(prr.Object);

            srv.CreatePermissionRecordIfNotExists(new PermissionRecordModel { UniqueKey = "123" , Description = "134"});
            prr.Verify(p => p.CreatePermissionRecord(It.IsAny<PermissionRecordModel>()), Times.Once);
        }

    }
}
