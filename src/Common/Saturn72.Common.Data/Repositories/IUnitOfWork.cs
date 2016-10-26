#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Common.Data.Repositories
{
    public interface IUnitOfWork<TId>
    {
        IEnumerable<TDomainModel> GetAll<TDomainModel, TEntity>()
            where TDomainModel : DomainModelBase<TId>
            where TEntity : class;

        TDomainModel GetById<TDomainModel, TEntity>(TId id)
            where TEntity : class
            where TDomainModel : DomainModelBase<TId>;

        TDomainModel Replace<TDomainModel, TEntity>(TDomainModel model)
            where TEntity : class
            where TDomainModel : DomainModelBase<TId>;

        TDomainModel Update<TDomainModel, TEntity>(TDomainModel model)
            where TEntity : class
            where TDomainModel : DomainModelBase<TId>;

        TDomainModel Create<TDomainModel, TEntity>(TDomainModel model)
            where TEntity : class
            where TDomainModel : DomainModelBase<TId>;

        Task<TDomainModel> CreateAsync<TDomainModel, TEntity>(TDomainModel model)
            where TEntity : class where TDomainModel : DomainModelBase<TId>;

        int Delete<TEntity>(TId id) where TEntity : class;
        int Delete<TEntity>(IEnumerable<TId> ids) where TEntity : class;
    }
}