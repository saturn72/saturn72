using Saturn72.Core.Services.File;
using Saturn72.Core.Services.File.FileHandlers;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Saturn72.Core.Services.Tests.File.FileHandlers
{
    public class JsonFileHandlerTests
    {
        [Fact]
        public void JsonFileValidator_ReturnExtensions()
        {
            var jfv = new JsonFileHandler();
            jfv.SupportedExtensions.Count().ShouldBe(1);
            jfv.SupportedExtensions.ShouldContain("json");
        }

        [Fact]
        public void JsonFileValidator_Validate_returns_UnSupported()
        {
            var jfv = new JsonFileHandler();
            jfv.Validate(null, "fff", null).ShouldBe(FileStatusCode.Unsupported);
        }

        [Fact]
        public void JsonFileValidator_Validate_returns_Invalid()
        {
            var jfv = new JsonFileHandler();
            var bytes = Encoding.UTF8.GetBytes("bad json");
            jfv.Validate(bytes, "json", null).ShouldBe(FileStatusCode.Invalid);
        }
        public static IEnumerable<object[]> GetJsonArrayIndexes()
        {
            yield return  new object[]{0};
            yield return  new object[]{1};
        }
        [Theory]
        [MemberData(nameof(GetJsonArrayIndexes))]
        public void JsonFileValidator_Validate_returns_Valid(int i)
        {
            var arrayJson = "[{\"key1\":\"value1\"},{\"key2\":\"value2\"},{\"key3\":\"value3\"},{\"key4\":\"value4\"}]";
            var simpleJson = "{\"data\":{\"key1\":\"value1\",\"key2\":\"value2\",\"key3\":\"value3\"}}";

            var jsonArr = new[] {arrayJson, simpleJson};
            var bytes = Encoding.UTF8.GetBytes(jsonArr[i]);

            var jfv = new JsonFileHandler();
            jfv.Validate(bytes, "json", null).ShouldBe(FileStatusCode.Valid);
        }
    }
}
