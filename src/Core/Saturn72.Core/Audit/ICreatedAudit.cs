using System;

namespace Saturn72.Core.Audit
{
    public interface ICreatedAudit<TUserId>
    {
        DateTime CreatedOnUtc { get; set; }

        TUserId CreatedByUserId { get; set; }
    }
}