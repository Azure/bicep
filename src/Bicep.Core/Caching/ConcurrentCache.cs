// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Caching
{
    public class ConcurrentCache<TKey, TValue> : IConcurrentCache<TKey, TValue>
        where TKey : notnull
    {
        private readonly ConcurrentDictionary<TKey, TValue> items = new();

        public FrozenSet<TKey> Keys => items.Keys.ToFrozenSet();

        public TValue GetOrAdd(TKey key, Func<TValue> valueFactory) => items.GetOrAdd(key, _ => valueFactory());

        public TValue? TryRemove(TKey key) => items.TryRemove(key, out var value) ? value : default;

        public IEnumerable<TValue> Trim(IEnumerable<TKey> keys)
        {
            var removedValues = new List<TValue>();

            foreach (var key in keys)
            {
                if (items.TryRemove(key, out var value))
                {
                    removedValues.Add(value);
                }
            }

            return removedValues;
        }
    }
}
