#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Saturn72.Core.Audit;
using Saturn72.Core.Caching;
using Saturn72.Core.Data;
using Saturn72.Core.Domain;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Events;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl
{
    public abstract class DomainModelCrudServiceBase<TDomainModel>
        where TDomainModel : DomainModelBase
    {
        #region ctor

        protected DomainModelCrudServiceBase(IEventPublisher eventPublisher, ICacheManager cacheManager, ITypeFinder typeFinder, IWorkContext workContext)
        {
            EventPublisher = eventPublisher;
            CacheManager = cacheManager;
            TypeFinder = typeFinder;
            WorkContext = workContext;
        }

        #endregion

        protected virtual IEnumerable<TDomainModel> GetAll()
        {
            throw new NotImplementedException(
                );
            //return ModelRepository.GetAll();
        }

        protected virtual async Task<TDomainModel> CreateAndPublishCreatedEventAsync(TDomainModel model, Action<TDomainModel> createFunc)
        {

            return await Task.Run(() => CreateAndPublishCreatedEvent(model, createFunc));
        }

        protected virtual TDomainModel CreateAndPublishCreatedEvent(TDomainModel model, Action<TDomainModel> createFunc)
        {
            Guard.NotNull(model);
            PrepareModelBeforeCreateAction(model);
            throw new NotImplementedException();

            //var domainModel = ModelRepository.Create(model);
            //EventPublisher.DomainModelCreated<TDomainModel>(model);
            //  return domainModel;
        }

        protected virtual TDomainModel Update(TDomainModel model)
        {
            throw new NotImplementedException();

            //Guard.NotNull(model);
            //PrepareModelBeforeUpdateAction(model);

            //ModelRepository.Update(model);
            //EventPublisher.DomainModelUpdated<TDomainModel>(model);

            //return model;
        }

        protected Task<TDomainModel> UpdateAsync(TDomainModel model)
        {
            return Task.Run(() => Update(model));
        }

        protected void Delete(long id)
        {
            throw new NotImplementedException();

            //ValidateNonDefaullong(id);

            //var model = GetById(id);

            //var deletedAudit = model as IDeletedAudit;
            //if (deletedAudit.NotNull())
            //{
            //    deletedAudit.Deleted = true;
            //    deletedAudit.DeletedOnUtc = DateTime.UtcNow;
            //    deletedAudit.DeletedByUserId = WorkContext.CurrentUserId;
            //    ModelRepository.Update(model);
            //}
            //else
            //{
            //    ModelRepository.Delete(id);
            //}

            //EventPublisher.DomainModelDeleted(model);
        }

        private static void ValidateNonDefaullong(long id)
        {
            Guard.MustFollow(id.CompareTo(default(long)) > 0);
        }

        protected TDomainModel GetById(long id)
        {
            throw new NotImplementedException();

            //ValidateNonDefaullong(id);
            //return ModelRepository.GetById(id);
        }

        #region Fields

        protected readonly ICacheManager CacheManager;
        protected readonly IEventPublisher EventPublisher;
        protected readonly ITypeFinder TypeFinder;
        protected readonly IWorkContext WorkContext;

        #endregion

        #region Utilities

        protected virtual void PrepareModelBeforeUpdateAction<T>(T model)
        {
            var updatedAudit = model as IUpdatedAudit;

            if (updatedAudit.NotNull())
            {
                updatedAudit.UpdatedOnUtc = DateTime.UtcNow;
                updatedAudit.UpdatedByUserId = WorkContext.CurrentUserId;
            }
        }

        protected virtual void PrepareModelBeforeCreateAction<T>(T model)
        {
            var createdAudit = model as ICreatedAudit;
            if (createdAudit.NotNull())
            {
                createdAudit.CreatedOnUtc = DateTime.UtcNow;
                createdAudit.CreatedByUserId = WorkContext.CurrentUserId;
            }

            PrepareModelBeforeUpdateAction(model);
        }

        #endregion
    }
}