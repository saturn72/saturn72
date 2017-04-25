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
using Saturn72.Core.Services.Impl.Media;
using Saturn72.Core.Services.Media;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.Media
{
    public class MediaUploadManagerTests
    {
        [Test]
        public void MediaUploadManager_UploadFile_ThrowsOnNullrequest()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() =>
            {
                try
                {
                    var t = new MediaUploadManager(null, null, null, null).UploadAsync(null).Result;
                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }
            });
        }

        [Test]
        public void MediaUploadManager_UploadFile_ReturnsEmptyResult()
        {
            var res = new MediaUploadManager(null, null, null, null).UploadAsync(new List<MediaUploadRequest>()).Result;
            res.Count().ShouldEqual(0);
        }

        [Test]
        public void MediaUploadManager_ReturnInvalid()
        {
            var uReq = new MediaUploadRequest
            {
                FileName = "ttt.txt"
            };

            var uMgr = new MediaUploadManager(null, null, null, null);
            var res1 = uMgr.UploadAsync(new [] {uReq}).Result.First();

            res1.Status.ShouldEqual(MediaStatusCode.Invalid);
            res1.WasUploaded.ShouldBeFalse();

            uReq.Bytes = null;
            var res2 = uMgr.UploadAsync(new[] { uReq }).Result.First();

            res2.Status.ShouldEqual(MediaStatusCode.Invalid);
            res2.WasUploaded.ShouldBeFalse();

            uReq.Bytes = new byte[] {};
            var res3 = uMgr.UploadAsync(new[] { uReq }).Result.First();

            res3.Status.ShouldEqual(MediaStatusCode.Invalid);
            res3.WasUploaded.ShouldBeFalse();
        }


        [Test]
        public void MediaUploadManager_IsSupportedExtension()
        {
            var vFactory = new Mock<IMediaUploadValidationFactory>();
            var vFactoryResult = false;
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => vFactoryResult);

            const string uReq = "txt";
            var uMgr = new MediaUploadManager(vFactory.Object, null, null, null);
            uMgr.IsSupportedExtension(uReq).ShouldBeFalse();

            vFactoryResult = true;
            uMgr.IsSupportedExtension(uReq).ShouldBeTrue();
        }


        [Test]
        public void MediaUploadManager_UploadFile_ReturnsNotSupported()
        {
            var vFactory = new Mock<IMediaUploadValidationFactory>();
            var vFactoryResult = false;
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => false);

            var uReq = new MediaUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] {1,1,1,1,1 }
        };

            var uMgr = new MediaUploadManager(vFactory.Object, null, null, null);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldEqual(MediaStatusCode.NotSupported);
            res.WasUploaded.ShouldBeFalse();
        }

        [Test]
        public void MediaUploadManager_UploadFile_ReturnsCurropted()
        {
            var vFactory = new Mock<IMediaUploadValidationFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<MediaUploadRequest>())).Returns(() => MediaStatusCode.Corrupted);

            var uReq = new MediaUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] { 1, 1, 1, 1, 1 }
            };

            var uMgr = new MediaUploadManager(vFactory.Object, null, null, null);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldEqual(MediaStatusCode.Corrupted);
            res.WasUploaded.ShouldBeFalse();
        }

        [Test]
        public void MediaUploadManager_UploadFile_FailedToUploadByMediaRepository()
        {
            var vFactory = new Mock<IMediaUploadValidationFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<MediaUploadRequest>())).Returns(() => MediaStatusCode.Valid);
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(LogLevel.AllSystemLogLevels.ToArray());

            var ePub = new Mock<IEventPublisher>();
            var mRepo = new Mock<IMediaRepository>();
            var uReq = new MediaUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] { 1, 0, 1, 0, 00 }
            };

            var uMgr = new MediaUploadManager(vFactory.Object, logger.Object, ePub.Object, mRepo.Object);
            var res = uMgr.UploadAsync(new[] { uReq }).Result.First();

            res.Status.ShouldEqual(MediaStatusCode.FailedToUpload);
            res.WasUploaded.ShouldBeFalse();
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Information), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
            ePub.Verify(e => e.Publish(It.IsAny<CreatedEvent<MediaModel>>()), Times.Never);
        }


        [Test]
        public void MediaUploadManager_UploadFile_UploadsFile()
        {
            var vFactory = new Mock<IMediaUploadValidationFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<MediaUploadRequest>())).Returns(() => MediaStatusCode.Valid);
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(LogLevel.AllSystemLogLevels.ToArray()); var ePub = new Mock<IEventPublisher>();
            var mRepo = new Mock<IMediaRepository>();
            mRepo.Setup(mr => mr.Create(It.IsAny<MediaModel>())).Callback<MediaModel>(m =>
            {
                m.Id = 123;
                m.UploadSessionId = Guid.NewGuid();
            });

            var uReq = new MediaUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = new byte[] {1, 0, 1, 0, 00}
            };

            var uMgr = new MediaUploadManager(vFactory.Object, logger.Object, ePub.Object, mRepo.Object);
            var res = uMgr.UploadAsync(new[] {uReq}).Result.First();

            res.Status.ShouldEqual(MediaStatusCode.Uploaded);
            res.WasUploaded.ShouldBeTrue();
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Information), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
            ePub.Verify(e => e.Publish(It.IsAny<CreatedEvent<MediaModel>>()), Times.Once);
        }
    }
}