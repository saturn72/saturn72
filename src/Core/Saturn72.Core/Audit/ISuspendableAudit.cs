using System.Collections.Generic;

namespace Saturn72.Core.Audit
{
    public interface ISuspendableAudit
    {
        IEnumerable<SuspendPoint> SuspendedOnUtc { get; set; }
    }
}