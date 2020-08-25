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

        public static IEnumerable<T> AsEnumerable<T>(this T single)
        {
            yield return single;
        }
    }
}

