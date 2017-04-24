using System.Linq;
using NUnit.Framework;
using Saturn72.Core.Services.Media;

namespace Saturn72.Core.Services.Tests.Media
{
    public class MediaUploadRequestTests
    {
        [Test]
        public void FileUploadRequestTests_GetsExtensionWithoutDot()
        {
            var r = new MediaUploadRequest
            {
                FileName = "ccc.e"
            };
            r.Extension.SequenceEqual("e");
        }
    }
}
