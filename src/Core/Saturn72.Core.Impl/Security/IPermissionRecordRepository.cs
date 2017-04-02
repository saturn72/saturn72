using Saturn72.Core.Domain.Security;

namespace Saturn72.Core.Services.Impl.Security
{
    public interface IPermissionRecordRepository
    {
        bool PermissionRecordExists(string permissionUniqueKey);
        void CreatePermissionRecord(PermissionRecordModel permissionRecordModel);
    }
}
