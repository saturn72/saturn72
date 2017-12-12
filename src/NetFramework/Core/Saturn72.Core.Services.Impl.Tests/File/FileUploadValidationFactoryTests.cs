using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Services.Impl.File;
using Saturn72.Core.Services.File;
using Shouldly;

namespace Saturn72.Core.Services.Impl.Tests.File
{
    public class FileUploadValidationFactoryTests
    {
        [Test]
        public void FileUploadValidationFactory_IsSupportedExtension_Throws()
        {
            var mv = new FileUploadValidationFactory(null);
            Should.Throw<ArgumentNullException>(()=>mv.IsSupportedExtension("ttt").ShouldBeFalse());
        }

        [Test]
        public void FileUploadValidationFactory_IsSupportedExtension_ReturnsTrueAndFalse()
        {
            var mv = new List<IFileValidator>();

            var mvFactory = new FileUploadValidationFactory(mv);
            //EmptyCollection
            var fileExtension = "ttt";
            mvFactory.IsSupportedExtension(fileExtension).ShouldBeFalse();

            var mvMock = new Mock<IFileValidator>();
            var mvResult = fileExtension+"ert";
            mvMock.Setup(m => m.SupportedExtensions).Returns(()=> new[] {mvResult});
            mv.Add(mvMock.Object);
            //no suitable validator
            mvFactory.IsSupportedExtension(fileExtension).ShouldBeFalse();

            //SuitableVlidator
            mvResult = fileExtension;
            mvFactory.IsSupportedExtension(fileExtension).ShouldBeTrue();
        }

        [Test]
        public void FileUploadValidationFactory_Validate_ReturnsInvalidOnNullObject()
        {
            var mv = new List<IFileValidator>();
            var mvFactory = new FileUploadValidationFactory(mv);
            mvFactory.Validate(null).ShouldBe(FileStatusCode.Invalid);

        }

        [Test]
        public void FileUploadValidationFactory_Validate_HasNoSupportForExtension_ReturnNotSupported()
        {
            const string fileExtension = "ttt";
            var mvMock = new Mock<IFileValidator>();
            var mvExtension = fileExtension + "eee";
            mvMock.Setup(m => m.SupportedExtensions).Returns(() => new[] {mvExtension});

            var mv = new List<IFileValidator> {mvMock.Object};

            var mvFactory = new FileUploadValidationFactory(mv);

            var mediaUploadRequest = new FileUploadRequest
            {
                FileName = "ttt." + fileExtension
            };
            mvFactory.Validate(mediaUploadRequest).ShouldBe(FileStatusCode.Unsupported);

        }
        [Test]
        public void FileUploadValidationFactory_Validate_ReturnsValueFromValidator()
        {
            const string fileExtension = "ttt";
            var mvMock = new Mock<IFileValidator>();
            mvMock.Setup(m => m.SupportedExtensions).Returns(() => new[] { fileExtension });
            var result = FileStatusCode.Blocked;
            mvMock.Setup(m => m.Validate(It.IsAny<byte[]>(), It.IsAny<string>()))
                .Returns(result);
            var mv = new List<IFileValidator> { mvMock.Object };

            var mvFactory = new FileUploadValidationFactory(mv);

            var mediaUploadRequest = new FileUploadRequest
            {
                FileName = "ttt." + fileExtension
            };
            mvFactory.Validate(mediaUploadRequest).ShouldBe(result);
        }
    }
}
