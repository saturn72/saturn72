using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Mimas.Common.Domain.SellerPolicy;
using Mimas.Server.Services.SellerPolicy;
using Mimas.WebApi.Models.SellerPolicy;
using Saturn72.Common.WebApi;
using Saturn72.Extensions;

namespace Mimas.WebApi.Controllers
{
    [RoutePrefix("api/sellerpolicy")]
    public class SellerPolicyController : Saturn72ApiControllerBase
    {
        private readonly ISellerPolicyService _sellerPolicyService;

        public SellerPolicyController(ISellerPolicyService sellerPolicyService)
        {
            _sellerPolicyService = sellerPolicyService;
        }

        [Route("shipping")]
        public async Task<IHttpActionResult> GetShippingPolicies(HttpRequestMessage request, long sellerId)
        {
            if (sellerId <= 0)
                return BadRequest("Illegal seller Id.");

            var policies = await _sellerPolicyService.GetSellerShippingPoliciesAsync(sellerId);
            if (policies.IsNull())
                return BadRequest("The seller Id could not be found: " + sellerId);

            var apiModels = policies.Select(p => p.ToApiModel<SellerShippingPolicy, SellerShippingPolicyApiModel>());
            return Ok(apiModels);
        }
    }
}