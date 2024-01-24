// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers
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
