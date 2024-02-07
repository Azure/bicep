// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types
{
    public record ResourceTypeComponents(
        ResourceTypeReference TypeReference,
        ResourceScope ValidParentScopes,
        ResourceScope ReadOnlyScopes,
        ResourceFlags Flags,
        ITypeReference Body);

    public class ResourceType(
        NamespaceType declaringNamespace,
        ResourceTypeReference typeReference,
        ResourceScope validParentScopes,
        ResourceScope readOnlyScopes,
        ResourceFlags flags,
        ITypeReference body,
        ImmutableHashSet<string> uniqueIdentifierProperties
        ) : TypeSymbol(typeReference.FormatName()), IScopeReference
    {
        public override TypeKind TypeKind => TypeKind.Resource;

        public NamespaceType DeclaringNamespace { get; } = declaringNamespace;

        public ResourceTypeReference TypeReference { get; } = typeReference;

        public ImmutableHashSet<string> UniqueIdentifierProperties { get; } = uniqueIdentifierProperties;

        /// <summary>
        /// Represents the possible scopes that this resource type can be deployed at.
        /// Does not account for cross-scope deployment limitations.
        /// </summary>
        public ResourceScope ValidParentScopes { get; } = validParentScopes;

        /// <summary>
        /// Represents the scopes in which this resource type may only be used with the `existing` keyword.
        /// </summary>
        public ResourceScope ReadOnlyScopes { get; } = readOnlyScopes;

        public ResourceFlags Flags { get; } = flags;

        public ITypeReference Body { get; } = body;

        public ResourceScope Scope => ResourceScope.Resource;

        public static ResourceType? TryUnwrap(TypeSymbol typeSymbol)
            => typeSymbol switch
            {
                ResourceType resourceType => resourceType,
                ArrayType { Item: ResourceType resourceType } => resourceType,
                _ => null
            };
    }
}
