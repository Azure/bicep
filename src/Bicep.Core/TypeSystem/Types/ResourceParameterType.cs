// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types
{
    public class ResourceParameterType(NamespaceType declaringNamespace, ResourceTypeReference typeReference) : TypeSymbol(typeReference.FormatType())
    {
        public NamespaceType DeclaringNamespace { get; } = declaringNamespace;

        public ResourceTypeReference TypeReference { get; } = typeReference;

        public override TypeKind TypeKind => TypeKind.Resource;
    }
}
