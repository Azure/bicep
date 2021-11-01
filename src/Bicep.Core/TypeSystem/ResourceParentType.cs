// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public class ResourceParentType : TypeSymbol
    {
        public ResourceParentType(ResourceTypeReference childTypeReference)
            : base(GetFullyQualifiedParentTypeName(childTypeReference))
        {
            this.ChildTypeReference = childTypeReference;
        }

        public ResourceTypeReference ChildTypeReference { get; }

        public override TypeKind TypeKind => TypeKind.Resource;

        private static string GetFullyQualifiedParentTypeName(ResourceTypeReference childTypeReference) =>
            childTypeReference.TypeSegments.Take(childTypeReference.TypeSegments.Length - 1).ConcatString("/");
    }
}
