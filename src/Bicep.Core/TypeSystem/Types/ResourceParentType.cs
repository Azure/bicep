// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types
{
    public class ResourceParentType(ResourceTypeReference childTypeReference) : TypeSymbol(GetFullyQualifiedParentTypeName(childTypeReference))
    {
        public ResourceTypeReference ChildTypeReference { get; } = childTypeReference;

        public override TypeKind TypeKind => TypeKind.Resource;

        private static string GetFullyQualifiedParentTypeName(ResourceTypeReference childTypeReference) =>
            childTypeReference.TypeSegments.Take(childTypeReference.TypeSegments.Length - 1).ConcatString("/");
    }
}
