#region

using System.Collections.Generic;
using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Services.Impl.Data
{
    public interface IUnitOfWork<TDomainModel>
        where TDomainModel : DomainModelBase
    {
        IEnumerable<TDomainModel> GetAll();
        TDomainModel GetById(long id);
        TDomainModel Replace(TDomainModel model);
        TDomainModel Update(TDomainModel model);
        object Create(TDomainModel model);
        int Delete(long id);
        int Delete(IEnumerable<long> ids);
    }
}