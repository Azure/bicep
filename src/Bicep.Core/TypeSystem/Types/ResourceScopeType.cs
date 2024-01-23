// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types
{
    public class ResourceScopeType : TypeSymbol, IScopeReference
    {
        public ResourceScopeType(string name, ResourceScope scopeType)
            : base(name)
        {
            Scope = scopeType;
        }

        public override TypeKind TypeKind => TypeKind.ResourceScopeReference;

        public ResourceScope Scope { get; }

        public override string FormatNameForCompoundTypes() =>
            Enum.IsDefined(typeof(ResourceScope), Scope) ? Name : WrapTypeName();
    }
}
