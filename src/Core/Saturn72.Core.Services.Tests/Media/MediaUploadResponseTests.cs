using System;
using NUnit.Framework;
using Saturn72.Core.Domain.FileUpload;
using Saturn72.Core.Services.Media;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Tests.Media
{
    public class MediaUploadResponseTests
    {
        [Test]
        public void MediaUploadResponseTests_WasUploaded()
        {
            var media = new MediaModel {UploadSessionId = Guid.NewGuid() };
            new MediaUploadResponse(null, MediaStatusCode.Uploaded, media, "").WasUploaded.ShouldBeTrue();
            new MediaUploadResponse(null, MediaStatusCode.Unsupported, media, "").WasUploaded.ShouldBeFalse();
            new MediaUploadResponse(null, MediaStatusCode.Uploaded, null, "").WasUploaded.ShouldBeFalse();
            new MediaUploadResponse(null, MediaStatusCode.Corrupted, null, "").WasUploaded.ShouldBeFalse();
        }
    }
}