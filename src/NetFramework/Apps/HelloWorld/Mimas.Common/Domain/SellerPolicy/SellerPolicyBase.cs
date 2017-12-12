using Saturn72.Core.Domain;

namespace Mimas.Common.Domain.SellerPolicy
{
    public abstract class SellerPolicyBase : DomainModelBase<long>
    {
        #region ctor

        protected SellerPolicyBase(SellerPolicyType sellerPolicyType)
        {
            SellerPolicyType = sellerPolicyType;
        }

        #endregion

        internal SellerPolicyType SellerPolicyType { get; }
        public bool IsDefault { get; set; }
        public long PolicyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        #region Fields

        #endregion
    }
}