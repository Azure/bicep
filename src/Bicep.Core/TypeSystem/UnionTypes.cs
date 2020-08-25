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
        private UnionType(string name, ImmutableArray<TypeSymbol> members)
            : base(name)
        {
            this.Members = members;
        }

        public override TypeKind TypeKind => this.Members.Any() ? TypeKind.Union : TypeKind.Never;

        public ImmutableArray<TypeSymbol> Members { get; }

        public static TypeSymbol Create(IEnumerable<TypeSymbol> unionMembers)
        {
            // flatten and then de-duplicate members
            var finalMembers = FlattenMembers(unionMembers)
                .Distinct()
                .OrderBy(m => m.Name, StringComparer.Ordinal)
                .ToImmutableArray();

            return finalMembers.Length switch
            {
                0 => new UnionType("never", ImmutableArray<TypeSymbol>.Empty),
                1 => finalMembers[0],
                _ => new UnionType(FormatName(finalMembers), finalMembers)
            };
        }

        public static TypeSymbol Create(params TypeSymbol[] members) => Create((IEnumerable<TypeSymbol>) members);

        private static IEnumerable<TypeSymbol> FlattenMembers(IEnumerable<TypeSymbol> members) => 
            members.SelectMany(member => member is UnionType union 
                ? FlattenMembers(union.Members)
                : member.AsEnumerable());

        private static string FormatName(IEnumerable<TypeSymbol> unionMembers) => unionMembers.Select(m => m.Name).ConcatString(" | ");
    }
}

