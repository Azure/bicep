// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class ResourceReferenceType : TypeSymbol
    {
        public ResourceReferenceType(string name, ResourceScope resourceScopeType)
            : base(name)
        {
            ResourceScopeType = resourceScopeType;
        }

        public override TypeKind TypeKind => TypeKind.ResourceScopeReference;

        public ResourceScope ResourceScopeType { get; }
    }
}
