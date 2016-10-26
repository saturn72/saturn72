
using System.Threading.Tasks;

namespace Saturn72.Core.Services.User
{
    public interface IUserRegistrationService
    {
        Task<UserRegistrationResponse> RegisterAsync(UserRegistrationRequest request);
        bool ValidateUserByUsernameAndPassword(string usernameOrEmail, string password);
    }
}
