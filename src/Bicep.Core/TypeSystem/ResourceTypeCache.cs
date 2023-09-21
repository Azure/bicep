// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public class ResourceTypeCache
    {
        private record struct CacheKey(
            ResourceTypeGenerationFlags Flags,
            ResourceTypeReference Type);

        private readonly ConcurrentDictionary<CacheKey, ResourceTypeComponents> cache = new();

        public ResourceTypeComponents GetOrAdd(ResourceTypeGenerationFlags flags, ResourceTypeReference typeReference, Func<ResourceTypeComponents> buildFunc)
            => cache.GetOrAdd(new(flags, typeReference), _ => buildFunc());
    }
}
