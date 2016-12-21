using System;

namespace Saturn72.Core.Audit
{
    public interface IUpdatedAudit<TUserId> : ICreatedAudit<TUserId>
    {
        DateTime UpdatedOnUtc { get; set; }
        TUserId UpdatedByUserId { get; set; }
    }
}