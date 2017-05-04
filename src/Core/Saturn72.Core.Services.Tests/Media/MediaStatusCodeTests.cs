#region

using NUnit.Framework;
using Saturn72.Core.Services.File;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Tests.Media
{
    public
        class MediaStatusCodeTests
    {
        [Test]
        public void UploadStatus_StatusCodeAndMessages()
        {
            var fsc = FileStatusCode.Blocked;
            fsc.Code.ShouldEqual(0);
            fsc.Message.ShouldEqual("The file type is blocked by the system");

            fsc = FileStatusCode.Unsupported;
            fsc.Code.ShouldEqual(50);
            fsc.Message.ShouldEqual("The file type is not supported");

            fsc = FileStatusCode.Corrupted;
            fsc.Code.ShouldEqual(100);
            fsc.Message.ShouldEqual("The file is corrupted");

            fsc = FileStatusCode.Valid;
            fsc.Code.ShouldEqual(1200);
            fsc.Message.ShouldEqual("The file was validated");

            fsc = FileStatusCode.Invalid;
            fsc.Code.ShouldEqual(200);
            fsc.Message.ShouldEqual("The file is invalid");

            fsc = FileStatusCode.Uploaded;
            fsc.Code.ShouldEqual(1400);
            fsc.Message.ShouldEqual("The file was uploaded");

            fsc = FileStatusCode.FailedToUpload;
            fsc.Code.ShouldEqual(1600);
            fsc.Message.ShouldEqual("The file faild to upload");

            fsc = FileStatusCode.UnexpectedError; fsc.Code.ShouldEqual(1800);
            fsc.Message.ShouldEqual("Unexpected error occured");

    }
}
}