using System.Collections.Generic;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Services.Impl.Security
{
    public interface IPermissionRecordRepository
    {
        bool PermissionRecordExists(string permissionUniqueKey);
        void CreatePermissionRecord(PermissionRecordModel permissionRecordModel);
        IEnumerable<UserRoleModel> GetUserUserRoles(long userId);
        IEnumerable<PermissionRecordModel> GetUserPermissions(long userId);
    }
}