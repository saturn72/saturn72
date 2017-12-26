using System;

namespace Saturn72.Core.Audit
{
    public interface ICreateAudit
    {
        DateTime CreatedOnUtc { get; set; }
        long CreatedByUserId { get; set; }
    }
}