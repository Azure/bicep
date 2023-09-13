// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Bicep.LanguageServer.Completions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (TSource item in source)
            {
                action(item);
            }

            return source;
        }
    }
}
