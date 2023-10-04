// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public class EmptyResourceTypeProvider : IResourceTypeProvider
    {
        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => Enumerable.Empty<ResourceTypeReference>();

        public ResourceType? TryGetDefinedType(NamespaceType declaringNamespace, ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
            => null;

        public ResourceType? TryGenerateFallbackType(NamespaceType declaringNamespace, ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
            => null;

        public bool HasDefinedType(ResourceTypeReference typeReference)
            => false;

        public ImmutableDictionary<string, ImmutableArray<ResourceTypeReference>> TypeReferencesByType
            => ImmutableDictionary<string, ImmutableArray<ResourceTypeReference>>.Empty;

        public string Version { get; } = "1.0.0";
    }
}
