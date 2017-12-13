using System;
using Saturn72.Extensions;

namespace Saturn72.Core.Audit
{
    public static class AuditExtensions
    {
        public static bool WasUpdated(this IUpdateAudit audit)
        {
            return audit.UpdatedOnUtc != default(DateTime);
        }

        public static bool WasDeleted(this IDeleteAudit audit)
        {
            return audit.DeletedOnUtc.NotNull();
        }
    }
}