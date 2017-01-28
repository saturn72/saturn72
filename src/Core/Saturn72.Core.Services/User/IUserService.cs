using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Services.User
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetAllUsersAsync();

        Task<UserModel> GetUserByUsername(string username);
        Task<UserModel> GetUserByEmail(string email);
        Task<IEnumerable<UserRoleDomainModel>> GetUserUserRolesByUserIdAsync(long userId);
        Task UpdateUser(UserModel user);
    }
}