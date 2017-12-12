using System;
using Saturn72.Core.Domain;

namespace Saturn72.Core.Audit
{
    public class SuspendPoint : DomainModelBase
    {
        public DateTime SuspendedOnUtc { get; set; }
        public SuspensionReason SuspensionResReason { get; set; }
        public string Description { get; set; }
    }
}