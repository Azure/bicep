// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public class ResourceType : TypeSymbol, IScopeReference
    {
        public ResourceType(ResourceTypeReference typeReference, ResourceScope validParentScopes, ITypeReference body)
            : base(typeReference.FormatName())
        {
            TypeReference = typeReference;
            ValidParentScopes = validParentScopes;
            Body = body;
        }

        public override TypeKind TypeKind => TypeKind.Resource;

        public ResourceTypeReference TypeReference { get; }

        /// <summary>
        /// Represents the possible scopes that this resource type can be deployed at.
        /// Does not account for cross-scope deployment limitations.
        /// </summary>
        public ResourceScope ValidParentScopes { get; }

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
