#region

using System.Collections.Generic;

#endregion

namespace Saturn72.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            return GetValueOrDefault(source, key, default(TValue));
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key,
            TValue defaultValue)
        {
            TValue tmp;
            return source.TryGetValue(key, out tmp)
                ? tmp
                : defaultValue;
        }

        public static TValue GetValueOrSet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key,
            TValue setValue)
        {
            TValue tmp;
            if (source.TryGetValue(key, out tmp))
                return tmp;
            source[key] = setValue;
            return setValue;
        }
    }
}