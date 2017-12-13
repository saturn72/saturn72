#region Usings

using Saturn72.Core.Domain;
using Saturn72.Core.Services;
using Shouldly;
using Xunit;

#endregion

namespace Saturn72.Core.Services.Tests
{
    public class ServiceResponseExtensionsTests
    {
        [Fact]
        public void ServiceResponseExtensions_HasError_ShouldReturnFalse()
        {
            var res = new ServiceResponse<SomeModel>();
            res.HasErrors().ShouldBeFalse();
        }

        [Fact]
        public void ServiceResponseExtensions_HasError_ShouldReturnTrue()
        {
            var res = new ServiceResponse<SomeModel> {ErrorMessage = "qwe"};
            res.HasErrors().ShouldBeTrue();
        }

        internal class SomeModel : DomainModelBase
        {
        }
    }
}