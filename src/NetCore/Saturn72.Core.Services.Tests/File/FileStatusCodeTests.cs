#region

using Saturn72.Core.Services.File;
using Shouldly;
using Xunit;

#endregion

namespace Saturn72.Core.Services.Tests.File
{
    public class FileStatusCodeTests
    {
        [Fact]
        public void UploadStatus_StatusCodeAndMessages()
        {
            var fsc = FileStatusCode.Blocked;
            fsc.Code.ShouldBe(0);
            fsc.Message.ShouldBe("The file type is blocked by the system");

            fsc = FileStatusCode.Unsupported;
            fsc.Code.ShouldBe(50);
            fsc.Message.ShouldBe("The file type is not supported");

            fsc = FileStatusCode.Corrupted;
            fsc.Code.ShouldBe(100);
            fsc.Message.ShouldBe("The file is corrupted");

            fsc = FileStatusCode.Valid;
            fsc.Code.ShouldBe(1200);
            fsc.Message.ShouldBe("The file was validated");

            fsc = FileStatusCode.Invalid;
            fsc.Code.ShouldBe(200);
            fsc.Message.ShouldBe("The file is invalid");

            fsc = FileStatusCode.Uploaded;
            fsc.Code.ShouldBe(1400);
            fsc.Message.ShouldBe("The file was uploaded");

            fsc = FileStatusCode.FailedToUpload;
            fsc.Code.ShouldBe(1600);
            fsc.Message.ShouldBe("The file faild to upload");

            fsc = FileStatusCode.UnexpectedError;
            fsc.Code.ShouldBe(1800);
            fsc.Message.ShouldBe("Unexpected error occured");
        }
    }
}