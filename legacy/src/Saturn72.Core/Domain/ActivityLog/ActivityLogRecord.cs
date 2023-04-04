#region Usings

using System;

#endregion

namespace Saturn72.Core.Domain.ActivityLog
{
    public class ActivityLogRecord : DomainModelBase
    {
        public ActivityLogEntityType EntityType { get; set; }

        public long EntityId { get; set; }

        public ActivityLogAction LogAction { get; set; }

        public DomainModelBase Model { get; set; }

        public DateTime ActivityDoneOnUtc { get; set; }
    }
}