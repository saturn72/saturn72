using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mimas.Common.Domain.SellerPolicy;

namespace Mimas.Server.Services.SellerPolicy
{
    public interface ISellerPolicyService
    {
        Task<IEnumerable<SellerShippingPolicy>> GetSellerShippingPoliciesAsync(long sellerId);
    }
}
