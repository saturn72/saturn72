using System;

namespace Saturn72.Core.Audit
{
    public interface IPrivacyAudit
    {
        bool IsPrivate { get; set; }
        DateTime? SetToPrivateOnUtc { get; set; }
        long? SetToPrivateByUserId { get; set; }
    }
}
