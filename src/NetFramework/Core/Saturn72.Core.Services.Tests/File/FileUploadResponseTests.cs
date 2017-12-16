using NUnit.Framework;
using Saturn72.Core.Domain.FileUpload;
using Saturn72.Core.Services.File;
using Shouldly;

namespace Saturn72.Core.Services.Tests.File
{
    public class MediaUploadResponseTests
    {
        [Test]
        public void MediaUploadResponseTests_WasUploaded()
        {
            var media = new FileUploadRecordModel {UploadSessionId = 111};
            new FileUploadResponse(null, FileStatusCode.Uploaded, media, "").WasUploaded.ShouldBeTrue();
            new FileUploadResponse(null, FileStatusCode.Unsupported, media, "").WasUploaded.ShouldBeFalse();
            new FileUploadResponse(null, FileStatusCode.Uploaded, null, "").WasUploaded.ShouldBeFalse();
            new FileUploadResponse(null, FileStatusCode.Corrupted, null, "").WasUploaded.ShouldBeFalse();
        }
    }
}