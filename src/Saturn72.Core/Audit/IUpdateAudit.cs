using System;

namespace Saturn72.Core.Audit
{
    public interface IUpdateAudit : ICreateAudit
    {
        DateTime? UpdatedOnUtc { get; set; }
        long? UpdatedByUserId { get; set; }
    }
}