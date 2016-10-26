using System;

namespace Saturn72.Core.Audit
{
    public interface IDeletedAudit
    {
        bool Deleted { get; set; }

        DateTime? DeletedOnUtc { get; set; }
    }
}