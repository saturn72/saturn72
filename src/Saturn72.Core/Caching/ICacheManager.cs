namespace Saturn72.Core.Caching
{
    public interface ICacheManager
    {
        /// <summary>
        ///     Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="TCachedObject">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        TCachedObject Get<TCachedObject>(string key);

        /// <summary>
        ///     Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        void Set<TCachedObject>(string key, TCachedObject data, int cacheTime);

        void RemoveByPattern(string pattern);

        /// <summary>
        ///     Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        /// <summary>
        ///     Clear all cache data
        /// </summary>
        void Clear();
    }
}