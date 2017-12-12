#region

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.FileUpload;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.File;
using Saturn72.Core.Services.Impl.File;
using Shouldly;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.File
{
    public class FileUploadServiceTests
    {
        [Test]
        public void FileUploadService_UploadFile_ThrowsOnNullrequest()
        {
            Should.Throw<NullReferenceException>(() =>
            {
                try
                {
                    var t = new FileUploadService(null, null, null, null, null, null).UploadAsync(null).Result;
                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }
            });
        }

        [Test]
        public void FileUploadService_UploadFile_ReturnsEmptyResult()
        {
            var res = new FileUploadService(null, null, null, null, null, null).UploadAsync(new List<FileUploadRequest>())
                .Result;
            res.Count().ShouldBe(0);
        }

        [Test]
        public void FileUploadService_ReturnInvalid()
        {
            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt"
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);

            var uMgr = new FileUploadService(null, null, null, null, sessionRepo.Object, null);
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


        [Test]
        public void FileUploadService_IsSupportedExtension()
        {
            var vFactory = new Mock<IFileUploadValidationFactory>();
            var vFactoryResult = false;
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => vFactoryResult);

            const string uReq = "txt";
            var uMgr = new FileUploadService(vFactory.Object, null, null, null, null, null);
            uMgr.IsSupportedExtension(uReq).ShouldBeFalse();

            vFactoryResult = true;
            uMgr.IsSupportedExtension(uReq).ShouldBeTrue();
        }


        [Test]
        public void FileUploadService_UploadFile_ReturnsNotSupported()
        {
            var vFactory = new Mock<IFileUploadValidationFactory>();
            var vFactoryResult = false;
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => false);

            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] {1, 1, 1, 1, 1}
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);
            var uMgr = new FileUploadService(vFactory.Object, null, null, null, sessionRepo.Object, null);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldBe(FileStatusCode.Unsupported);
            res.WasUploaded.ShouldBeFalse();
        }

        [Test]
        public void FileUploadService_UploadFile_ReturnsCurropted()
        {
            var vFactory = new Mock<IFileUploadValidationFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<FileUploadRequest>())).Returns(() => FileStatusCode.Corrupted);

            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] {1, 1, 1, 1, 1}
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);

            var uMgr = new FileUploadService(vFactory.Object, null, null, null, sessionRepo.Object, null);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldBe(FileStatusCode.Corrupted);
            res.WasUploaded.ShouldBeFalse();
        }

        [Test]
        public void FileUploadService_UploadFile_FailedToUploadByMediaRepository()
        {
            var vFactory = new Mock<IFileUploadValidationFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<FileUploadRequest>())).Returns(() => FileStatusCode.Valid);
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
            var uMgr = new FileUploadService(vFactory.Object, logger.Object, ePub.Object, mRepo.Object,
                sessionRepo.Object, null);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldBe(FileStatusCode.FailedToUpload);
            res.WasUploaded.ShouldBeFalse();
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Information), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
            ePub.Verify(e => e.Publish(It.IsAny<CreatedEvent<FileUploadRecordModel>>()), Times.Never);
        }

        [Test]
        public void FileUploadService_UploadFile_UploadsFile()
        {
            var vFactory = new Mock<IFileUploadValidationFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<FileUploadRequest>())).Returns(() => FileStatusCode.Valid);
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

            var uMgr = new FileUploadService(vFactory.Object, logger.Object, ePub.Object, mRepo.Object,
                sessionRepo.Object, null);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldBe(FileStatusCode.Uploaded);
            res.WasUploaded.ShouldBeTrue();
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Information), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
            ePub.Verify(e => e.Publish(It.IsAny<CreatedEvent<FileUploadRecordModel>>()), Times.Once);
        }

        #region Get UploadByUploadSessionId

        [Test]
        public void FileUploadService_ThrowsOnEmptySessionId()
        {
            var srv = new FileUploadService(null, null, null, null, null, null);

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

        [Test]
        public void FileUploadService_GetUploadsByUploadSessionId()
        {
            var furRepo = new Mock<IFileUploadRecordRepository>();
            IEnumerable<FileUploadRecordModel> furResult = null;
            furRepo.Setup(f => f.GetByUploadSessionId(It.IsAny<long>())).Returns(() => furResult);
            var cm = new Mock<ICacheManager>();

            var srv = new FileUploadService(null, null, null, furRepo.Object, null, cm.Object);
            srv.GetFileUploadRecordByUploadSessionIdAsync(123).Result.ShouldBeNull();
            cm.Verify(c=>c.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        #endregion
    }
}