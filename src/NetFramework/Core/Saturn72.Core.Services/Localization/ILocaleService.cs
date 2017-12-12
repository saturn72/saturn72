using System.Collections.Generic;

namespace Saturn72.Core.Services.Localization
{
    public interface ILocaleService
    {
        string GetLocaleResource(string resourceKey, int languageId, string defaultValue = "",
            bool returnNullOnNotFound = false);

        Dictionary<string, KeyValuePair<long, string>> GetAllLocaleResources(int languageId);
    }
}