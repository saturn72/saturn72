﻿using System;
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
        }

        public virtual void PrepareForUpdateAudity(IUpdatedAudit audit)
        {
            if (audit.IsNull())
                return;
            if (audit.CreatedOnUtc == default(DateTime))
                PrepareForCreateAudity(audit);

            audit.UpdatedOnUtc = DateTime.UtcNow;
            audit.UpdatedByUserId = _workContext.CurrentUserId;
        }

        public virtual void PrepareForDeleteAudity(IDeletedAudit audit)
        {
            if (audit.IsNull())
                return;

            if (audit.DeletedOnUtc.NotNull() || audit.DeletedByUserId != 0 || audit.Deleted)
                throw new InvalidOperationException("DeletedAudit already deleted.");

            audit.DeletedOnUtc = DateTime.UtcNow;
            audit.DeletedByUserId = _workContext.CurrentUserId;
            audit.Deleted = true;
        }
    }
}