using System;
using System.Collections.Generic;
using Shouldly;
using System.Linq;
using System.Text;
using Xunit;
using Saturn72.Core.Services.File.FileValidators;
using Saturn72.Core.Services.File;

namespace Saturn72.Core.Services.Tests.File.FileValidators
{
    public class JsonFileValidatorTests
    {
        [Fact]
        public void JsonFileValidator_ReturnExtensions()
        {
            var jfv = new JsonFileValidator();
            jfv.SupportedExtensions.Count().ShouldBe(1);
            jfv.SupportedExtensions.ShouldContain("json");
        }

        [Fact]
        public void JsonFileValidator_Validate_returns_UnSupported()
        {
            var jfv = new JsonFileValidator();
            jfv.Validate(null, "fff").ShouldBe(FileStatusCode.Unsupported);
        }

        [Fact]
        public void JsonFileValidator_Validate_returns_Invalid()
        {
            var jfv = new JsonFileValidator();
            var bytes = Encoding.UTF8.GetBytes("bad json");
            jfv.Validate(bytes, "json").ShouldBe(FileStatusCode.Invalid);
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

            var jfv = new JsonFileValidator();
            jfv.Validate(bytes, "json").ShouldBe(FileStatusCode.Valid);
        }
    }
}
