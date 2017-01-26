#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

#endregion

namespace Saturn72.Core.Caching
{
    public class MemoryCacheManager : ICacheManager
    {
        /// <summary>
        ///     Cache object
        /// </summary>
        protected ObjectCache Cache
        {
            get { return MemoryCache.Default; }
        }

        /// <summary>
        ///     Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public virtual T Get<T>(string key)
        {
            return (T) Cache[key];
        }

        /// <summary>
        ///     Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            Cache.Set(new CacheItem(key, data), policy);

            //if (Cache.Contains(key))
            //    Cache.Set(new CacheItem(key, data), policy);
            //else Cache.Add(new CacheItem(key, data), policy);
        }

        public IEnumerable<string> Keys => Cache.Select(p => p.Key);

        /// <summary>
        ///     Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        ///     Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}