
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Services.User
{
    public interface IUserService
    {
        UserDomainModel GetUserByUsername(string username);
        UserDomainModel GetUserByEmail(string email);
        UserDomainModel GetUserBy(Func<UserDomainModel, bool> func);
        Task<IEnumerable<UserRoleDomainModel>> GetUserUserRolesByUserIdAsync(long userId);
        void UpdateUser(UserDomainModel user);
    }
}
