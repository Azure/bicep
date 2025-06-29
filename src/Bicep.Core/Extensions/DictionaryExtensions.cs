// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue? TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, TKey key) where TValue : class
        {
            source.TryGetValue(key, out TValue? value);
            return value;
        }

        /// <remarks>
        /// NOT thread safe!
        /// If you need thread safety, use a <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}"/>.
        /// </remarks>
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            TKey key,
            Func<TKey, TValue> valueFactory)
        {
            if (!source.TryGetValue(key, out var value))
            {
                value = valueFactory(key);
                source.Add(key, value);
            }

            return value;
        }
    }
}

