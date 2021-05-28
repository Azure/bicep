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
            var flattenedMembers = FlattenMembers(unionMembers)
                .Distinct()
                .OrderBy(m => m.Type.Name, StringComparer.Ordinal)
                .ToImmutableArray();

            if(flattenedMembers.Any(member => member is AnyType))
            {
                // a union type with "| any" is the same as "any" type
                return LanguageConstants.Any;
            }

            IEnumerable<ITypeReference> intermediateMembers = flattenedMembers;
            if (flattenedMembers.Any(member => member.Type == LanguageConstants.String))
            {
                // the union has the base "string" type, so we can drop all string literal types from it
                intermediateMembers = intermediateMembers.Where(member => member.Type is not StringLiteralType);
            }

            if(flattenedMembers.Any(member => member.Type == LanguageConstants.Array))
            {
                // the union has the base "array" type, so we can drop any more specific array types
                intermediateMembers = intermediateMembers.Where(member => member.Type is not ArrayType || member.Type == LanguageConstants.Array);
            }

            var normalizedMembers = intermediateMembers.ToImmutableArray();

            return normalizedMembers.Length switch
            {
                0 => new UnionType("never", ImmutableArray<ITypeReference>.Empty),
                1 => normalizedMembers[0].Type,
                _ => new UnionType(FormatName(normalizedMembers), normalizedMembers)
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

