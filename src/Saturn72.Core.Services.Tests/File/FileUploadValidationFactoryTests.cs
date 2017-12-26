using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Shouldly;
using Xunit;
using Saturn72.Core.Services.File;

namespace Saturn72.Core.Services.Tests.File
{
    public class FileUploadValidationFactoryTests
    {
        [Fact]
        public void FileUploadValidationFactory_IsSupportedExtension_Throws()
        {
            var mv = new FileHandlerFactory(null);
            Should.Throw<ArgumentNullException>(()=>mv.IsSupportedExtension("ttt").ShouldBeFalse());
        }

        [Fact]
        public void FileUploadValidationFactory_IsSupportedExtension_ReturnsTrueAndFalse()
        {
            var mv = new List<IFileHandler>();

            var mvFactory = new FileHandlerFactory(mv);
            //EmptyCollection
            var fileExtension = "ttt";
            mvFactory.IsSupportedExtension(fileExtension).ShouldBeFalse();

            var mvMock = new Mock<IFileHandler>();
            var mvResult = fileExtension+"ert";
            mvMock.Setup(m => m.SupportedExtensions).Returns(()=> new[] {mvResult});
            mv.Add(mvMock.Object);
            //no suitable validator
            mvFactory.IsSupportedExtension(fileExtension).ShouldBeFalse();

            //SuitableVlidator
            mvResult = fileExtension;
            mvFactory.IsSupportedExtension(fileExtension).ShouldBeTrue();
        }

        [Fact]
        public void FileUploadValidationFactory_Validate_ReturnsInvalidOnNullObject()
        {
            var mv = new List<IFileHandler>();
            var mvFactory = new FileHandlerFactory(mv);
            mvFactory.Validate(null, null).ShouldBe(FileStatusCode.Invalid);

        }

        [Fact]
        public void FileUploadValidationFactory_Validate_HasNoSupportForExtension_ReturnNotSupported()
        {
            const string fileExtension = "ttt";
            var mvMock = new Mock<IFileHandler>();
            var mvExtension = fileExtension + "eee";
            mvMock.Setup(m => m.SupportedExtensions).Returns(() => new[] {mvExtension});

            var mv = new List<IFileHandler> {mvMock.Object};

            var mvFactory = new FileHandlerFactory(mv);

            var mediaUploadRequest = new FileUploadRequest
            {
                FileName = "ttt." + fileExtension,
                Bytes = Encoding.UTF8.GetBytes("abcd"),
            };
            mvFactory.Validate(mediaUploadRequest.Extension, mediaUploadRequest.Bytes).ShouldBe(FileStatusCode.Unsupported);

        }
        [Fact]
        public void FileUploadValidationFactory_Validate_ReturnsValueFromValidator()
        {
            const string fileExtension = "ttt";
            var mvMock = new Mock<IFileHandler>();
            mvMock.Setup(m => m.SupportedExtensions).Returns(() => new[] { fileExtension });
            var result = FileStatusCode.Blocked;
            mvMock.Setup(m => m.Validate(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<object>()))
                .Returns(result);
            var mv = new List<IFileHandler> { mvMock.Object };

            var mvFactory = new FileHandlerFactory(mv);

            var mediaUploadRequest = new FileUploadRequest
            {
                FileName = "ttt." + fileExtension,
                Bytes = Encoding.UTF8.GetBytes("abcd"),
            };
            mvFactory.Validate(mediaUploadRequest.Extension, mediaUploadRequest.Bytes).ShouldBe(result);
        }
    }
}
