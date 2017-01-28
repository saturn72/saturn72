#region

using System;
using System.Collections.Generic;
using Saturn72.Core.Data;
using Saturn72.Core.Domain.Users;

#endregion

namespace Saturn72.Core.Services.Impl.User
{
    public interface IUserRepository : IRepository<UserModel>
    {
        IEnumerable<UserRoleDomainModel> GetUserUserRoles(long userId);
    }
}