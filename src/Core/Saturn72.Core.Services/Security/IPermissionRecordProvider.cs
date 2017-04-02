using System.Collections.Generic;
using Saturn72.Core.Domain.Security;

namespace Saturn72.Core.Services.Security
{
    public interface IPermissionRecordProvider
    {
        IEnumerable<PermissionRecordModel> Permissions { get; }
    }
}
