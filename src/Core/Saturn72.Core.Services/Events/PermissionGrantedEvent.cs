using System.Collections.Generic;
using Saturn72.Core.Domain.Security;

namespace Saturn72.Core.Services.Events
{
    public class PermissionGrantedEvent : EventBase
    {
        public long GrantedUserId { get; set; }
        public IEnumerable<PermissionRecordModel> GrantedPermissionRecords { get; set; }
    }
}