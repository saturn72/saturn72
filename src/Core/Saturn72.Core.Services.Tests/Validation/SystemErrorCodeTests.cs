
using NUnit.Framework;
using Saturn72.Core.Services.Validation;
using Shouldly;

namespace Saturn72.Core.Services.Tests.Validation
{
    public class SystemErrorCodeTests
    {
     [Test]
        public void SystemErrorCodeExtensions_Flatten_ValidateFormat()
        {
            var code = "code";
            var message = "message";
            var category = "category";
            var subcategory = "subcategory";
            var expected = string.Format("<= Code: {0} <=> Message: {1} <=> Category: {2} <=> Subcategory: {3} =>", code,
                message, category, subcategory);

                new SystemErrorCode(code, message, category, subcategory).Flatten().ShouldBe(expected);
        }
    }
}
