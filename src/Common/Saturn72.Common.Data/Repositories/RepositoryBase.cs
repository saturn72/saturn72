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
    public abstract class RepositoryBase<TDomainModel, TEntity>
        : IRepository<TDomainModel>
        where TDomainModel : DomainModelBase where TEntity : class
    {
        protected readonly IUnitOfWork<TDomainModel> UnitOfWork;

        protected RepositoryBase(IUnitOfWork<TDomainModel> unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public virtual IEnumerable<TDomainModel> GetAll()
        {
            return UnitOfWork.GetAll();
        }

        public TDomainModel GetById(long id)
        {
            return UnitOfWork.GetById(id);
        }

        public TDomainModel Update(TDomainModel model)
        {
            Guard.NotNull(model);

            return UnitOfWork.Update(model);
        }

        public TDomainModel Create(TDomainModel model)
        {
            Guard.NotNull(model);

            return UnitOfWork.Create(model);
        }

        public async Task<TDomainModel> CreateAsync(TDomainModel model)
        {
            return await UnitOfWork.CreateAsync(model);
        }

        public void Delete(long id)
        {
            if (UnitOfWork.Delete(id) <= 0)
                throw new InvalidOperationException(
                    "Failed to delete table row. Type: {0}, row Id: {1}".AsFormat(typeof(TEntity), id));
        }

        public void Delete(IEnumerable<long> ids)
        {
            UnitOfWork.Delete(ids);
        }

        public void Delete(TDomainModel model)
        {
            Delete(model.Id);
        }

        public void Delete(IEnumerable<TDomainModel> models)
        {
            UnitOfWork.Delete(models.Select(x => x.Id));
        }

        public IEnumerable<TDomainModel> GetBy(Func<TDomainModel, bool> func)
        {
            return UnitOfWork.GetAll().Where(func).ToArray();
        }
    }
}