// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers
{
    public interface IResourceTypeProvider
    {
        /// <summary>
        /// Tries to get a resource type from the set of well known resource types. Returns null if none is available.
        /// </summary>
        ResourceType? TryGetDefinedType(NamespaceType declaringNamespace, ResourceTypeReference reference, ResourceTypeGenerationFlags flags);

        /// <summary>
        /// Tries to generate a fallback resource type definition, if possible. Returns null if this is not possible.
        /// </summary>
        ResourceType? TryGenerateFallbackType(NamespaceType declaringNamespace, ResourceTypeReference reference, ResourceTypeGenerationFlags flags);

        /// <summary>
        /// Checks whether the type exists in the set of well known resource types.
        /// </summary>
        bool HasDefinedType(ResourceTypeReference typeReference);

        /// <summary>
        /// Returns the full list of available types defined by this provider.
        /// </summary>
        IEnumerable<ResourceTypeReference> GetAvailableTypes();

        ImmutableDictionary<string, ImmutableArray<ResourceTypeReference>> TypeReferencesByType { get; }
    }
}
