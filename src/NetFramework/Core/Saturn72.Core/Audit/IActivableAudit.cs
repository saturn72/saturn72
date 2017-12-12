using System;
using System.Collections.Generic;

namespace Saturn72.Core.Audit
{
    public interface IActivableAudit
    {
        IEnumerable<DateTime> ActivatedOnUtc { get; set; }
        IEnumerable<DateTime> DeactivatedOnUtc { get; set; }
    }
}