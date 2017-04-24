#region

using System;
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
            typeof(NullReferenceException).ShouldBeThrownBy(() => new MediaUploadManager(null, null, null).Upload(null));
        }

        [Test]
        public void MediaUploadManager_ReturnInvalid()
        {
            var uReq = new MediaUploadRequest
            {
                FileName = "ttt.txt"
            };

            var uMgr = new MediaUploadManager(null, null, null);
            var res1 = uMgr.Upload(uReq);

            res1.Status.ShouldEqual(MediaStatusCode.Invalid);
            res1.WasUploaded.ShouldBeFalse();

            uReq.Bytes = () => (byte[]) null;
            var res2 = uMgr.Upload(uReq);

            res2.Status.ShouldEqual(MediaStatusCode.Invalid);
            res2.WasUploaded.ShouldBeFalse();

            uReq.Bytes = () => new byte[] {};
            var res3 = uMgr.Upload(uReq);

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
            var uMgr = new MediaUploadManager(vFactory.Object, null, null);
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
                Bytes = () => new byte[] {1,1,1,1,1 }
        };

            var uMgr = new MediaUploadManager(vFactory.Object, null, null);
            var res = uMgr.Upload(uReq);

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
                Bytes = () => new byte[] { 1, 1, 1, 1, 1 }
            };

            var uMgr = new MediaUploadManager(vFactory.Object, null, null);
            var res = uMgr.Upload(uReq);

            res.Status.ShouldEqual(MediaStatusCode.Corrupted);
            res.WasUploaded.ShouldBeFalse();
        }

        [Test]
        public void MediaUploadManager_UploadFile_UploadsFile()
        {
            var vFactory = new Mock<IMediaUploadValidationFactory>();
            vFactory.Setup(v => v.IsSupportedExtension(It.IsAny<string>())).Returns(() => true);
            vFactory.Setup(v => v.Validate(It.IsAny<MediaUploadRequest>())).Returns(() => MediaStatusCode.Valid);
            var logger = new Mock<ILogger>();
            var ePub = new Mock<IEventPublisher>();

            var uReq = new MediaUploadRequest
            {
                FileName = "ttt.txt",
                Bytes = () => new byte[] {1, 0, 1, 0, 00}
            };

            var uMgr = new MediaUploadManager(vFactory.Object, logger.Object, ePub.Object);
            var res = uMgr.Upload(uReq);

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