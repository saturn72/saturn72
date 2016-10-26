#region

using System;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Services.Impl.Media;
using Saturn72.Core.Services.Media;
using Saturn72.Core.Services.Security;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.Media
{
    public class UploadServiceTests
    {
        [Test]
        public void UploadService_UploadFile_ThrowsOnNullrequest()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => new UploadService(null).Upload(null));
        }

        [Test]
        public void UploadService_UploadFile_returnsNotSupported()
        {
            var fvManager = new Mock<IFileValidationManager>();
            fvManager.Setup(vs => vs.ValidateFile(It.IsAny<FileValidationRequest>()))
                .Returns<FileValidationRequest>(rq => new FileValidationResult(rq, FileValidationResultCode.NotSupported));

            var srv = new UploadService(fvManager.Object);
            var res = srv.Upload(new UploadRequest(null));
            res.FileValidationResult.ResultCode.ShouldEqual(FileValidationResultCode.NotSupported);
            res.Uploaded.ShouldBeFalse();
        }

        [Test]
        public void UploadService_UploadFile_returnsBlockedContent()
        {
            var fvManager = new Mock<IFileValidationManager>();
            fvManager.Setup(vs => vs.ValidateFile(It.IsAny<FileValidationRequest>()))
                .Returns<FileValidationRequest>(rq => new FileValidationResult(rq, FileValidationResultCode.Blocked));

            var srv = new UploadService(fvManager.Object);
            var res = srv.Upload(new UploadRequest(null));
            res.FileValidationResult.ResultCode.ShouldEqual(FileValidationResultCode.Blocked);
            res.Uploaded.ShouldBeFalse();
        }

        [Test]
        public void UploadService_UploadFile_returnsCorrupted()
        {
            var fvManager = new Mock<IFileValidationManager>();
            fvManager.Setup(vs => vs.ValidateFile(It.IsAny<FileValidationRequest>()))
                .Returns<FileValidationRequest>(rq => new FileValidationResult(rq, FileValidationResultCode.Corrupted));

            var srv = new UploadService(fvManager.Object);
            var res = srv.Upload(new UploadRequest(null));
            res.FileValidationResult.ResultCode.ShouldEqual(FileValidationResultCode.Corrupted);
            res.Uploaded.ShouldBeFalse();
        }

        [Test]
        [Ignore("Wait until upload service is developed")]
        public void UploadService_UploadFile_Uploads()
        {
            var fvManager = new Mock<IFileValidationManager>();
            fvManager.Setup(vs => vs.ValidateFile(It.IsAny<FileValidationRequest>()))
                .Returns<FileValidationRequest>(rq => new FileValidationResult(rq, FileValidationResultCode.Validated));

            var srv = new UploadService(fvManager.Object);
            var res = srv.Upload(new UploadRequest(null));
            res.FileValidationResult.ResultCode.ShouldEqual(FileValidationResultCode.Validated);

            res.Uploaded.ShouldBeTrue();
        }
    }
}