using System.Linq;
using NUnit.Framework;
using Saturn72.Core.Services.File;

namespace Saturn72.Core.Services.Tests.File
{
    public class MediaUploadRequestTests
    {
        [Test]
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
