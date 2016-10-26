using System;

namespace Saturn72.Core.Audit
{
    public interface ICreatedAudit
    {
        DateTime CreatedOnUtc { get; set; }
    }
}