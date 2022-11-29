// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

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

            if (aggregatedItemType.TypeKind == TypeKind.Never || aggregatedItemType.TypeKind == TypeKind.Any)
            {
                // it doesn't really make sense to collapse 'never' or 'any'
                return null;
            }

            if (aggregatedItemType is UnionType unionType)
            {
                if (unionType.Members.All(x => x is StringLiteralType || x == LanguageConstants.String))
                {
                    return unionType.Members.Any(x => x == LanguageConstants.String) ?
                        LanguageConstants.String :
                        unionType;
                }

                if (unionType.Members.All(x => TypeValidator.AreTypesAssignable(x.Type, LanguageConstants.Bool)))
                {
                    return unionType.Members.Any(x => x == LanguageConstants.Bool)
                        ? LanguageConstants.Bool
                        : unionType;
                }

                if (unionType.Members.All(x => TypeValidator.AreTypesAssignable(x.Type, LanguageConstants.Int)))
                {
                    return unionType.Members.Any(x => x == LanguageConstants.Int)
                        ? LanguageConstants.Bool
                        : unionType;
                }

                // We have a mix of item types that cannot be collapsed
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
            => CreateTypeUnion((IEnumerable<ITypeReference>)members);

        public static bool IsLiteralType(TypeSymbol type) => type switch
        {
            StringLiteralType => true,
            IntegerLiteralType => true,
            BooleanLiteralType => true,

            // A tuple can be a literal only if each item contained therein is also a literal
            TupleType tupleType => tupleType.Items.All(t => IsLiteralType(t.Type)),

            // An object type can be a literal iff:
            //   - All properties are themselves of a literal type
            //   - No properties are optional
            //   - Only explicitly defined properties are accepted (i.e., no additional properties are permitted)
            //
            // The lattermost condition is identified by the object type either not defining an AdditionalPropertiesType
            // or explicitly flagging the AdditionalPropertiesType as a fallback (the default for non-sealed user-defined types)
            ObjectType objectType => (objectType.AdditionalPropertiesType is null || objectType.AdditionalPropertiesFlags.HasFlag(TypePropertyFlags.FallbackProperty)) &&
                objectType.Properties.All(kvp => kvp.Value.Flags.HasFlag(TypePropertyFlags.Required) && IsLiteralType(kvp.Value.TypeReference.Type)),

            _ => false,
        };

        /// <summary>
        /// Gets the type of the property whose name we can obtain at compile-time.
        /// </summary>
        /// <param name="baseType">The base object type</param>
        /// <param name="propertyExpressionPositionable">The position of the property name expression</param>
        /// <param name="propertyName">The resolved property name</param>
        /// <param name="shouldWarn">Whether diagnostics with a configurable level should be issued as warnings</param>
        /// <param name="diagnostics">Sink for diagnostics are not included in the return type symbol</param>
        public static TypeSymbol GetNamedPropertyType(ObjectType baseType, IPositionable propertyExpressionPositionable, string propertyName, bool shouldWarn, IDiagnosticWriter diagnostics)
        {
            if (baseType.TypeKind == TypeKind.Any)
            {
                // all properties of "any" type are of type "any"
                return LanguageConstants.Any;
            }

            // is there a declared property with this name
            var declaredProperty = baseType.Properties.TryGetValue(propertyName);
            if (declaredProperty != null)
            {
                if (declaredProperty.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                {
                    var writeOnlyDiagnostic = DiagnosticBuilder.ForPosition(propertyExpressionPositionable).WriteOnlyProperty(shouldWarn, baseType, propertyName);
                    diagnostics.Write(writeOnlyDiagnostic);

                    if (writeOnlyDiagnostic.Level == DiagnosticLevel.Error)
                    {
                        return ErrorType.Create(Enumerable.Empty<ErrorDiagnostic>());
                    }
                }

                if (declaredProperty.Flags.HasFlag(TypePropertyFlags.FallbackProperty))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(propertyExpressionPositionable).FallbackPropertyUsed(propertyName));
                }

                // there is - return its type
                return declaredProperty.TypeReference.Type;
            }

            // the property is not declared
            // check additional properties
            if (baseType.AdditionalPropertiesType != null)
            {
                // yes - return the additional property type
                return baseType.AdditionalPropertiesType.Type;
            }

            var availableProperties = baseType.Properties.Values
                .Where(p => !p.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(p => p.Name)
                .OrderBy(x => x);

            var diagnosticBuilder = DiagnosticBuilder.ForPosition(propertyExpressionPositionable);

            var unknownPropertyDiagnostic = availableProperties.Any() switch
            {
                true => SpellChecker.GetSpellingSuggestion(propertyName, availableProperties) switch
                {
                    string suggestedPropertyName when suggestedPropertyName != null =>
                        diagnosticBuilder.UnknownPropertyWithSuggestion(shouldWarn, baseType, propertyName, suggestedPropertyName),
                    _ => diagnosticBuilder.UnknownPropertyWithAvailableProperties(shouldWarn, baseType, propertyName, availableProperties),
                },
                _ => diagnosticBuilder.UnknownProperty(shouldWarn, baseType, propertyName)
            };

            diagnostics.Write(unknownPropertyDiagnostic);

            return (unknownPropertyDiagnostic.Level == DiagnosticLevel.Error) ? ErrorType.Create(Enumerable.Empty<ErrorDiagnostic>()) : LanguageConstants.Any;
        }

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

            if (flattenedMembers.Any(member => member.Type == LanguageConstants.Array))
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

        public static bool SatisfiesCondition(TypeSymbol typeSymbol, Func<TypeSymbol, bool> conditionFunc)
            => typeSymbol switch {
                UnionType unionType => unionType.Members.All(t => conditionFunc(t.Type)),
                _ => conditionFunc(typeSymbol),
            };
    }
}
