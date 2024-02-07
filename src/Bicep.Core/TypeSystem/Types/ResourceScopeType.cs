// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types
{
    public class ResourceScopeType(string name, ResourceScope scopeType) : TypeSymbol(name), IScopeReference
    {
        public override TypeKind TypeKind => TypeKind.ResourceScopeReference;

        public ResourceScope Scope { get; } = scopeType;

        public override string FormatNameForCompoundTypes() =>
            Enum.IsDefined(typeof(ResourceScope), Scope) ? Name : WrapTypeName();
    }
}
