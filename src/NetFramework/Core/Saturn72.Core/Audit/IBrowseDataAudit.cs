using System;

namespace Saturn72.Core.Audit
{
    public interface IBrowseDataAudit
    {
        DateTime LastBrowsedOnUtc { get; set; }
        string LastClientAppId { get; set; }
        string LastIpAddress { get; set; }
    }
}