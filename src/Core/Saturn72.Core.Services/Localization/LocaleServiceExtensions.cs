using System.Diagnostics;
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

        private const string LocaleResourceFormat = "{0}.{1}.{2}";
        public static string GetLocaleResourceByCallerMethod(this ILocaleService localeService, string resourceKeySuffix)
        {
            var method = localeService.GetStackTraceFrame(2).GetMethod();
            var resourceKey  = LocaleResourceFormat.AsFormat(method.DeclaringType.FullName,
              method.Name, resourceKeySuffix);

            var languageId = 0;
            return localeService.GetLocaleResource(resourceKey, languageId);
        }
    }
}