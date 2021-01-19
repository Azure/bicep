// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class ModuleType : TypeSymbol, IScopeReference
    {
        public ModuleType(string name, ITypeReference body)
            : base(name)
        {
            Body = body;
        }

        public override TypeKind TypeKind => TypeKind.Module;

        public ITypeReference Body { get; }

        public ResourceScope Scope => ResourceScope.Module;
    }
}