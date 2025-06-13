// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ConcatString(this IEnumerable<string> source, string separator) => string.Join(separator, source);

        public static string ConcatString(this IEnumerable<char> source, string separator) => source.Select(char.ToString).ConcatString(separator);

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, params TSource[] second)
            => first.Concat((IEnumerable<TSource>)second);

        public static IEnumerable<IGrouping<TKey, TSource>> GroupByExcludingNull<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey?> keySelector, IEqualityComparer<TKey>? comparer = null)
            where TKey : class
            => source.Where(x => keySelector(x) != null).GroupBy(x => keySelector(x)!, comparer);

        public static ImmutableDictionary<TKey, TSource> ToImmutableDictionaryExcludingNull<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey?> keySelector, IEqualityComparer<TKey>? keyComparer = null)
            where TKey : class
            => source.Where(x => keySelector(x) != null).ToImmutableDictionary(x => keySelector(x)!, keyComparer);

        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionaryExcludingNull<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey?> keySelector, Func<TSource, TValue> elementSelector, IEqualityComparer<TKey>? keyComparer = null)
            where TKey : class
            => source.Where(x => keySelector(x) != null).ToImmutableDictionary(x => keySelector(x)!, elementSelector, keyComparer);

        public static ImmutableDictionary<TKey, TSource> ToImmutableDictionaryExcludingNullValues<TSource, TKey>(this IEnumerable<TSource?> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? keyComparer = null)
            where TKey : notnull
            where TSource : class
            => source.WhereNotNull().ToImmutableDictionary(x => keySelector(x), x => x, keyComparer);

        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionaryExcludingNullValues<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue?> elementSelector, IEqualityComparer<TKey>? keyComparer = null)
            where TKey : notnull
            where TValue : class
            => source.Select(x => (key: keySelector(x), value: elementSelector(x))).Where(x => x.value != null).ToImmutableDictionary(
                kvp => kvp.key,
                kvp => kvp.value!,
                keyComparer);

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

        public static IEnumerable<T> TakeWhileNotNull<T>(this IEnumerable<T?> source)
            where T : class
        {
            return source.TakeWhile(x => x is not null)
                .Cast<T>();
        }

        public static T[] ToArrayExcludingNull<T>(this IEnumerable<T?> source)
            where T : class
            => [.. source.WhereNotNull()];

        public static ILookup<T, T> InvertLookup<T>(this ILookup<T, T> source)
            => source.SelectMany(group => group.Select(val => (group.Key, val)))
                .ToLookup(x => x.val, x => x.Key);

        public static ImmutableDictionary<TKey, ImmutableHashSet<TValue>> ToImmutableDictionary<TKey, TValue>(this ILookup<TKey, TValue> source)
            where TKey : notnull
            => source.ToImmutableDictionary(x => x.Key, x => x.ToImmutableHashSet());


        public static IEnumerable<T> EnumerateRecursively<T>(T element, Func<T, T?> getNextElement)
        {
            yield return element;

            var next = getNextElement(element);
            while (next is not null)
            {
                yield return next;

                next = getNextElement(next);
            }
        }

        // Enables this usage:
        // var (first) = new[] { 1, 2, 3 };
        public static void Deconstruct<T>(this IEnumerable<T> items, out T t1)
        {
            using var enumerator = items.GetEnumerator();
            t1 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Sequence contains no elements");
        }

        // Enables this usage:
        // var (first, second) = new[] { 1, 2, 3 };
        public static void Deconstruct<T>(this IEnumerable<T> items, out T t1, out T t2)
        {
            using var enumerator = items.GetEnumerator();
            t1 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Sequence contains no elements");
            t2 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Sequence contains no more elements");
        }

        // Enables this usage:
        // var (first, second, third) = new[] { 1, 2, 3 };
        public static void Deconstruct<T>(this IEnumerable<T> items, out T t1, out T t2, out T t3)
        {
            using var enumerator = items.GetEnumerator();
            t1 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Sequence contains no elements");
            t2 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Sequence contains no more elements");
            t3 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Sequence contains no more elements");
        }
    }
}

