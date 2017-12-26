using System;
using Saturn72.Core.Audit;
using Saturn72.Extensions;

namespace Saturn72.Core.Services
{
    public class AuditHelper
    {
        private readonly IWorkContext _workContext;

        public AuditHelper(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        public virtual void PrepareForCreateAudity(ICreateAudit audit)
        {
            if (audit.IsNull())
                return;
            if (audit.CreatedOnUtc != default(DateTime) || audit.CreatedByUserId != 0)
                throw new InvalidOperationException("Create audit already initialized.");

            audit.CreatedOnUtc = DateTime.UtcNow;
            audit.CreatedByUserId = _workContext.CurrentUserId;
            ResetUpdatedAudity(audit as IUpdateAudit);
            ResetDeletedAudity(audit as IDeleteAudit);
            SetBrowseData(audit as IAccessAudit);
        }

        public virtual void PrepareForUpdateAudity(IUpdateAudit audit)
        {
            if (audit.IsNull())
                return;
            if (audit.CreatedOnUtc == default(DateTime))
                PrepareForCreateAudity(audit);

            audit.UpdatedOnUtc = DateTime.UtcNow;
            audit.UpdatedByUserId = _workContext.CurrentUserId;

            ResetDeletedAudity(audit as IDeleteAudit);
        }

        public virtual void PrepareForDeleteAudity(IDeleteAudit audit)
        {
            if (audit.IsNull())
                return;

            Func<bool> deletedByUser = () => audit.DeletedByUserId.NotNull() && audit.DeletedByUserId.Value > 0;
            if (audit.DeletedOnUtc.NotNull() || deletedByUser())
                throw new InvalidOperationException("DeletedAudit already deleted.");

            audit.DeletedOnUtc = DateTime.UtcNow;
            audit.DeletedByUserId = _workContext.CurrentUserId;

            SetBrowseData(audit as IAccessAudit);
        }
        #region Utilities

        private void SetBrowseData(IAccessAudit audit)
        {
            if (audit.IsNull())
                return;
            audit.LastAccessedOnUtc = DateTime.UtcNow;
            audit.LastAccessedIpAddress = _workContext.CurrentUserIpAddress;
            audit.LastAccessedByUserId = _workContext.CurrentUserId;
            audit.LastAccessedAppId = _workContext.ClientId;
        }


        private void ResetUpdatedAudity(IUpdateAudit audit)
        {
            if (audit.IsNull())
                return;
            audit.UpdatedByUserId = null;
            audit.UpdatedOnUtc = null;
        }
        private void ResetDeletedAudity(IDeleteAudit audit)
        {
            if (audit.IsNull())
                return;
            audit.DeletedByUserId = null;
            audit.DeletedOnUtc = null;
        }

        #endregion
    }
}