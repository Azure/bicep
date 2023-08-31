// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem
{
    public record ResourceTypeComponents(
        ResourceTypeReference TypeReference,
        ResourceScope ValidParentScopes,
        ResourceScope ReadOnlyScopes,
        ResourceFlags Flags,
        ITypeReference Body);

    public class ResourceType : TypeSymbol, IScopeReference
    {
        public ResourceType(
            NamespaceType declaringNamespace,
            ResourceTypeReference typeReference,
            ResourceScope validParentScopes,
            ResourceScope readOnlyScopes,
            ResourceFlags flags,
            ITypeReference body,
            ImmutableHashSet<string> uniqueIdentifierProperties
        ) : base(typeReference.FormatName())
        {
            DeclaringNamespace = declaringNamespace;
            TypeReference = typeReference;
            ValidParentScopes = validParentScopes;
            ReadOnlyScopes = readOnlyScopes;
            Flags = flags;
            Body = body;
            UniqueIdentifierProperties = uniqueIdentifierProperties;
        }

        public override TypeKind TypeKind => TypeKind.Resource;

        public NamespaceType DeclaringNamespace { get; }

        public ResourceTypeReference TypeReference { get; }

        public ImmutableHashSet<string> UniqueIdentifierProperties { get; }

        /// <summary>
        /// Represents the possible scopes that this resource type can be deployed at.
        /// Does not account for cross-scope deployment limitations.
        /// </summary>
        public ResourceScope ValidParentScopes { get; }

        /// <summary>
        /// Represents the scopes in which this resource type may only be used with the `existing` keyword.
        /// </summary>
        public ResourceScope ReadOnlyScopes { get; }

        public ResourceFlags Flags { get; }

        public ITypeReference Body { get; }

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
