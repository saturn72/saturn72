using System;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Localization
{
    public static class LocaleServiceExtensions
    {
        public static string GetLocaleResource(this ILocaleService localeService, string resourceKey)
        {
            var languageId = 0;
            //get language ID from user context/defaults
            return localeService.GetLocaleResource(resourceKey, languageId);
        }
    }
}