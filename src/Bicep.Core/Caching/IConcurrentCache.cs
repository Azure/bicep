// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Caching
{
    public interface IConcurrentCache<TKey, TValue>
    {
        FrozenSet<TKey> Keys { get; }

        TValue GetOrAdd(TKey key, Func<TValue> valueFactory);

        TValue? TryRemove(TKey key);

        IEnumerable<TValue> Trim(IEnumerable<TKey> keys);
    }
}
