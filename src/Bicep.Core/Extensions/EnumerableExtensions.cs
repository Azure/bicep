// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ConcatString(this IEnumerable<string> source, string separator) => string.Join(separator, source);

        public static string ConcatString(this IEnumerable<char> source, string separator) => source.Select(char.ToString).ConcatString(separator);

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, params TSource[] second)
            => first.Concat((IEnumerable<TSource>)second);

        public static IEnumerable<IGrouping<TKey, TSource>> GroupByExcludingNull<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey?> keySelector, IEqualityComparer<TKey> comparer)
            where TKey : class
            => source.Where(x => keySelector(x) != null).GroupBy(x => keySelector(x)!, comparer);

        public static ImmutableDictionary<TKey, TSource> ToImmutableDictionaryExcludingNull<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey?> keySelector, IEqualityComparer<TKey> keyComparer)
            where TKey : class
            => source.Where(x => keySelector(x) != null).ToImmutableDictionary(x => keySelector(x)!, keyComparer);

        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionaryExcludingNull<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey?> keySelector, Func<TSource, TValue> elementSelector, IEqualityComparer<TKey> keyComparer)
            where TKey : class
            => source.Where(x => keySelector(x) != null).ToImmutableDictionary(x => keySelector(x)!, elementSelector, keyComparer);

        public static ImmutableDictionary<TKey, TSource> ToImmutableDictionaryExcludingNullValues<TSource, TKey>(this IEnumerable<TSource?> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
            where TKey : notnull
            where TSource : class
            => source.WhereNotNull().ToImmutableDictionary(x => keySelector(x), x => x, keyComparer);

        public static ImmutableHashSet<TSource> ToImmutableHashSetExcludingNull<TSource>(this IEnumerable<TSource?> source, IEqualityComparer<TSource> comparer)
            where TSource : class
            => source.WhereNotNull().ToImmutableHashSet(comparer);
            
        public static IEnumerable<T> AsEnumerable<T>(this T single)
        {
            yield return single;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> range)
        {
            foreach (var element in range)
            {
                list.Add(element);
            }
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
            where T : class
        {
            foreach (var item in source)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        public static ILookup<T, T> InvertLookup<T>(this ILookup<T, T> source)
            => source.SelectMany(group => group.Select(val => (group.Key, val)))
                .ToLookup(x => x.val, x => x.Key);

        public static ImmutableDictionary<TKey, ImmutableHashSet<TValue>> ToImmutableDictionary<TKey, TValue>(this ILookup<TKey, TValue> source)
            where TKey : notnull
            => source.ToImmutableDictionary(x => x.Key, x => x.ToImmutableHashSet());
    }
}

