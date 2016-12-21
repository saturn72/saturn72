using System;
using Saturn72.Core.Audit;

namespace Saturn72.Core.Domain.Tasks
{
    public class BackgroundTaskAttachtmentDomainModel : DomainModelBase<long>, IUpdatedAudit<long>
    {
        public bool IsPrimary { get; set; }
        public long BackgroundTaskId { get; set; }
        public string FilePath { get; set; }

        public byte[] Bytes { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public long CreatedByUserId { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public long UpdatedByUserId { get; set; }
    }
}