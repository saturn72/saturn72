
using System;
using System.Threading;
using Moq;
using Xunit;
using Saturn72.Core.Audit;
using Shouldly;
using Saturn72.Core.Services;

namespace Saturn72.Core.Services.Tests
{
    public class AuditHelperTests
    {
        #region create
        [Fact]
        public void AuditHelper_PrepareForCreate_DoesNotThrwos_OnNull()
        {
            new AuditHelper(null).PrepareForCreateAudity(null);
        }

        [Fact]
        public void AuditHelper_PrepareForCreate_Thrwos()
        {
            var aHelper = new AuditHelper(null);
            var audit = new DummyFullAudit
            {
                CreatedOnUtc = DateTime.Now.AddDays(10)
            };
            Should.Throw<InvalidOperationException>(() => aHelper.PrepareForCreateAudity(audit));

            audit = new DummyFullAudit
            {
                CreatedByUserId = 100
            };
            Should.Throw<InvalidOperationException>(() => aHelper.PrepareForCreateAudity(audit));
        }

        [Fact]
        public void AuditHelper_PrepareForCreate_AddAudity()
        {
            var wc = new Mock<IWorkContext>();
            var userId = 100;
            wc.Setup(w => w.CurrentUserId)
                .Returns(userId);
            var aHelper = new AuditHelper(wc.Object);
            var audit = new DummyFullAudit();
            aHelper.PrepareForCreateAudity(audit)
                ;
            audit.CreatedByUserId.ShouldBe(userId);
            audit.CreatedOnUtc.ShouldNotBe(default(DateTime));
            audit.CreatedOnUtc.ShouldBeLessThanOrEqualTo(DateTime.Now);
        }

        [Fact]
        public void AuditHelper_PrepareForCreate_AddAudity_BrowseData()
        {
            const int userId = 100;
            const string clientId = "123";
            const string ipAddress = "ipAddress";

            var wc = new Mock<IWorkContext>();

            wc.Setup(w => w.CurrentUserId).Returns(userId);
            wc.Setup(w => w.ClientId).Returns(clientId);
            wc.Setup(w => w.CurrentUserIpAddress).Returns(ipAddress);

            var aHelper = new AuditHelper(wc.Object);
            var audit = new DummyFull_Accesable_Audit();
            aHelper.PrepareForCreateAudity(audit)
                ;
            audit.CreatedByUserId.ShouldBe(userId);
            audit.CreatedOnUtc.ShouldNotBe(default(DateTime));
            audit.CreatedOnUtc.ShouldBeLessThanOrEqualTo(DateTime.Now);
            audit.CreatedOnUtc.ShouldBeLessThanOrEqualTo(DateTime.Now);

            audit.LastAccessedOnUtc.ShouldBeLessThanOrEqualTo(DateTime.Now);
            audit.LastAccessedAppId.ShouldBeLessThanOrEqualTo(clientId);
            audit.LastAccessedIpAddress.ShouldBeLessThanOrEqualTo(ipAddress);
        }
        #endregion Create
        #region Create
        [Fact]
        public void AuditHelper_PrepareForUpdate_DoesNotThrwos_OnNull()
        {
            new AuditHelper(null).PrepareForUpdateAudity(null);
        }

        [Fact]
        public void AuditHelper_PrepareForUpdate_AddAudity_WithoutCreate()
        {
            var wc = new Mock<IWorkContext>();
            var userId = 100;
            wc.Setup(w => w.CurrentUserId)
                .Returns(userId)
                .Callback(() => Thread.Sleep(50));

            var createdOn = DateTime.Now.AddDays(-10);
            var createdByUserId = 110;
            var aHelper = new AuditHelper(wc.Object);
            var audit = new DummyFullAudit
            {
                CreatedOnUtc = createdOn,
                CreatedByUserId = createdByUserId
            };
            aHelper.PrepareForUpdateAudity(audit);
            audit.CreatedByUserId.ShouldBe(createdByUserId);
            audit.CreatedOnUtc.ShouldBe(createdOn);

            audit.UpdatedByUserId.ShouldBe(userId);
            audit.UpdatedOnUtc.HasValue.ShouldBeTrue();
            audit.UpdatedOnUtc.Value.ShouldBeGreaterThan(audit.CreatedOnUtc);
            audit.UpdatedOnUtc.HasValue.ShouldBeTrue();
            audit.UpdatedOnUtc.Value.ShouldBeLessThanOrEqualTo(DateTime.Now);

        }
        [Fact]
        public void AuditHelper_PrepareForUpdate_AddAudity_WithCreate()
        {
            var wc = new Mock<IWorkContext>();
            var userId = 100;
            wc.Setup(w => w.CurrentUserId)
                .Returns(userId);

            var aHelper = new AuditHelper(wc.Object);
            var audit = new DummyFullAudit();
            aHelper.PrepareForUpdateAudity(audit);
            audit.CreatedByUserId.ShouldBe(userId);
            audit.CreatedOnUtc.ShouldNotBe(default(DateTime));
            Thread.Sleep(5);
            audit.CreatedOnUtc.ShouldBeLessThan(DateTime.Now);

            audit.UpdatedByUserId.ShouldBe(userId);
            audit.UpdatedOnUtc.HasValue.ShouldBeTrue();
            audit.UpdatedOnUtc.Value.ShouldBeGreaterThanOrEqualTo(audit.CreatedOnUtc);
            audit.UpdatedOnUtc.HasValue.ShouldBeTrue();
            audit.UpdatedOnUtc.Value.ShouldBeLessThanOrEqualTo(DateTime.Now);

        }

        [Fact]
        public void AuditHelper_PrepareForUpdate_Browsable_UpdateAudity_WithCreate()
        {
            const int userId = 100;
            const string clientId = "123";
            const string ipAddress = "ipAddress";

            var wc = new Mock<IWorkContext>();

            wc.Setup(w => w.CurrentUserId).Returns(userId);
            wc.Setup(w => w.ClientId).Returns(clientId);
            wc.Setup(w => w.CurrentUserIpAddress).Returns(ipAddress);


            var aHelper = new AuditHelper(wc.Object);
            var audit = new DummyFull_Accesable_Audit();
            aHelper.PrepareForUpdateAudity(audit);
            audit.CreatedByUserId.ShouldBe(userId);
            audit.CreatedOnUtc.ShouldNotBe(default(DateTime));
            Thread.Sleep(5);
            audit.CreatedOnUtc.ShouldBeLessThan(DateTime.Now);

            audit.UpdatedByUserId.ShouldBe(userId);
            (audit.UpdatedOnUtc >= audit.CreatedOnUtc).ShouldBeTrue();
            (audit.UpdatedOnUtc <= DateTime.Now).ShouldBeTrue();

            audit.LastAccessedOnUtc.ShouldBeLessThanOrEqualTo(DateTime.Now);
            audit.LastAccessedAppId.ShouldBe(clientId);
            audit.LastAccessedIpAddress.ShouldBe(ipAddress);
        }


        #endregion

        #region Delete
        [Fact]
        public void AuditHelper_PrepareForelete_DoesNotThrwos_OnNull()
        {
            new AuditHelper(null).PrepareForDeleteAudity(null);
        }
        [Fact]
        public void AuditHelper_PrepareForDelete_Thrwos()
        {
            var aHelper = new AuditHelper(null);
            var audit = new DummyFullAudit
            {
                DeletedOnUtc = DateTime.Now.AddDays(100)
            };
            Should.Throw<InvalidOperationException>(() => aHelper.PrepareForDeleteAudity(audit));

            audit = new DummyFullAudit
            {
                DeletedByUserId = 100
            };
            Should.Throw<InvalidOperationException>(() => aHelper.PrepareForDeleteAudity(audit));

            audit = new DummyFullAudit
            {
                DeletedOnUtc = DateTime.UtcNow.AddDays(-10)
            };
            Should.Throw<InvalidOperationException>(() => aHelper.PrepareForDeleteAudity(audit));
        }

        [Fact]
        public void AuditHelper_PrepareForDelete_AddAudity_DeletedByNotAssigned()
        {
            var wc = new Mock<IWorkContext>();
            var userId = 100;
            wc.Setup(w => w.CurrentUserId)
                .Returns(userId)
                .Callback(() => Thread.Sleep(50));

            var aHelper = new AuditHelper(wc.Object);
            var audit = new DummyFullAudit();
            aHelper.PrepareForDeleteAudity(audit);

            audit.DeletedByUserId.ShouldBe(userId);
            audit.DeletedOnUtc.ShouldNotBeNull();
            audit.DeletedOnUtc.Value.ShouldBeLessThan(DateTime.Now);
        }

        [Fact]
        public void AuditHelper_PrepareForDelete_AddAudity_DeletedByAssignedToIllegalUser()
        {
            var wc = new Mock<IWorkContext>();
            var userId = 100;
            wc.Setup(w => w.CurrentUserId)
                .Returns(userId)
                .Callback(() => Thread.Sleep(50));

            var aHelper = new AuditHelper(wc.Object);
            var audit1 = new DummyFullAudit();
            audit1.DeletedByUserId = 0;
            aHelper.PrepareForDeleteAudity(audit1);

            audit1.DeletedByUserId.ShouldBe(userId);
            audit1.DeletedOnUtc.ShouldNotBeNull();
            audit1.DeletedOnUtc.Value.ShouldBeLessThan(DateTime.Now);

            var audit2 = new DummyFullAudit();
            audit2.DeletedByUserId = -123;
            aHelper.PrepareForDeleteAudity(audit2);

            audit2.DeletedByUserId.ShouldBe(userId);
            audit2.DeletedOnUtc.ShouldNotBeNull();
            audit2.DeletedOnUtc.Value.ShouldBeLessThan(DateTime.Now);
        }
        [Fact]
        public void AuditHelper_PrepareForDelete_Accessable_AddAudity_DeletedByAssignedToIllegalUser()
        {
            const int userId = 100;
            const string clientId = "123";
            const string ipAddress = "ipAddress";

            var wc = new Mock<IWorkContext>();

            wc.Setup(w => w.ClientId).Returns(clientId);
            wc.Setup(w => w.CurrentUserIpAddress).Returns(ipAddress);

            wc.Setup(w => w.CurrentUserId)
                .Returns(userId)
                .Callback(() => Thread.Sleep(50));

            var aHelper = new AuditHelper(wc.Object);
            var audit1 = new DummyFull_Accesable_Audit();
            audit1.DeletedByUserId = 0;
            aHelper.PrepareForDeleteAudity(audit1);

            audit1.DeletedByUserId.ShouldBe(userId);
            audit1.DeletedOnUtc.ShouldNotBeNull();
            audit1.DeletedOnUtc.Value.ShouldBeLessThan(DateTime.Now);
            audit1.LastAccessedOnUtc.ShouldBeLessThanOrEqualTo(DateTime.Now);
            audit1.LastAccessedAppId.ShouldBe(clientId);
            audit1.LastAccessedIpAddress.ShouldBe(ipAddress);

            var audit2 = new DummyFull_Accesable_Audit();
            audit2.DeletedByUserId = -123;
            aHelper.PrepareForDeleteAudity(audit2);

            audit2.DeletedByUserId.ShouldBe(userId);
            audit2.DeletedOnUtc.ShouldNotBeNull();
            audit2.DeletedOnUtc.Value.ShouldBeLessThan(DateTime.Now);

            audit2.LastAccessedOnUtc.ShouldBeLessThanOrEqualTo(DateTime.Now);
            audit2.LastAccessedAppId.ShouldBe(clientId);
            audit2.LastAccessedIpAddress.ShouldBe(ipAddress);
        }
        #endregion

        internal class DummyFullAudit : IFullAudit
        {
            public DateTime CreatedOnUtc { get; set; }
            public long CreatedByUserId { get; set; }
            public DateTime? UpdatedOnUtc { get; set; }
            public long? UpdatedByUserId { get; set; }
            public DateTime? DeletedOnUtc { get; set; }
            public long? DeletedByUserId { get; set; }
        }

        internal class DummyFull_Accesable_Audit : IFullAudit, IAccessAudit
        {
            public DateTime CreatedOnUtc { get; set; }
            public long CreatedByUserId { get; set; }
            public DateTime? UpdatedOnUtc { get; set; }
            public long? UpdatedByUserId { get; set; }
            public DateTime? DeletedOnUtc { get; set; }
            public long? DeletedByUserId { get; set; }
            public DateTime LastAccessedOnUtc { get; set; }
            public long LastAccessedByUserId { get; set; }
            public string LastAccessedIpAddress { get; set; }
            public string LastAccessedAppId { get; set; }
        }


    }
}
