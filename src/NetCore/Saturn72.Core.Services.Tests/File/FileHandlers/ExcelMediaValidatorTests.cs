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

        #region Validate

        [Fact]
        public void ExcelFileHandler_Validate_Unsupportedtypes()
        {
            new ExcelFileHandler().Validate(null, "ddd").ShouldBe(FileStatusCode.Unsupported);
        }

        [Fact]
        public void ExcelFileHandler_Validate_ReturnsCorruptedFileError()
        {
            var nv = new ExcelFileHandler();
            var cossuptedXlsFile = new byte[] { 1, 1, 1, 1, 1 };
            nv.Validate(cossuptedXlsFile, "xls").ShouldBe(FileStatusCode.Invalid);
        }

        [Fact]
        public void ExcelFileHandler_Validate_ReturnsValid()
        {
            var resourcesPath = Path.Combine("Resources");
            var allGoodFiles = Directory.GetFiles(resourcesPath, "good.*");
            allGoodFiles.Length.ShouldBeGreaterThan(0);
            var nv = new ExcelFileHandler();
            foreach (var f in allGoodFiles)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                nv.Validate(fs, ext).ShouldBe(FileStatusCode.Valid);
            }
        }

        [Fact]
        public void ExcelFileHandler_Validate_OnEmpty()
        {
            var excelFileHandler = new ExcelFileHandler();
            excelFileHandler.Validate(new byte[] { }, "xls").ShouldBe(FileStatusCode.EmptyFile);

            var resourcesPath = Path.Combine("Resources");
            var allGoodFiles = Directory.GetFiles(resourcesPath, "empty.*");
            allGoodFiles.Length.ShouldBeGreaterThan(0);
            foreach (var f in allGoodFiles)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                excelFileHandler.Validate(fs, ext).ShouldBe(FileStatusCode.Invalid);
            }
        }
        [Fact]
        public void ExcelFileHandler_Validate_ReturnsInvalid()
        {
            var excelFileHandler = new ExcelFileHandler();
            var resourcesPath = Path.Combine("Resources");
            var files = Directory.GetFiles(resourcesPath, "empty-*.*");
            files.Length.ShouldBeGreaterThan(0);
            foreach (var f in files)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                excelFileHandler.Validate(fs, ext).ShouldBe(FileStatusCode.Invalid);
            }
        }


        [Fact]
        public void ExcelFileHandler_Validate_OnCurropted()
        {
            var ExcelFileHandler = new ExcelFileHandler();
            var resourcesPath = Path.Combine("Resources");
            var allGoodFiles = Directory.GetFiles(resourcesPath, "corrupted.*");
            allGoodFiles.Length.ShouldBeGreaterThan(0);
            foreach (var f in allGoodFiles)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                ExcelFileHandler.Validate(fs, ext).ShouldBe(FileStatusCode.Invalid);
            }
        }

        //TODO: Move To Rhea
        [Fact]
        public void ExcelFileHandler_Validate_OnEmptyFirstRow()
        {
            var excelFileHandler = new ExcelFileHandler();
            var resourcesPath = Path.Combine("Resources");
            var files = Directory.GetFiles(resourcesPath, "first-row-missing.*");
            files.Length.ShouldBeGreaterThan(0);
            foreach (var f in files)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                excelFileHandler.Validate(fs, ext).ShouldBe(FileStatusCode.Invalid);
            }
        }

        [Fact]
        public void ExcelFileHandler_Validate_ThrowsOnNullBytes()
        {
            var nv = new ExcelFileHandler();
            Should.Throw<NullReferenceException>(() => nv.Validate(null, "xls"));

        }

        #endregion

        #region Minify

        [Fact]
        public void ExcelFileHandler_Minify_NoFirstColumn()
        {
            //Empty ==> returns empty byte[]
            var resourcesPath = Path.Combine("Resources");
            var allEmptyFiles = Directory.GetFiles(resourcesPath, "empty.*");
            allEmptyFiles.Length.ShouldBeGreaterThan(0);
            var efh = new ExcelFileHandler();
            foreach (var f in allEmptyFiles)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                efh.Minify(fs, ext).Length.ShouldBe(0);
            }
        }

        [Fact]
        public void ExcelFileHandler_Minify_SpaceInColumnsAndRows()
        {
            var resourcesPath = Path.Combine("Resources");
            var xlsFiles = Directory.GetFiles(resourcesPath, "space-in-*.xls");
            xlsFiles.Length.ShouldBeGreaterThan(0);
            var efh = new ExcelFileHandler();
            foreach (var f in xlsFiles)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                efh.Minify(fs, ext).Length.ShouldBe(fs.Length);
            }

            var xlsxFiles = Directory.GetFiles(resourcesPath, "space-in-*.xlsx");
            xlsxFiles.Length.ShouldBeGreaterThan(0);
            foreach (var f in xlsxFiles)
            {
                var ext = Path.GetExtension(f).Replace(".", string.Empty);
                var fs = System.IO.File.ReadAllBytes(f);
                efh.Minify(fs, ext).Length.ShouldBeLessThan(fs.Length);
            }
        }

        #endregion
    }
}