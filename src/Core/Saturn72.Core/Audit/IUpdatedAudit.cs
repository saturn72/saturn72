using System;

namespace Saturn72.Core.Audit
{
    public interface IUpdatedAudit:ICreatedAudit
    {
        DateTime UpdatedOnUtc { get; set; }
    }
}