using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Services.User
{
    public interface IUserService
    {
        Task<IEnumerable<UserDomainModel>> GetAllUsersAsync();

        Task<UserDomainModel> GetUserByUsername(string username);
        Task<UserDomainModel> GetUserByEmail(string email);
        Task<IEnumerable<UserRoleDomainModel>> GetUserUserRolesByUserIdAsync(long userId);
        Task UpdateUser(UserDomainModel user);
    }
}