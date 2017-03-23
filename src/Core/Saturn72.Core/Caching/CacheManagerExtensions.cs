#region

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#endregion

namespace Saturn72.Core.Caching
{
    public static class CacheManagerExtensions
    {
        private const int DefaultCacheTime = 60;

        public static void Set(this ICacheManager cacheManager, string key, object data)
        {
            cacheManager.Set(key, data, DefaultCacheTime);
        }
        /// <summary>
        ///     Addsite to cache only if not already exists
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void SetIfNotExists(this ICacheManager cacheManager, string key, object data)
        {
            if (cacheManager.IsSet(key))
                return;
            cacheManager.Set(key, data);
        }

        /// <summary>
        ///     Addsite to cache only if not already exists
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTimeInMinutes"></param>
        public static void SetIfNotExists(this ICacheManager cacheManager, string key, object data,
            int cacheTimeInMinutes)
        {
            if (cacheManager.IsSet(key))
                return;
            cacheManager.Set(key, data, cacheTimeInMinutes);
        }

        /// <summary>
        ///     Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, DefaultCacheTime, acquire);
        }

        /// <summary>
        ///     Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static Task<T> Get<T>(this ICacheManager cacheManager, string key, Func<Task<T>> acquire)
        {
            return Get(cacheManager, key, DefaultCacheTime, acquire);
        }

        /// <summary>
        ///     Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="cacheTime">Cache time in minutes (0 - do not cache)</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static async Task<T> Get<T>(this ICacheManager cacheManager, string key, int cacheTime,
            Func<Task<T>> acquire)
        {
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }
            var value = await acquire();
            cacheManager.Set(key, value, DefaultCacheTime);
            return value;
        }

        /// <summary>
        ///     Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="cacheTime">Cache time in minutes (0 - do not cache)</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }

            var result = acquire();
            if (cacheTime > 0)
                cacheManager.Set(key, result, cacheTime);
            return result;
        }

        /// <summary>
        ///     Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public static bool IsSet(this ICacheManager cacheManager, string key)
        {
            return cacheManager.Keys.Contains(key);
        }

        /// <summary>
        ///     Removes items by pattern
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="pattern">Pattern</param>
        /// <param name="keys">All keys in the cache</param>
        public static void RemoveByPattern(this ICacheManager cacheManager, string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (var key in cacheManager.Keys.Where(p => regex.IsMatch(p.ToString())).ToList())
                cacheManager.Remove(key);
        }
    }
}