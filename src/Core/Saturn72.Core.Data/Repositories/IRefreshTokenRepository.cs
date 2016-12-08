using Saturn72.Core.Domain.Security;

namespace Saturn72.Core.Data.Repositories
{
    public interface IRefreshTokenRepository:IRepository<RefreshTokenDomainModel, long>
    {
    }
}
