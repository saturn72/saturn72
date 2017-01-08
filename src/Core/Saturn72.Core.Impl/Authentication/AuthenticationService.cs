#region

using System.Threading.Tasks;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Services.Authentication;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthenticationService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }


        public Task AddRefreshTokenAsync(RefreshTokenDomainModel token)
        {
            Guard.NotNull(token);
            return _refreshTokenRepository.CreateAsync(token);
        }
    }
}