#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Common.Data.Repositories
{
    public interface IUnitOfWork<TDomainModel>
        where TDomainModel : DomainModelBase
    {
        IEnumerable<TDomainModel> GetAll();
        TDomainModel GetById(long id);

        TDomainModel Replace(TDomainModel model);

        TDomainModel Update(TDomainModel model);

        TDomainModel Create(TDomainModel model);

        Task<TDomainModel> CreateAsync(TDomainModel model);

        int Delete(long id);
        int Delete(IEnumerable<long> ids);
    }
}