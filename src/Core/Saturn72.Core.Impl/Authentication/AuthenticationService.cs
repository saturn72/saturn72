#region

using System.Threading.Tasks;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Authentication;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Events;

#endregion

namespace Saturn72.Core.Services.Impl.Authentication
{
    public class AuthenticationService : DomainModelCrudServiceBase<RefreshTokenDomainModel, long>, IAuthenticationService
    {
        public AuthenticationService(IRefreshTokenRepository refreshTokenRepository, IEventPublisher eventPublisher,
            ICacheManager cacheManager, ITypeFinder typeFinder)
            : base(refreshTokenRepository, eventPublisher, cacheManager, typeFinder)
        {
        }


        public Task AddRefreshTokenAsync(RefreshTokenDomainModel token)
        {
            return CreateAsync(token);
        }
    }
}