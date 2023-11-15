// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types
{
    public class ResourceParameterType : TypeSymbol
    {
        public ResourceParameterType(NamespaceType declaringNamespace, ResourceTypeReference typeReference)
            : base(typeReference.FormatType())
        {
            DeclaringNamespace = declaringNamespace;
            TypeReference = typeReference;
        }

        public NamespaceType DeclaringNamespace { get; }

        public ResourceTypeReference TypeReference { get; }

        public override TypeKind TypeKind => TypeKind.Resource;
    }
}
