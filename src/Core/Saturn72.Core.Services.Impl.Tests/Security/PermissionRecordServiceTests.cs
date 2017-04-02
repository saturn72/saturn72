using System;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Services.Impl.Security;
using Saturn72.Core.Services.Security;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Impl.Tests.Security
{
    public class PermissionRecordServiceTests
    {
        [Test]
        public void PermissionRecordService_CreatePErmissionRecordIfNotExists_Throws()
        {
            var srv = new PermissionRecordService(null);

            typeof(NullReferenceException).ShouldBeThrownBy(() => srv.CreatePermissionRecordIfNotExists(null));
            typeof(ArgumentException).ShouldBeThrownBy(() => srv.CreatePermissionRecordIfNotExists(new PermissionRecordModel()));
            typeof(ArgumentException).ShouldBeThrownBy(() => srv.CreatePermissionRecordIfNotExists(new PermissionRecordModel { Description = "123" }));
            typeof(ArgumentException).ShouldBeThrownBy(() => srv.CreatePermissionRecordIfNotExists(new PermissionRecordModel {UniqueKey = "123"}));
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
