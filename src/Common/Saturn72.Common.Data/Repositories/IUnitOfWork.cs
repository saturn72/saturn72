#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Common.Data.Repositories
{
    public interface IUnitOfWork<TDomainModel, in TId>
        where TDomainModel : DomainModelBase<TId>
    {
        IEnumerable<TDomainModel> GetAll();
        TDomainModel GetById(TId id);

        TDomainModel Replace(TDomainModel model);

        TDomainModel Update(TDomainModel model);

        TDomainModel Create(TDomainModel model);

        Task<TDomainModel> CreateAsync(TDomainModel model);

        int Delete(TId id);
        int Delete(IEnumerable<TId> ids);
    }
}