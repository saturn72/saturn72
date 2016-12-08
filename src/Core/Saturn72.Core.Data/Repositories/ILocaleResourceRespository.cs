using System.Collections.Generic;
using Saturn72.Core.Domain.Localization;

namespace Saturn72.Core.Data.Repositories
{
    public  interface ILocaleResourceRespository
    {
        LocaleResourceDomainModel GetLocaleResource(string key, int languageId);
        IEnumerable<LocaleResourceDomainModel> GetAllLocaleResources(int languageId);
    }
}