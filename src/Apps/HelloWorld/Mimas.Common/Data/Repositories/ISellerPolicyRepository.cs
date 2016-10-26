using System.Collections.Generic;
using System.Threading.Tasks;
using Mimas.Common.Domain.SellerPolicy;

namespace Mimas.Common.Data.Repositories
{
    public interface ISellerPolicyRepository
    {
        Task<IEnumerable<SellerShippingPolicy>> GetSellerShippingPoliciesAsync(long sellerId);
    }
}