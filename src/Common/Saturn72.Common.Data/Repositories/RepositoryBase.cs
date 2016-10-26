#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Saturn72.Core.Data;
using Saturn72.Core.Domain;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.Data.Repositories
{
    public abstract class RepositoryBase<TDomainModel, TId, TEntity>
        : IRepository<TDomainModel, TId>
        where TDomainModel : DomainModelBase<TId> where TEntity : class
    {
        protected readonly IUnitOfWork<TId> UnitOfWork;

        protected RepositoryBase(IUnitOfWork<TId> unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public virtual IEnumerable<TDomainModel> GetAll()
        {
            return UnitOfWork.GetAll<TDomainModel, TEntity>();
        }

        public TDomainModel GetById(TId id)
        {
            return UnitOfWork.GetById<TDomainModel, TEntity>(id);
        }

        public TDomainModel Update(TDomainModel model)
        {
            Guard.NotNull(model);

            return UnitOfWork.Update<TDomainModel, TEntity>(model);
        }

        public TDomainModel Create(TDomainModel model)
        {
            Guard.NotNull(model);

            return UnitOfWork.Create<TDomainModel, TEntity>(model);
        }

        public async Task<TDomainModel> CreateAsync(TDomainModel model)
        {
            return await UnitOfWork.CreateAsync<TDomainModel, TEntity>(model);
        }

        public void Delete(TId id)
        {
            if (UnitOfWork.Delete<TEntity>(id) <= 0)
                throw new InvalidOperationException(
                    "Failed to delete table row. Type: {0}, row Id: {1}".AsFormat(typeof(TEntity), id));
        }

        public void Delete(IEnumerable<TId> ids)
        {
            UnitOfWork.Delete<TEntity>(ids);
        }

        public void Delete(TDomainModel model)
        {
            Delete(model.Id);
        }

        public void Delete(IEnumerable<TDomainModel> models)
        {
            UnitOfWork.Delete<TEntity>(models.Select(x => x.Id));
        }

        public IEnumerable<TDomainModel> GetBy(Func<TDomainModel, bool> func)
        {
            return UnitOfWork.GetAll<TDomainModel, TEntity>().Where(func).ToArray();
        }
    }
}