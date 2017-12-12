using System.Collections.Generic;
using Mimas.Common.Domain.SellerPolicy;

namespace Mimas.Framework.Data.Repositories
{
    public interface ISellerPolicyRepository
    {
        IEnumerable<SellerShippingPolicy> GetSellerShippingPolicies(long sellerId);
    }
}
