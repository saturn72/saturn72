#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Saturn72.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEachItem<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var s in source)
                action(s);
        }

        public static bool IsEmptyOrNull(this IEnumerable source)
        {
            return source == null || !source.GetEnumerator().MoveNext();
        }

        public static bool NotEmptyOrNull(this IEnumerable source)
        {
            return !IsEmptyOrNull(source);
        }

        public static T Random<T>(this IEnumerable<T> source)
        {
            var seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
            var randomIndex = new Random(seed).Next(source.Count());

            return source.ElementAtOrDefault(randomIndex);
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !(!source.IsNull() && source.Any());
        }

        public static bool IsIEnumerableofType(this Type type)
        {
            var genArgs = type.GetGenericArguments();
            if (genArgs.Length == 1 && typeof(IEnumerable<>).MakeGenericType(genArgs).IsAssignableFrom(type))
                return true;
            return type.BaseType != null && IsIEnumerableofType(type.BaseType);
        }
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, params T[] second)
        {
            return !second.Any() ? first : first.Concat(second.ToList());
        }

        public static bool HasValues(this IEnumerable source)
        {
            return source.GetEnumerator().MoveNext();
        }

        /// <summary>
        ///     Gets the max IEnumerable object by internal property
        /// </summary>
        /// <typeparam name="TSource">Enumerable first object</typeparam>
        /// <typeparam name="TKey">Property type to set the maxby on</typeparam>
        /// <param name="source">IEnumerable</param>
        /// <param name="selector">Proerty selector</param>
        /// <returns>TSource</returns>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        ///     Gets the max IEnumerable object by internal property
        /// </summary>
        /// <typeparam name="TSource">enumerable first object</typeparam>
        /// <typeparam name="TKey">Property type to set the maxby on</typeparam>
        /// <param name="source">The IEnumerable to iterate on</param>
        /// <param name="selector">the Selector</param>
        /// <param name="comparer">Comparator</param>
        /// <returns>TSource</returns>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");


            var sourceIterator = source.GetEnumerator();
            if (!sourceIterator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            var max = sourceIterator.Current;
            var maxKey = selector(max);

            while (sourceIterator.MoveNext())
            {
                var candidate = sourceIterator.Current;
                var candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, maxKey) <= 0) continue;

                max = candidate;
                maxKey = candidateProjected;
            }
            sourceIterator.Dispose();

            return max;
        }
    }
}