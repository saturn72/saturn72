#region

using System;

#endregion

namespace Saturn72.Core.Caching
{
    public static class CacheManagerExtensions
    {
        public const int DefaultCacheTime = 60;

        /// <summary>
        ///     Add datato cache
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="key">Cahce key</param>
        /// <param name="value">cached data</param>
        public static void Set<TCachedObject>(this ICacheManager cacheManager, string key, TCachedObject value)
        {
            cacheManager.Set(key, value, DefaultCacheTime);
        }

        /// <summary>
        ///     Addsite to cache only if not already exists
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void SetIfNotExists<TCachedObject>(this ICacheManager cacheManager, string key, TCachedObject value)
        {
            SetIfNotExists(cacheManager, key, value, DefaultCacheTime);
        }

        /// <summary>
        ///     Addsite to cache only if not already exists
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTimeInSeconds"></param>
        public static void SetIfNotExists<TCachedObject>(this ICacheManager cacheManager, string key, TCachedObject value, uint cacheTimeInSeconds)
        {
            if (cacheManager.Get<TCachedObject>(key) != null)
                return;
            cacheManager.Set(key, value, cacheTimeInSeconds);
        }

        /// <summary>
        ///     Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="TCachedObject">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <param name="cacheTimeInSeconds">Cacing time in minutes</param>
        /// <returns>Cached item</returns>
        public static TCachedObject Get<TCachedObject>(this ICacheManager cacheManager, string key, Func<TCachedObject> acquire, uint cacheTimeInSeconds)
        {
            var cachedObject = cacheManager.Get<TCachedObject>(key);
            if (cachedObject != null)
                return cachedObject;
            cachedObject = acquire();
            if (cachedObject == null)
                return default(TCachedObject);

            cacheManager.Set(key, cachedObject, cacheTimeInSeconds);
            return cachedObject;
        }


        /// <summary>
        ///     Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="TCachedObject">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static TCachedObject Get<TCachedObject>(this ICacheManager cacheManager, string key, Func<TCachedObject> acquire)
        {
            return Get(cacheManager,key, acquire, DefaultCacheTime);
        }
    }
}