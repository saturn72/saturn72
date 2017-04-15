using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Services.User
{
    public interface IUserService
    {
        Task<UserModel> GetUserByUsernameAsync(string username);
        Task<UserModel> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserRoleModel>> GetUserUserRolesByUserIdAsync(long userId);
        Task UpdateUserAsync(UserModel user);
        Task<IEnumerable<PermissionRecordModel>> GetUserPermissionsAsync(long userId);
    }
}