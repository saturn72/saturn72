using System;
using System.Collections.Generic;
using Saturn72.Core.Caching;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Localization;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.Localization
{
    public class LocaleService : ILocaleService
    {
        private const string AllLocaleRsourcesLanguageKey = "Saturn72.all.lr-lang_{0}";
        private readonly ICacheManager _cacheManager;

        private readonly ILocaleResourceRespository _localeResourceRespository;
        private readonly ILogger _logger;

        public LocaleService(ILocaleResourceRespository localeResourceRespository, ICacheManager cacheManager,
            ILogger logger)
        {
            _localeResourceRespository = localeResourceRespository;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public virtual string GetLocaleResource(string resourceKey, int languageId, string defaultValue = "", bool returnNullOnNotFound = false)
        {
            if (!resourceKey.HasValue())
                return returnNullOnNotFound ?  null :defaultValue;

            resourceKey = resourceKey.Trim().ToLowerInvariant();

            KeyValuePair<long, string> lr;

            if (GetAllLocaleResources(languageId).TryGetValue(resourceKey, out lr))
                return lr.Value;

                _logger.Warning("Resource string ({0}) is not found. Language ID = {1}".AsFormat(resourceKey, languageId));
            return returnNullOnNotFound ? null : defaultValue;
        }

        public virtual Dictionary<string, KeyValuePair<long, string>> GetAllLocaleResources(int languageId)
        {
            var cacheKey = AllLocaleRsourcesLanguageKey.AsFormat(languageId);
            return _cacheManager.Get(cacheKey, () =>
            {
                var locales = _localeResourceRespository.GetAllLocaleResources(languageId);

                //format: <name, <id, value>>
                var dictionary = new Dictionary<string, KeyValuePair<long, string>>();
                foreach (var locale in locales)
                {
                    var lrKey = locale.Key.ToLowerInvariant();
                    if (!dictionary.ContainsKey(lrKey))
                        dictionary.Add(lrKey, new KeyValuePair<long, string>(locale.Id, locale.Value));
                }
                return dictionary;
            });
        }
    }
}