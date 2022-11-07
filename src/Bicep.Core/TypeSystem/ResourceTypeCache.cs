// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Resources;
using System.Collections.Concurrent;

namespace Bicep.Core.TypeSystem
{
    public class ResourceTypeCache
    {
        private record CacheKey(
            ResourceTypeGenerationFlags Flags,
            ResourceTypeReference Type);

        private readonly ConcurrentDictionary<CacheKey, ResourceTypeComponents> cache = new();

        public ResourceTypeComponents GetOrAdd(ResourceTypeGenerationFlags flags, ResourceTypeReference typeReference, Func<ResourceTypeComponents> buildFunc)
        {
            var cacheKey = new CacheKey(flags, typeReference);

            return cache.GetOrAdd(cacheKey, cacheKey => buildFunc());
        }
    }
}
