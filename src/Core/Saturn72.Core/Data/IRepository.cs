#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Data
{
    public interface IRepository<TDomainModel>
        where TDomainModel : DomainModelBase
    {
        IEnumerable<TDomainModel> GetAll();

        TDomainModel GetById(long id);
        IEnumerable<TDomainModel> GetBy(Func<TDomainModel, bool> func);

        TDomainModel Update(TDomainModel model);

        TDomainModel Create(TDomainModel channelModel);

        void Delete(long id);

        void Delete(IEnumerable<long> ids);
    }
}