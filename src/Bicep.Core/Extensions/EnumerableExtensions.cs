// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ConcatString(this IEnumerable<string> source, string separator) => string.Join(separator, source);

        public static string ConcatString(this IEnumerable<char> source, string separator) => source.Select(char.ToString).ConcatString(separator);

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, params TSource[] second)
            => first.Concat((IEnumerable<TSource>)second);

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
    }
}

