using System;
using Saturn72.Core.Audit;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl
{
    public class AuditHelper
    {
        private readonly IWorkContext _workContext;

        public AuditHelper(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        public virtual void PrepareForCreateAudity(ICreatedAudit audit)
        {
            if (audit.IsNull())
                return;
            if (audit.CreatedOnUtc != default(DateTime) || audit.CreatedByUserId != 0)
                throw new InvalidOperationException("Create audit already initialized.");

            audit.CreatedOnUtc = DateTime.UtcNow;
            audit.CreatedByUserId = _workContext.CurrentUserId;
            ResetUpdatedAudity(audit as IUpdatedAudit);
            ResetDeletedAudity(audit as IDeletedAudit);
            SetBrowseData(audit as IBrowseDataAudit);
        }

        public virtual void PrepareForUpdateAudity(IUpdatedAudit audit)
        {
            if (audit.IsNull())
                return;
            if (audit.CreatedOnUtc == default(DateTime))
                PrepareForCreateAudity(audit);

            audit.UpdatedOnUtc = DateTime.UtcNow;
            audit.UpdatedByUserId = _workContext.CurrentUserId;

            ResetDeletedAudity(audit as IDeletedAudit);
    }

    public virtual void PrepareForDeleteAudity(IDeletedAudit audit)
        {
            if (audit.IsNull())
                return;

            Func<bool> deletedByUser = () => audit.DeletedByUserId.NotNull() && audit.DeletedByUserId.Value > 0;
            if (audit.DeletedOnUtc.NotNull() || audit.Deleted || deletedByUser())
                throw new InvalidOperationException("DeletedAudit already deleted.");

            audit.DeletedOnUtc = DateTime.UtcNow;
            audit.DeletedByUserId = _workContext.CurrentUserId;
            audit.Deleted = true;

            SetBrowseData(audit as IBrowseDataAudit);
        }
        #region Utilities

        private void SetBrowseData(IBrowseDataAudit audit)
        {
            if (audit.IsNull())
                return;
            audit.LastBrowsedOnUtc = DateTime.UtcNow;
            audit.LastIpAddress = _workContext.CurrentUserIpAddress;
            audit.LastClientAppId = _workContext.ClientId;
        }


        private void ResetUpdatedAudity(IUpdatedAudit audit)
        {
            if (audit.IsNull())
                return;
            audit.UpdatedByUserId = null;
            audit.UpdatedOnUtc = null;
        }
        private void ResetDeletedAudity(IDeletedAudit audit)
        {
            if (audit.IsNull())
                return;
            audit.Deleted = false;
            audit.DeletedByUserId = null;
            audit.DeletedOnUtc = null;
        }

        #endregion
    }
}