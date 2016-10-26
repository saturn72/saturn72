using System.Threading.Tasks;
using Saturn72.Core.Domain.Security;

namespace Saturn72.Core.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task AddRefreshTokenAsync(RefreshTokenDomainModel token);
    }
}