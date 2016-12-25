#region

using NUnit.Framework;
using Saturn72.Core.Services.FileUpload;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Tests.Media
{
    public
        class UploadStatusTests
    {
        [Test]
        public void UploadStatus_StatusCodeAndMessages()
        {
            FileUploadStatus.Uploaded.Code.ShouldEqual(1200);
            FileUploadStatus.Uploaded.Message.ShouldEqual("Uploaded");

            FileUploadStatus.Invalid.Code.ShouldEqual(1400);
            FileUploadStatus.Invalid.Message.ShouldEqual("Invalid");
        }
    }
}