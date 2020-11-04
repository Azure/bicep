// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class ResourceScopeReference : TypeSymbol
    {
        public ResourceScopeReference(string name, ResourceScopeType resourceScopeType)
            : base(name)
        {
            ResourceScopeType = resourceScopeType;
        }

        public override TypeKind TypeKind => TypeKind.ResourceScopeReference;

        public ResourceScopeType ResourceScopeType { get; }
    }
}
