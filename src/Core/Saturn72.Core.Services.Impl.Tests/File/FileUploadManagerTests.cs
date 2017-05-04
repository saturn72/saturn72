#region

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.FileUpload;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Impl.File;
using Saturn72.Core.Services.File;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.File
{
    public class FileUploadManagerTests
    {
        [Test]
        public void FileUploadManager_UploadFile_ThrowsOnNullrequest()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() =>
            {
                try
                {
                    var t = new FileUploadManager(null, null, null, null, null).UploadAsync(null).Result;
                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }
            });
        }

        [Test]
        public void FileUploadManager_UploadFile_ReturnsEmptyResult()
        {
            var res = new FileUploadManager(null, null, null, null, null).UploadAsync(new List<FileUploadRequest>()).Result;
            res.Count().ShouldEqual(0);
        }

        [Test]
        public void FileUploadManager_ReturnInvalid()
        {
            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt"
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);

            var uMgr = new FileUploadManager(null, null, null, null, sessionRepo.Object);
            var res1 = uMgr.UploadAsync(new [] {uReq}).Result.First();

            res1.Status.ShouldEqual(FileStatusCode.Invalid);
            res1.WasUploaded.ShouldBeFalse();

            uReq.Bytes = null;
            var res2 = uMgr.UploadAsync(new[] { uReq }).Result.First();

            res2.Status.ShouldEqual(FileStatusCode.Invalid);
            res2.WasUploaded.ShouldBeFalse();

            uReq.Bytes = new byte[] {};
            var res3 = uMgr.UploadAsync(new[] { uReq }).Result.First();

            res3.Status.ShouldEqual(FileStatusCode.Invalid);
            res3.WasUploaded.ShouldBeFalse();
        }


        [Test]
        public void FileUploadManager_IsSupportedExtension()
        {
            var vFactory = new Mock<IFileUploadValidationFactory>();
            var vFactoryResult = false;
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => vFactoryResult);

            const string uReq = "txt";
            var uMgr = new FileUploadManager(vFactory.Object, null, null, null, null);
            uMgr.IsSupportedExtension(uReq).ShouldBeFalse();

            vFactoryResult = true;
            uMgr.IsSupportedExtension(uReq).ShouldBeTrue();
        }


        [Test]
        public void FileUploadManager_UploadFile_ReturnsNotSupported()
        {
            var vFactory = new Mock<IFileUploadValidationFactory>();
            var vFactoryResult = false;
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => false);

            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] {1,1,1,1,1 }
        };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);
            var uMgr = new FileUploadManager(vFactory.Object, null, null, null, sessionRepo.Object);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldEqual(FileStatusCode.Unsupported);
            res.WasUploaded.ShouldBeFalse();
        }

        [Test]
        public void FileUploadManager_UploadFile_ReturnsCurropted()
        {
            var vFactory = new Mock<IFileUploadValidationFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<FileUploadRequest>())).Returns(() => FileStatusCode.Corrupted);

            var uReq = new FileUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] { 1, 1, 1, 1, 1 }
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);

            var uMgr = new FileUploadManager(vFactory.Object, null, null, null, sessionRepo.Object);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldEqual(FileStatusCode.Corrupted);
            res.WasUploaded.ShouldBeFalse();
        }

        [Test]
        public void FileUploadManager_UploadFile_FailedToUploadByMediaRepository()
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
                Bytes = new byte[] { 1, 0, 1, 0, 00 }
            };

            var sessionRepo = new Mock<IFileUploadSessionRepository>();
            sessionRepo.Setup(s => s.Create(It.IsAny<FileUploadSessionModel>()))
                .Callback<FileUploadSessionModel>(n => n.Id = 123);
            var uMgr = new FileUploadManager(vFactory.Object, logger.Object, ePub.Object, mRepo.Object, sessionRepo.Object);
            var res = uMgr.UploadAsync(new[] { uReq }).Result.First();

            res.Status.ShouldEqual(FileStatusCode.FailedToUpload);
            res.WasUploaded.ShouldBeFalse();
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Information), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
            ePub.Verify(e => e.Publish(It.IsAny<CreatedEvent<FileUploadRecordModel>>()), Times.Never);
        }


        [Test]
        public void FileUploadManager_UploadFile_UploadsFile()
        {
            var vFactory = new Mock<IFileUploadValidationFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<FileUploadRequest>())).Returns(() => FileStatusCode.Valid);
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(LogLevel.AllSystemLogLevels.ToArray()); var ePub = new Mock<IEventPublisher>();
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

            var uMgr = new FileUploadManager(vFactory.Object, logger.Object, ePub.Object, mRepo.Object, sessionRepo.Object);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldEqual(FileStatusCode.Uploaded);
            res.WasUploaded.ShouldBeTrue();
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Information), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
            ePub.Verify(e => e.Publish(It.IsAny<CreatedEvent<FileUploadRecordModel>>()), Times.Once);
        }
    }
}