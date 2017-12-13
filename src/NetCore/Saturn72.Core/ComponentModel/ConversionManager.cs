using System;
using System.ComponentModel;
using Saturn72.Core.Caching;
using Saturn72.Extensions;

namespace Saturn72.Core.ComponentModel
{
    public class ConversionManager
    {
        private readonly ICacheManager _cacheManager;

        public ConversionManager(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public Converter Get<TConvertToType>()
        {
            return Get(typeof(TConvertToType));
        }

        public Converter Get(Type convertToType)
        {
            var cacheKey = CacheKeys.ConverterByType.AsFormat(convertToType.ToString());

            return _cacheManager.Get(cacheKey,
                () => new Converter(TypeDescriptor.GetConverter(convertToType)), CacheKeys.ConverterCacheTime);
        }
    }
}
