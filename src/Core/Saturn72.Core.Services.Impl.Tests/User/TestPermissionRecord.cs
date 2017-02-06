using System.Collections.Generic;
using Saturn72.Core.Domain.Security;

namespace Saturn72.Core.Services.Impl.Tests.User
{
    public class TestPermissionRecords
    {
        internal static readonly PermissionRecordModel PermissionRecord1 = new PermissionRecordModel {UniqueKey = "uk1"};
        internal static readonly PermissionRecordModel PermissionRecord2 = new PermissionRecordModel {UniqueKey = "uk2"};
        internal static readonly PermissionRecordModel PermissionRecord3 = new PermissionRecordModel {UniqueKey = "uk3"};
        internal static readonly PermissionRecordModel PermissionRecord4 = new PermissionRecordModel {UniqueKey = "uk6"};

        internal static readonly IEnumerable<PermissionRecordModel> PermisisonCollection1 = new
            []

            {
                PermissionRecord1, PermissionRecord2, PermissionRecord3
            };
    }
}