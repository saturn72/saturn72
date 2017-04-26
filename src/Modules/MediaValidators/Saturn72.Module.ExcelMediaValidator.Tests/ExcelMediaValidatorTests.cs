using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Saturn72.Core.Services.Media;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Module.ExcelMediaValidator.Tests
{
    public class ExcelMediaValidatorTests
    {
        [Test]
        public void ExcelMediaValidator_Validate_Unsupportedtypes()
        {
            new ExcelMediaValidator().Validate(null, "ddd").ShouldEqual(MediaStatusCode.Unsupported);
        }
        [Test]
        public void ExcelMediaValidator_Validate_ReturnsCorruptedFileError()
        {
            var nv = new ExcelMediaValidator();
            var cossuptedXlsFile = new byte[] {1, 1, 1, 1, 1};
            nv.Validate(cossuptedXlsFile, "xls").ShouldEqual(MediaStatusCode.Corrupted);
        }

        [Test]
        public void ExcelMediaValidator_Validate_ReturnsValid()
        {
            var resourcesPath = Path.Combine(GetCurrentAssemblyFolder(),"Resources");
            var allGoodFiles = Directory.GetFiles(resourcesPath, "good.*");
            var nv = new ExcelMediaValidator();
            foreach (var f in allGoodFiles)
            {
                var fs = File.ReadAllBytes(f);
                nv.Validate(fs, "xls").ShouldEqual(MediaStatusCode.Valid);
            }
        }

        public string GetCurrentAssemblyFolder()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }
    }
}