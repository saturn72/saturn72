#region

using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using Shouldly;
using Xunit;
using Saturn72.Core.Services.File;
using Saturn72.Core.Domain.FileUpload;
using System.Linq;
using System.Threading.Tasks;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Caching;
using Saturn72.Core.Services.Logging;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Audit;

#endregion

namespace Saturn72.Core.Services.Tests.File
{
    public class FileUploadServiceTests
    {
        [Fact]
        public void FileUploadService_UploadFile_ThrowsOnNullrequest()
        {
            Should.Throw<NullReferenceException>(() =>
            {
                try
                {
                    var t = new FileUploadService(null, null, null, null, null,null, null).UploadAsync(null).Result;
                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }
            });
        }

        [Fact]
        public void FileUploadService_UploadFile_ReturnsEmptyResult()
        {
            var res = new FileUploadService(null, null, null, null, null, null, null).UploadAsync(new List<FileUploadRequest>())
                .Result;
            res.Count().ShouldBe(0);
        }

        [Fact]
        public void FileUploadService_ReturnInvalid()
        {
            var wc = new Mock<IWorkContext>();
            var cId = 123;
            wc.Setup(w => w.CurrentUserId).Returns(cId);
            var ah = new Mock<AuditHelper>(wc.Object);

            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt"
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);

            var uMgr = new FileUploadService(null, null, null, null, sessionRepo.Object, null, ah.Object);
            var res1 = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res1.Status.ShouldBe(FileStatusCode.Invalid);
            res1.WasUploaded.ShouldBeFalse();

            uReq.Bytes = null;
            var res2 = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res2.Status.ShouldBe(FileStatusCode.Invalid);
            res2.WasUploaded.ShouldBeFalse();

            uReq.Bytes = new byte[] { };
            var res3 = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res3.Status.ShouldBe(FileStatusCode.Invalid);
            res3.WasUploaded.ShouldBeFalse();
        }

        [Fact]
        public void FileUploadService_IsSupportedExtension()
        {
            var vFactory = new Mock<IFileHandlerFactory>();
            var vFactoryResult = false;
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => vFactoryResult);

            const string uReq = "txt";
            var uMgr = new FileUploadService(vFactory.Object, null, null, null, null, null, null);
            uMgr.IsSupportedExtension(uReq).ShouldBeFalse();

            vFactoryResult = true;
            uMgr.IsSupportedExtension(uReq).ShouldBeTrue();
        }

        [Fact]
        public void FileUploadService_UploadFile_ReturnsNotSupported()
        {
            var wc = new Mock<IWorkContext>();
            var cId = 123;
            wc.Setup(w => w.CurrentUserId).Returns(cId);
            var ah = new Mock<AuditHelper>(wc.Object);

            var vFactory = new Mock<IFileHandlerFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => false);

            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] {1, 1, 1, 1, 1}
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);
            var uMgr = new FileUploadService(vFactory.Object, null, null, null, sessionRepo.Object, null, ah.Object);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldBe(FileStatusCode.Unsupported);
            res.WasUploaded.ShouldBeFalse();
        }

        [Fact]
        public void FileUploadService_UploadFile_ReturnsCurropted()
        {
            var wc = new Mock<IWorkContext>();
            var cId = 123;
            wc.Setup(w => w.CurrentUserId).Returns(cId);
            var ah = new Mock<AuditHelper>(wc.Object);

            var vFactory = new Mock<IFileHandlerFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(() => FileStatusCode.Corrupted);

            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] {1, 1, 1, 1, 1}
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);

            var uMgr = new FileUploadService(vFactory.Object, null, null, null, sessionRepo.Object, null, ah.Object);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldBe(FileStatusCode.Corrupted);
            res.WasUploaded.ShouldBeFalse();
        }

        [Fact]
        public void FileUploadService_UploadFile_FailedToUploadByMediaRepository()
        {
            var wc = new Mock<IWorkContext>();
            var cId = 123;
            wc.Setup(w => w.CurrentUserId).Returns(cId);
            var ah = new Mock<AuditHelper>(wc.Object);

            var vFactory = new Mock<IFileHandlerFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(() => FileStatusCode.Valid);
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(LogLevel.AllSystemLogLevels.ToArray());

            var ePub = new Mock<IEventPublisher>();
            var mRepo = new Mock<IFileUploadRecordRepository>();
            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] {1, 0, 1, 0, 00}
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);
            var uMgr = new FileUploadService(vFactory.Object, logger.Object, ePub.Object, mRepo.Object, sessionRepo.Object, null, ah.Object);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldBe(FileStatusCode.FailedToUpload);
            res.WasUploaded.ShouldBeFalse();
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Information), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
            ePub.Verify(e => e.Publish(It.IsAny<DomainModelCreatedEvent<FileUploadRecordModel>>()), Times.Never);
        }

        [Fact]
        public void FileUploadService_UploadFile_UploadsFile_ByByteArray()
        {
            var wc = new Mock<IWorkContext>();
            var cId = 123;
            wc.Setup(w => w.CurrentUserId).Returns(cId);
            var ah = new Mock<AuditHelper>(wc.Object);

            var vFactory = new Mock<IFileHandlerFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(() => FileStatusCode.Valid);
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(LogLevel.AllSystemLogLevels.ToArray());
            var ePub = new Mock<IEventPublisher>();
            var mRepo = new Mock<IFileUploadRecordRepository>();
            mRepo.Setup(mr => mr.Create(It.IsAny<FileUploadRecordModel>())).Callback<FileUploadRecordModel>(m =>
            {
                m.Id = 123;
                m.UploadSessionId = 1111;
            });

            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] {1, 0, 1, 0, 00}
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);

            var uMgr = new FileUploadService(vFactory.Object, logger.Object, ePub.Object, mRepo.Object, sessionRepo.Object, null, ah.Object);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldBe(FileStatusCode.Uploaded);
            res.WasUploaded.ShouldBeTrue();

            ah.Verify(a => a.PrepareForCreateAudity(It.IsAny<ICreateAudit>()), Times.Once);
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Information), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
            ePub.Verify(e => e.Publish(It.IsAny<DomainModelCreatedEvent<FileUploadRecordModel>>()), Times.Once);
        }

        [Fact]
        public void FileUploadService_UploadFile_UploadsFile_FromStream()
        {
            var wc = new Mock<IWorkContext>();
            var cId = 123;
            wc.Setup(w => w.CurrentUserId).Returns(cId);
            var ah = new Mock<AuditHelper>(wc.Object);

            var vFactory = new Mock<IFileHandlerFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(() => FileStatusCode.Valid);
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(LogLevel.AllSystemLogLevels.ToArray());
            var ePub = new Mock<IEventPublisher>();
            var mRepo = new Mock<IFileUploadRecordRepository>();
            mRepo.Setup(mr => mr.Create(It.IsAny<FileUploadRecordModel>())).Callback<FileUploadRecordModel>(m =>
            {
                m.Id = 123;
                m.UploadSessionId = 1111;
            });

            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] { 1, 0, 1, 0, 00 }
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);

            var uMgr = new FileUploadService(vFactory.Object, logger.Object, ePub.Object, mRepo.Object, sessionRepo.Object, null, ah.Object);
            var res = uMgr.UploadAsync(new[] { uReq }).Result.First();

            res.Status.ShouldBe(FileStatusCode.Uploaded);
            res.WasUploaded.ShouldBeTrue();
            ah.Verify(a => a.PrepareForCreateAudity(It.IsAny<ICreateAudit>()), Times.Once);
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Information), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
            ePub.Verify(e => e.Publish(It.IsAny<DomainModelCreatedEvent<FileUploadRecordModel>>()), Times.Once);
        }

        #region Get UploadByUploadSessionId

        [Fact]
        public void FileUploadService_ThrowsOnEmptySessionId()
        {
            var srv = new FileUploadService(null, null, null, null, null, null, null);

                Should.Throw<ArgumentOutOfRangeException>(() =>
                {
                    try
                    {
                        var r = srv.GetFileUploadRecordByUploadSessionIdAsync(0).Result;
                    }
                    catch (Exception e)
                    {
                        throw e.InnerException;
                    }
                });
        }

        [Fact]
        public async Task FileUploadService_GetUploadsByUploadSessionId_ReturnsNull()
        {
            var furRepo = new Mock<IFileUploadRecordRepository>();
            IEnumerable<FileUploadRecordModel> furResult = null;
            furRepo.Setup(f => f.GetByUploadSessionId(It.IsAny<long>())).Returns(() => furResult);
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Get<IEnumerable<FileUploadRecordModel>>(It.IsAny<string>()))
                .Returns(null as IEnumerable<FileUploadRecordModel>);
            var srv = new FileUploadService(null, null, null, furRepo.Object, null, cm.Object, null);

            var res = await srv.GetFileUploadRecordByUploadSessionIdAsync(123);
            res.ShouldBeNull();
            cm.Verify(c=>c.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task FileUploadService_GetUploadsByUploadSessionId_ReturnsCollection()
        {
            var furRepo = new Mock<IFileUploadRecordRepository>();
            IEnumerable<FileUploadRecordModel> furResult = new []{new FileUploadRecordModel(), };
            furRepo.Setup(f => f.GetByUploadSessionId(It.IsAny<long>())).Returns(() => furResult);
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Get<IEnumerable<FileUploadRecordModel>>(It.IsAny<string>()))
                .Returns(null as IEnumerable<FileUploadRecordModel>);
            var srv = new FileUploadService(null, null, null, furRepo.Object, null, cm.Object, null);

            var res = await srv.GetFileUploadRecordByUploadSessionIdAsync(123);
            res.Count().ShouldBe(furResult.Count());
            cm.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        #endregion
    }
}