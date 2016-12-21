#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain.Users;

#endregion

namespace Saturn72.Core.Data.Repositories
{
    public interface IUserRepository : IRepository<UserDomainModel, long>
    {
        void CreateUser(UserDomainModel user);
        IEnumerable<UserDomainModel> GetUsersBy(Func<UserDomainModel, bool> func);
        IEnumerable<UserRoleDomainModel> GetUserUserRoles(long userId);
    }
}