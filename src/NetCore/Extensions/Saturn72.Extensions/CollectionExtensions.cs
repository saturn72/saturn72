#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Saturn72.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        ///     Adds it to collection if not exists
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        public static void AddIfNotExist<T>(this ICollection<T> source, T toAdd)
        {
            Func<T, bool> searchCriteria = t => t.Equals(toAdd);
            AddIfNotExist(source, toAdd, searchCriteria);
        }

        /// <summary>
        ///     Adds ite to collection if not exists
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="source">the collection to add item to </param>
        /// <param name="toAdd">item to add</param>
        /// <param name="searchCriteria">predicate to determine if item is in collection</param>
        public static void AddIfNotExist<T>(this ICollection<T> source, T toAdd, Func<T, bool> searchCriteria)
        {
            Guard.NotNull(source);

            if (toAdd == null || source.Any(searchCriteria))
                return;
            source.Add(toAdd);
        }

        /// <summary>
        ///     Adds or overwrites collection's item
        /// </summary>
        /// <typeparam name="T">Item generic type.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <param name="toAdd">Item to add</param>
        /// <param name="searchCriteria">Search criteria for item</param>
        public static void AddOrOverwrite<T>(this ICollection<T> source, T toAdd, Func<T, bool> searchCriteria)
        {
            var collectionItem = source.FirstOrDefault(searchCriteria);
            if (collectionItem != null)
                source.Remove(collectionItem);

            source.Add(toAdd);
        }
    }
}