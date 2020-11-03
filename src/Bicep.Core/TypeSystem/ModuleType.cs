// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;

namespace Bicep.Core.TypeSystem
{
    public class ModuleType : TypeSymbol, IResourceScopeType
    {
        public ModuleType(string name, ITypeReference body)
            : base(name)
        {
            Body = body;
        }

        public override TypeKind TypeKind => TypeKind.Resource;

        public ITypeReference Body { get; }

        public ResourceScopeType ResourceScopeType => ResourceScopeType.ModuleScope;
    }
}