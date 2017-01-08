using System;
using Saturn72.Core.Audit;
using Saturn72.Core.Infrastructure;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl
{
    public class AuditHelper
    {
        public static void PrepareForCreateAudity(ICreatedAudit audit)
        {
            if (audit.IsNull())
                return;
            if (audit.CreatedOnUtc != default(DateTime) || audit.CreatedByUserId != 0)
                throw new InvalidOperationException("Create audit already initialized.");

            audit.CreatedOnUtc = DateTime.UtcNow;
            audit.CreatedByUserId = AppEngine.Current.Resolve<IWorkContext>().CurrentUserId;
        }

        public static void PrepareForUpdateAudity(IUpdatedAudit audit)
        {
            if (audit.IsNull())
                return;

            audit.UpdatedOnUtc = DateTime.UtcNow;
            audit.UpdatedByUserId = AppEngine.Current.Resolve<IWorkContext>().CurrentUserId;
        }

        public static void PrepareForDeleteAudity(IDeletedAudit audit)
        {
            if (audit.IsNull())
                return;

            if (audit.DeletedOnUtc != default(DateTime) || audit.DeletedByUserId != 0 || audit.Deleted)
                throw new InvalidOperationException("DeletedAudit already deleted.");

            audit.DeletedOnUtc = DateTime.UtcNow;
            audit.DeletedByUserId = AppEngine.Current.Resolve<IWorkContext>().CurrentUserId;
            audit.Deleted = true;
        }
    }
}