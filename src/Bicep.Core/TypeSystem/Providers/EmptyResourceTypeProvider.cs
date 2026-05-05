// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers
{
    public class EmptyResourceTypeProvider : IResourceTypeProvider
    {
        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => [];

        public ResourceType? TryGetDefinedType(NamespaceType declaringNamespace, ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
            => null;

        public ResourceType? TryGenerateFallbackType(NamespaceType declaringNamespace, ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
            => null;

        public bool HasDefinedType(ResourceTypeReference typeReference)
            => false;

        public ImmutableDictionary<string, ImmutableArray<ResourceTypeReference>> TypeReferencesByType
            => [];

        public string Version { get; } = "1.0.0";
    }
}
