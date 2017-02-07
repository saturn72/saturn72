#region

using System;
using System.Collections.Generic;
using Saturn72.Core.Data;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Domain.Users;

#endregion

namespace Saturn72.Core.Services.Impl.User
{
    public interface IUserRepository : IRepository<UserModel>
    {
        IEnumerable<UserRoleModel> GetUserUserRoles(long userId);
        IEnumerable<PermissionRecordModel> GetUserPermissions(long userId);
    }
}