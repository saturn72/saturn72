using System.Collections.Generic;
using Saturn72.Core.Domain.Security;

namespace Saturn72.Core.Services.Impl.Security
{
    public interface IPermissionRecordRepository
    {
        IEnumerable<PermissionRecordModel> GetUserPermissions(long userId);
    }
}