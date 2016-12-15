#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Data
{
    public interface IRepository<TDomainModel, in TId>
        where TDomainModel : DomainModelBase<TId>
    {
        IEnumerable<TDomainModel> GetAll();

        TDomainModel GetById(TId id);
        IEnumerable<TDomainModel> GetBy(Func<TDomainModel, bool> func);

        TDomainModel Update(TDomainModel model);

        TDomainModel Create(TDomainModel model);

        Task<TDomainModel> CreateAsync(TDomainModel model);

        void Delete(TId id);

        void Delete(IEnumerable<TId> ids);
    }
}