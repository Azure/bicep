// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;

namespace Bicep.Core.TypeSystem
{
    public class UnionType : TypeSymbol
    {
        private UnionType(string name, ImmutableArray<ITypeReference> members)
            : base(name)
        {
            this.Members = members;
        }

        public override TypeKind TypeKind => this.Members.Any() ? TypeKind.Union : TypeKind.Never;

        public ImmutableArray<ITypeReference> Members { get; }

        public static TypeSymbol Create(IEnumerable<ITypeReference> unionMembers)
        {
            // flatten and then de-duplicate members
            var finalMembers = FlattenMembers(unionMembers)
                .Distinct()
                .OrderBy(m => m.Type.Name, StringComparer.Ordinal)
                .ToImmutableArray();

            return finalMembers.Length switch
            {
                0 => new UnionType("never", ImmutableArray<ITypeReference>.Empty),
                1 => finalMembers[0].Type,
                _ => new UnionType(FormatName(finalMembers), finalMembers)
            };
        }

        public static TypeSymbol Create(params ITypeReference[] members) => Create((IEnumerable<ITypeReference>) members);

        public override string FormatNameForCompoundTypes() => this.WrapTypeName();

        private static IEnumerable<ITypeReference> FlattenMembers(IEnumerable<ITypeReference> members) => 
            members.SelectMany(member => member.Type is UnionType union 
                ? FlattenMembers(union.Members)
                : member.AsEnumerable());

        private static string FormatName(IEnumerable<ITypeReference> unionMembers) => 
            unionMembers.Select(m => m.Type.FormatNameForCompoundTypes()).ConcatString(" | ");
    }
}

