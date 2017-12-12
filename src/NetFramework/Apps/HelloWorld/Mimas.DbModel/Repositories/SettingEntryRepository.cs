#region

using System.Collections.Generic;
using Saturn72.Common.Data.Repositories;
using Saturn72.Core.Domain.Configuration;
using Saturn72.Core.Services.Data.Repositories;

#endregion

namespace Mimas.DbModel.Repositories
{
    public class SettingEntryRepository : RepositoryBase<SettingEntryDomainModel, long, SettingEntry>,
        ISettingEntryRepository
    {
        public SettingEntryRepository(IUnitOfWork<long> unitOfWork) : base(unitOfWork)
        {
        }
    }
}