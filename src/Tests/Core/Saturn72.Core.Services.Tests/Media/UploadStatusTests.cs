#region

using NUnit.Framework;
using Saturn72.Core.Services.Media;
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
            UploadStatus.Uploaded.Code.ShouldEqual(1200);
            UploadStatus.Uploaded.Message.ShouldEqual("Uploaded");
        }
    }
}