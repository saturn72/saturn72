using System.Linq;
using Saturn72.Core.Services.File;
using Xunit;

namespace Saturn72.Core.Services.Tests.File
{
    public class MediaUploadRequestTests
    {
        [Fact]
        public void FileUploadRequestTests_GetsExtensionWithoutDot()
        {
            var r = new FileUploadRequest
            {
                FileName = "ccc.e"
            };
            r.Extension.SequenceEqual("e");
        }
    }
}
