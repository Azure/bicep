// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue? TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, TKey key) where TValue : class
        {
            source.TryGetValue(key, out TValue value);
            return value;
        }
    }
}

