#region

using NUnit.Framework;
using Saturn72.Core.Services.Security;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Tests.Security
{
    public class FileValidationResultCodeTests
    {
        [Test]
        public void FileValidationResultCode_ValidateCodesAndMessages()
        {
            FileValidationResultCode.Blocked.Code.ShouldEqual(0);
            FileValidationResultCode.Blocked.Message.ShouldEqual("The file type is blocked by the system");

            FileValidationResultCode.NotSupported.Code.ShouldEqual(50);
            FileValidationResultCode.NotSupported.Message.ShouldEqual("The file type is not supported");

            FileValidationResultCode.Corrupted.Code.ShouldEqual(100);
            FileValidationResultCode.Corrupted.Message.ShouldEqual("The file is corrupted");

            FileValidationResultCode.Validated.Code.ShouldEqual(1200);
            FileValidationResultCode.Validated.Message.ShouldEqual("The file was validated");
        }
    }
}