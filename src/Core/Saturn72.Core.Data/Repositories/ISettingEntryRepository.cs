#region

using System.Collections.Generic;
using Saturn72.Core.Domain.Configuration;

#endregion

namespace Saturn72.Core.Data.Repositories
{
    public interface ISettingEntryRepository:IRepository<SettingEntryDomainModel, long>
    {
        void  Delete(IEnumerable<SettingEntryDomainModel> settings);
    }
}