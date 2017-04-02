using Saturn72.Core.Domain.Security;
using Saturn72.Core.Services.Security;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.Security
{
    public class PermissionRecordService:IPermissionRecordService
    {
        private readonly IPermissionRecordRepository _permissionRecordRepository;

        public PermissionRecordService(IPermissionRecordRepository permissionRecordRepository)
        {
            _permissionRecordRepository = permissionRecordRepository;
        }

        public void CreatePermissionRecordIfNotExists(PermissionRecordModel model)
        {
            Guard.NotNull(model);
            Guard.HasValue(model.UniqueKey);
            Guard.HasValue(model.Description);

            if (_permissionRecordRepository.PermissionRecordExists(model.UniqueKey))
                return;
            _permissionRecordRepository.CreatePermissionRecord(model);
        }
    }
}
