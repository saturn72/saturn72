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
    public abstract class DomainModelCrudServiceBase<TDomainModel, TId, TUserId>
        where TDomainModel : DomainModelBase<TId>
        where TId : IComparable
    {
        #region ctor

        protected DomainModelCrudServiceBase(IRepository<TDomainModel, TId> modelRepository, IEventPublisher eventPublisher, ICacheManager cacheManager, ITypeFinder typeFinder, IWorkContext<TUserId> workContext)
        {
            ModelRepository = modelRepository;
            EventPublisher = eventPublisher;
            CacheManager = cacheManager;
            TypeFinder = typeFinder;
            WorkContext = workContext;
        }

        #endregion

        protected virtual IEnumerable<TDomainModel> GetAll()
        {
            return ModelRepository.GetAll();
        }

        protected virtual IEnumerable<TDomainModel> FilterTable(Func<TDomainModel, bool> filter = null)
        {
            return filter.IsNull()
                ? GetAll().ToArray()
                : FilterCollection(GetAll(), filter);
        }

        protected virtual IEnumerable<TDomainModel> FilterCollection(IEnumerable<TDomainModel> entities,
            Func<TDomainModel, bool> filter)
        {
            return (filter.IsNull() ? entities : entities.Where(filter)).ToArray();
        }

        protected virtual async Task<TDomainModel> CreateAsync(TDomainModel model)
        {
            return await Task.Run(() => Create(model));
        }

        protected virtual TDomainModel Create(TDomainModel model)
        {
            Guard.NotNull(model);
            PrepareModelBeforeCreateAction(model);
            var domainModel = ModelRepository.Create(model);
            EventPublisher.DomainModelCreated<TDomainModel, TId>(model);
            return domainModel;
        }

        protected virtual TDomainModel Update(TDomainModel model)
        {
            Guard.NotNull(model);
            PrepareModelBeforeUpdateAction(model);

            ModelRepository.Update(model);
            EventPublisher.DomainModelUpdated<TDomainModel, TId>(model);

            return model;
        }

        protected Task<TDomainModel> UpdateAsync(TDomainModel model)
        {
            return Task.Run(() => Update(model));
        }

        protected void Delete(TId id)
        {
            ValidateNonDefaultId(id);

            var model = GetById(id);

            var deletedAudit = model as IDeletedAudit<TUserId>;
            if (deletedAudit.NotNull())
            {
                deletedAudit.Deleted = true;
                deletedAudit.DeletedOnUtc = DateTime.UtcNow;
                deletedAudit.DeletedByUserId = WorkContext.CurrentUserId;
                ModelRepository.Update(model);
            }
            else
            {
                ModelRepository.Delete(id);
            }

            EventPublisher.DomainModelDeleted<TDomainModel, TId>(model);
        }

        private static void ValidateNonDefaultId(TId id)
        {
            Guard.MustFollow(id.CompareTo(id.GetType().GetDefault()) > 0);
        }

        protected TDomainModel GetById(TId id)
        {
            ValidateNonDefaultId(id);
            return ModelRepository.GetById(id);
        }

        #region Fields

        protected readonly ICacheManager CacheManager;
        protected readonly IEventPublisher EventPublisher;
        protected readonly IRepository<TDomainModel, TId> ModelRepository;
        protected readonly ITypeFinder TypeFinder;
        protected readonly IWorkContext<TUserId> WorkContext;

        #endregion

        #region Utilities

        protected virtual void PrepareModelBeforeUpdateAction<T>(T model)
        {
            var updatedAudit = model as IUpdatedAudit<TUserId>;

            if (updatedAudit.NotNull())
            {
                updatedAudit.UpdatedOnUtc = DateTime.UtcNow;
                updatedAudit.UpdatedByUserId = WorkContext.CurrentUserId;
            }
        }

        protected virtual void PrepareModelBeforeCreateAction<T>(T model)
        {
            var createdAudit = model as ICreatedAudit<TUserId>;
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