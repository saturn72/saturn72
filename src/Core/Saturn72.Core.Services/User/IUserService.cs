
using System;
using System.Collections.Generic;
using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Services.User
{
    public interface IUserService
    {
        UserDomainModel GetUserByUsername(string username);
        UserDomainModel GetUserByEmail(string email);
        UserDomainModel GetUserBy(Func<UserDomainModel, bool> func);
        IEnumerable<UserRoleDomainModel> GetUserUserRolesByUserId(long userId);
        void UpdateUser(UserDomainModel user);
    }
}
