using System.Threading.Tasks;
using Mimas.Common.Domain.Users;

namespace Mimas.Common.Data.Repositories
{
    public interface ISellerRepository
    {
        Task<SellerDomainModel> GetSellerByIdAsync(long sellerId);
    }
}
