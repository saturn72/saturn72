using System;

namespace Saturn72.Core.Audit
{
    public interface IDeleteAudit : ICreateAudit
    {
        DateTime? DeletedOnUtc
        {
            get; set;
        }
        long? DeletedByUserId { get; set; }
    }
}