// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;

namespace Bicep.Core.TypeSystem
{
    public static class TypeHelper
    {
        /// <summary>
        /// Try to collapse multiple types into a single (non-union) type. Returns null if this is not possible.
        /// </summary>
        public static TypeSymbol? TryCollapseTypes(IEnumerable<ITypeReference> itemTypes)
        {
            var aggregatedItemType = CreateTypeUnion(itemTypes);
            if (aggregatedItemType.TypeKind == TypeKind.Union || aggregatedItemType.TypeKind == TypeKind.Never || aggregatedItemType.TypeKind == TypeKind.Any)
            {
                // We have a mix of item types or none
                return null;
            }

            return aggregatedItemType;
        }

        /// <summary>
        /// Collapses multiple types into either:
        /// * The 'never' type, if there are no types in the source list.
        /// * A single type, if the source types can be collapsed into a single type.
        /// * A union type.
        /// </summary>
        public static TypeSymbol CreateTypeUnion(IEnumerable<ITypeReference> unionMembers)
        {
            var normalizedMembers = NormalizeTypeList(unionMembers);

            return normalizedMembers.Length switch
            {
                0 => LanguageConstants.Never,
                1 => normalizedMembers[0].Type,
                _ => new UnionType(FormatName(normalizedMembers), normalizedMembers)
            };
        }

        /// <summary>
        /// Collapses multiple types into either:
        /// * The 'never' type, if there are no types in the source list.
        /// * A single type, if the source types can be collapsed into a single type.
        /// * A union type.
        /// </summary>
        public static TypeSymbol CreateTypeUnion(params ITypeReference[] members) 
            => CreateTypeUnion((IEnumerable<ITypeReference>) members);

        private static ImmutableArray<ITypeReference> NormalizeTypeList(IEnumerable<ITypeReference> unionMembers)
        {
            // flatten and then de-duplicate members
            var flattenedMembers = FlattenMembers(unionMembers)
                .Distinct()
                .OrderBy(m => m.Type.Name, StringComparer.Ordinal)
                .ToImmutableArray();

            if (flattenedMembers.Any(member => member is AnyType))
            {
                // a union type with "| any" is the same as "any" type
                return ImmutableArray.Create<ITypeReference>(LanguageConstants.Any);
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

            return intermediateMembers.ToImmutableArray();
        }

        private static IEnumerable<ITypeReference> FlattenMembers(IEnumerable<ITypeReference> members) => 
            members.SelectMany(member => member.Type is UnionType union 
                ? FlattenMembers(union.Members)
                : member.AsEnumerable());

        private static string FormatName(IEnumerable<ITypeReference> unionMembers) => 
            unionMembers.Select(m => m.Type.FormatNameForCompoundTypes()).ConcatString(" | ");
    }
}
