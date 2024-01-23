// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types
{
    public class ResourceParentType : TypeSymbol
    {
        public ResourceParentType(ResourceTypeReference childTypeReference)
            : base(GetFullyQualifiedParentTypeName(childTypeReference))
        {
            ChildTypeReference = childTypeReference;
        }

        public ResourceTypeReference ChildTypeReference { get; }

        public override TypeKind TypeKind => TypeKind.Resource;

        private static string GetFullyQualifiedParentTypeName(ResourceTypeReference childTypeReference) =>
            childTypeReference.TypeSegments.Take(childTypeReference.TypeSegments.Length - 1).ConcatString("/");
    }
}
