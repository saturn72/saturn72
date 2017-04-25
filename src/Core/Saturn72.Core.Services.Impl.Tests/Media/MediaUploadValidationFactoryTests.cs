using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Services.Impl.Media;
using Saturn72.Core.Services.Media;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Impl.Tests.Media
{
    public class MediaUploadValidationFactoryTests
    {
        [Test]
        public void MediaUploadValidationFactory_IsSupportedExtension_Throws()
        {
            var mv = new MediaUploadValidationFactory(null);
            typeof(ArgumentNullException).ShouldBeThrownBy(()=>mv.IsSupportedExtension("ttt").ShouldBeFalse());
        }

        [Test]
        public void MediaUploadValidationFactory_IsSupportedExtension_ReturnsTrueAndFalse()
        {
            var mv = new List<IMediaValidator>();

            var mvFactory = new MediaUploadValidationFactory(mv);
            //EmptyCollection
            var fileExtension = "ttt";
            mvFactory.IsSupportedExtension(fileExtension).ShouldBeFalse();

            var mvMock = new Mock<IMediaValidator>();
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
        public void MediaUploadValidationFactory_Validate_ReturnsInvalidOnNullObject()
        {
            var mv = new List<IMediaValidator>();
            var mvFactory = new MediaUploadValidationFactory(mv);
            mvFactory.Validate(null).ShouldEqual(MediaStatusCode.Invalid);

        }

        [Test]
        public void MediaUploadValidationFactory_Validate_HasNoSupportForExtension_ReturnNotSupported()
        {
            const string fileExtension = "ttt";
            var mvMock = new Mock<IMediaValidator>();
            var mvExtension = fileExtension + "eee";
            mvMock.Setup(m => m.SupportedExtensions).Returns(() => new[] {mvExtension});

            var mv = new List<IMediaValidator> {mvMock.Object};

            var mvFactory = new MediaUploadValidationFactory(mv);

            var mediaUploadRequest = new MediaUploadRequest
            {
                FileName = "ttt." + fileExtension
            };
            mvFactory.Validate(mediaUploadRequest).ShouldEqual(MediaStatusCode.NotSupported);

        }
        [Test]
        public void MediaUploadValidationFactory_Validate_ReturnsValueFromValidator()
        {
            const string fileExtension = "ttt";
            var mvMock = new Mock<IMediaValidator>();
            mvMock.Setup(m => m.SupportedExtensions).Returns(() => new[] { fileExtension });
            var result = MediaStatusCode.Blocked;
            mvMock.Setup(m => m.Validate(It.IsAny<byte[]>(), It.IsAny<string>()))
                .Returns(result);
            var mv = new List<IMediaValidator> { mvMock.Object };

            var mvFactory = new MediaUploadValidationFactory(mv);

            var mediaUploadRequest = new MediaUploadRequest
            {
                FileName = "ttt." + fileExtension
            };
            mvFactory.Validate(mediaUploadRequest).ShouldEqual(result);
        }
    }
}
