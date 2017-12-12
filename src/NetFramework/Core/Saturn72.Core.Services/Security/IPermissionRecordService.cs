using Saturn72.Core.Domain.Security;

namespace Saturn72.Core.Services.Security
{
    public interface IPermissionRecordService
    {
        void CreatePermissionRecordIfNotExists(PermissionRecordModel model);
    }
}
