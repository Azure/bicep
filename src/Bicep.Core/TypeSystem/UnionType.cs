// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class UnionType : TypeSymbol
    {
        public UnionType(string name, ImmutableArray<ITypeReference> members)
            : base(name)
        {
            this.Members = members;
        }

        public override TypeKind TypeKind => this.Members.Any() ? TypeKind.Union : TypeKind.Never;

        public ImmutableArray<ITypeReference> Members { get; }

        public override string FormatNameForCompoundTypes() => TypeKind == TypeKind.Never ? Name : WrapTypeName();
    }
}

