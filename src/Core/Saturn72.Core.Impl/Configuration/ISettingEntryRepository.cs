#region

using System.Collections.Generic;
using Saturn72.Core.Data;
using Saturn72.Core.Domain.Configuration;

#endregion

namespace Saturn72.Core.Services.Impl.Configuration
{
    public interface ISettingEntryRepository:IRepository<SettingEntryDomainModel, long>
    {
        void  Delete(IEnumerable<SettingEntryDomainModel> settings);
    }
}