using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Mimas.Common.Data.Repositories;
using Mimas.Common.Domain.SellerPolicy;
using Mimas.Common.Domain.Users;
using Mimas.Server.Services.SellerPolicy;
using Mimas.WebApi.Controllers;
using Mimas.WebApi.Models.SellerPolicy;
using Moq;
using NUnit.Framework;
using Saturn72.Mappers;
using Saturn72.UnitTesting.Framework;

namespace Mimas.WebApi.Tests.Controllers
{
    public class SellerPolicyControllerTests
    {
      
        [Test]
        public void SellerPolicyController_GetShippingPolicies_ReturnsBadRequest_OnIlegallSellerId()
        {
            var ctrl = new SellerPolicyController(null);
            var res = ctrl.GetShippingPolicies(null, 0).Result as BadRequestErrorMessageResult;
            res.ShouldNotBeNull();
            res.Message.Contains("Illegal").ShouldBeTrue();

            res = ctrl.GetShippingPolicies(null, -100).Result as BadRequestErrorMessageResult;
            res.ShouldNotBeNull();
            res.Message.Contains("Illegal").ShouldBeTrue();
        }

        [Test]
        public void SellerPolicyController_GetShippingPolicies_ReturnsBadRequest_OnNonExistSeller()
        {
            var sRepo = new Mock<ISellerPolicyService>();
            sRepo.Setup(r => r.GetSellerShippingPoliciesAsync(It.IsAny<long>()))
                .Returns(Task.FromResult<IEnumerable<SellerShippingPolicy>>(null));

            var ctrl = new SellerPolicyController(sRepo.Object);
            var res = ctrl.GetShippingPolicies(null, 123).Result as BadRequestErrorMessageResult;
            res.ShouldNotBeNull();
            res.Message.Contains("found").ShouldBeTrue();
        }


        [Test]
        public void SellerPolicyController_GetShippingPolicies_ReturnsPolicies()
        {
            var policies = new[]
            {
                new SellerShippingPolicy
                {
                    Id = 111111,
                    IsDefault = true,
                    Name = "Policy1",
                    PolicyId = 123123
                },
                new SellerShippingPolicy
                {
                    Id = 222,
                    IsDefault = true,
                    Name = "Policy2",
                    PolicyId = 22222
                },
                new SellerShippingPolicy
                {
                    Id = 333,
                    IsDefault = true,
                    Name = "Policy3",
                    PolicyId = 33333
                },
                new SellerShippingPolicy
                {
                    Id = 444,
                    IsDefault = true,
                    Name = "Policy4",
                    PolicyId = 44444
                }
            };
            var policiesAsApiModel =
                policies.Select(SimpleMapper.Map<SellerShippingPolicy, SellerShippingPolicyApiModel>);

            var spSrv = new Mock<ISellerPolicyService>();
            spSrv.Setup(s => s.GetSellerShippingPoliciesAsync(It.IsAny<long>()))
                .Returns(Task.FromResult((IEnumerable<SellerShippingPolicy>) policies));

            var ctrl = new SellerPolicyController(spSrv.Object);
            var res =
                ctrl.GetShippingPolicies(null, 123).Result as
                    OkNegotiatedContentResult<IEnumerable<SellerShippingPolicyApiModel>>;
            res.ShouldNotBeNull();

            var count = res.Content.Count();
            count.ShouldEqual(policies.Length);
            for (var i = 0; i < count; i++)
                res.Content.ElementAt(i)
                    .PropertyValuesAreEquals(policiesAsApiModel.ElementAt(i));
        }
    }
}