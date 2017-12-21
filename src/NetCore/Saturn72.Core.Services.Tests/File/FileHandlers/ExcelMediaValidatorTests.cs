using System;
using System.IO;
using Shouldly;
using Xunit;
using Saturn72.Core.Services.File;
using Saturn72.Core.Services.File.FileHandlers;

namespace Saturn72.Core.Services.Tests.File.FileHandlers
{
    public class ExcelFileHandlerTests
    {
        public ExcelFileHandlerTests()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        [Fact]
        public void ExcelFileHandler_Validate_Unsupportedtypes()
        {
            new ExcelFileHandler().Validate(null, "ddd").ShouldBe(FileStatusCode.Unsupported);
        }

        [Fact]
        public void ExcelFileHandler_Validate_ReturnsCorruptedFileError()
        {
            var nv = new ExcelFileHandler();
            var cossuptedXlsFile = new byte[] {1, 1, 1, 1, 1};
            nv.Validate(cossuptedXlsFile, "xls").ShouldBe(FileStatusCode.Corrupted);
        }

        [Fact]
        public void ExcelFileHandler_Validate_ReturnsValid()
        {
            var resourcesPath = Path.Combine("Resources");
            var allGoodFiles = Directory.GetFiles(resourcesPath, "good.*");
            var nv = new ExcelFileHandler();
            foreach (var f in allGoodFiles)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                nv.Validate(fs, ext).ShouldBe(FileStatusCode.Valid);
            }
        }

        [Fact]
        public void ExcelFileHandler_Validate_ReturnsEmpty()
        {
            var ExcelFileHandler = new ExcelFileHandler();
            ExcelFileHandler.Validate(new byte[] { }, "xls").ShouldBe(FileStatusCode.EmptyFile);

            var resourcesPath = Path.Combine("Resources");
            var allGoodFiles = Directory.GetFiles(resourcesPath, "empty.*");
            foreach (var f in allGoodFiles)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                ExcelFileHandler.Validate(fs, ext).ShouldBe(FileStatusCode.EmptyFile);
            }
        }

        [Fact]
        public void ExcelFileHandler_Validate_ReturnsCurropted()
        {
            var ExcelFileHandler = new ExcelFileHandler();
            var resourcesPath = Path.Combine("Resources");
            var allGoodFiles = Directory.GetFiles(resourcesPath, "corrupted.*");
            foreach (var f in allGoodFiles)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                ExcelFileHandler.Validate(fs, ext).ShouldBe(FileStatusCode.Corrupted);
            }
        }

        [Fact]
        public void ExcelFileHandler_Validate_ThrowsOnNullBytes()
        {
            var nv = new ExcelFileHandler();
            Should.Throw<NullReferenceException>(()=> nv.Validate(null, "xls"));

        }
    }
}