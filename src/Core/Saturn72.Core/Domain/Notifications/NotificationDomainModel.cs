using System;
using Saturn72.Core.Audit;

namespace Saturn72.Core.Domain.Notifications
{
    public class NotificationDomainModel : DomainModelBase<long>, IUpdatedAudit
    {
        public string Name { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}