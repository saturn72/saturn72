using System;

namespace Saturn72.Core.Audit
{
    public interface IDeletedAudit<TUserId>
    {
        bool Deleted { get; set; }

        DateTime? DeletedOnUtc { get; set; }

        TUserId DeletedByUserId { get; set; }
    }
}