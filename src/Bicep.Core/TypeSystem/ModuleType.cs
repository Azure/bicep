// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class ModuleType : TypeSymbol, IScopeReference
    {
        public ModuleType(string name, ResourceScope validParentScopes, ITypeReference body)
            : base(name)
        {
            ValidParentScopes = validParentScopes;
            Body = body;
        }

        public override TypeKind TypeKind => TypeKind.Module;

        public ResourceScope ValidParentScopes { get; }

        public ITypeReference Body { get; }

        public ResourceScope Scope => ResourceScope.Module;
    }
}