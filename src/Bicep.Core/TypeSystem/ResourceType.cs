// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public class ResourceType : TypeSymbol, IScopeReference
    {
        public ResourceType(ResourceTypeReference typeReference, ITypeReference body)
            : base(typeReference.FormatName())
        {
            TypeReference = typeReference;
            Body = body;
        }

        public override TypeKind TypeKind => TypeKind.Resource;

        public ResourceTypeReference TypeReference { get; }

        public ITypeReference Body { get; }

        public ResourceScope Scope => ResourceScope.Resource;
    }
}
