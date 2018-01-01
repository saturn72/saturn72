#region Usings

using System.Collections.Generic;
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
            var res = new ServiceResponse<SomeModel>(ServiceRequestType.Create);
            res.HasErrors().ShouldBeFalse();
        }

        [Fact]
        public void ServiceResponseExtensions_HasError_ShouldReturnTrue()
        {
            var res = new ServiceResponse<SomeModel>(ServiceRequestType.Create) { ErrorMessage = "qwe" };
            res.HasErrors().ShouldBeTrue();
        }
        [Theory]
        [MemberData(nameof(ServiceResponseExtensions_IsFullySuccess_Fails_Data))]
        public void ServiceResponseExtensions_IsFullySuccess_Fails(ServiceResponse<SomeModel> srvResponse)
        {
            srvResponse.IsFullySuccess().ShouldBeFalse();
        }

        public static IEnumerable<object[]> ServiceResponseExtensions_IsFullySuccess_Fails_Data => new[]
        {
            new object[]
            {
                new ServiceResponse<SomeModel>(ServiceRequestType.Create)
                    {Result = ServiceResponseResult.Unknown},
            },
            new object[]
            {
                new ServiceResponse<SomeModel>(ServiceRequestType.Create)
                {
                    Result = ServiceResponseResult.Unknown,
                    ErrorMessage = "dadada"
                }
            },
            new object[]
            {
                new ServiceResponse<SomeModel>(ServiceRequestType.Create)
                    {Result = ServiceResponseResult.Fail},
            },
            new object[]
            {
                new ServiceResponse<SomeModel>(ServiceRequestType.Create)
                {
                    Result = ServiceResponseResult.Fail,
                    ErrorMessage = "dadada"
                }
            },
            new object[]
            {
                new ServiceResponse<SomeModel>(ServiceRequestType.Create)
                    {Result = ServiceResponseResult.Partial},
            },
            new object[]
            {
                new ServiceResponse<SomeModel>(ServiceRequestType.Create)
                {
                    Result = ServiceResponseResult.Partial,
                    ErrorMessage = "dadada"
                }
            },
        };


        [Fact]
        public void ServiceResponseExtensions_IsFullySuccess_Success()
        {
            var srvResponse = new ServiceResponse<SomeModel>(ServiceRequestType.Create)
            {
                Result = ServiceResponseResult.Success
            };
            srvResponse.IsFullySuccess().ShouldBeTrue();
        }

        public class SomeModel : DomainModelBase
        {
        }
    }
}