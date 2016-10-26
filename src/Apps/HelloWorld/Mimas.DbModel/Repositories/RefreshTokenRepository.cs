#region

using Saturn72.Common.Data.Repositories;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Services.Data.Repositories;

#endregion

namespace Mimas.DbModel.Repositories
{
    public class RefreshTokenRepository : RepositoryBase<RefreshTokenDomainModel, long, RefreshToken>,
        IRefreshTokenRepository
    {
        public RefreshTokenRepository(IUnitOfWork<long> unitOfWork) : base(unitOfWork)
        {
        }
    }
}