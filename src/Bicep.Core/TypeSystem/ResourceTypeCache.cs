// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Resources;
using System.Collections.Concurrent;

namespace Bicep.Core.TypeSystem
{
    public class ResourceTypeCache
    {
        private class KeyComparer : IEqualityComparer<(ResourceTypeGenerationFlags flags, ResourceTypeReference type)>
        {
            public static IEqualityComparer<(ResourceTypeGenerationFlags flags, ResourceTypeReference type)> Instance { get; }
                = new KeyComparer();

            public bool Equals((ResourceTypeGenerationFlags flags, ResourceTypeReference type) x, (ResourceTypeGenerationFlags flags, ResourceTypeReference type) y)
                => x.flags == y.flags &&
                    ResourceTypeReferenceComparer.Instance.Equals(x.type, y.type);

            public int GetHashCode((ResourceTypeGenerationFlags flags, ResourceTypeReference type) x)
                => x.flags.GetHashCode() ^
                    ResourceTypeReferenceComparer.Instance.GetHashCode(x.type);
        }

        private readonly ConcurrentDictionary<(ResourceTypeGenerationFlags flags, ResourceTypeReference type), ResourceTypeComponents> cache
            = new ConcurrentDictionary<(ResourceTypeGenerationFlags flags, ResourceTypeReference type), ResourceTypeComponents>(KeyComparer.Instance);

        public ResourceTypeComponents GetOrAdd(ResourceTypeGenerationFlags flags, ResourceTypeReference typeReference, Func<ResourceTypeComponents> buildFunc)
        {
            var cacheKey = (flags, typeReference);

            return cache.GetOrAdd(cacheKey, cacheKey => buildFunc());
        }
    }
}
