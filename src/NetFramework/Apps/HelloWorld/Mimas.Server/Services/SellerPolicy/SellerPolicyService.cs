using System.Collections.Generic;
using System.Threading.Tasks;
using Mimas.Common.Data.Repositories;
using Mimas.Common.Domain.SellerPolicy;
using Saturn72.Extensions;

namespace Mimas.Server.Services.SellerPolicy
{
    public class SellerPolicyService : ISellerPolicyService
    {
        private readonly ISellerPolicyRepository _sellerPolicyRepository;
        private readonly ISellerRepository _sellerRepository;

        public SellerPolicyService(ISellerRepository sellerRepository, ISellerPolicyRepository sellerPolicyRepository)
        {
            _sellerRepository = sellerRepository;
            _sellerPolicyRepository = sellerPolicyRepository;
        }

        public async Task<IEnumerable<SellerShippingPolicy>> GetSellerShippingPoliciesAsync(long sellerId)
        {
            Guard.GreaterThan(sellerId, default(long), "please specify sellerId > 0");

            var seller = await _sellerRepository.GetSellerByIdAsync(sellerId);
            if(seller.IsNull())
                return null;

            return await _sellerPolicyRepository.GetSellerShippingPoliciesAsync(sellerId);
        }
    }
}