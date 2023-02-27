// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Azure.Deployments.Expression.Extensions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;

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
        /// Attempt to create a type symbol for a literal value.
        /// </summary>
        /// <param name="token">The literal value (expressed as a Newtonsoft JToken)</param>
        /// <returns></returns>
        public static TypeSymbol? TryCreateTypeLiteral(JToken token) => token switch
        {
            JObject jObject => TryCreateTypeLiteral(jObject),
            JArray jArray => TryCreateTypeLiteral(jArray),
            _ when token.Type == JTokenType.Boolean => new BooleanLiteralType(token.ToObject<bool>()),
            _ when token.IsTextBasedJTokenType() => TypeFactory.CreateStringLiteralType(token.ToString()),
            _ when token.Type == JTokenType.Integer && token.ToObject<BigInteger>() is BigInteger intVal && long.MinValue <= intVal && intVal <= long.MaxValue => TypeFactory.CreateIntegerLiteralType((long)intVal),
            _ => null,
        };

        private static TypeSymbol? TryCreateTypeLiteral(JObject jObject)
        {
            List<TypeProperty> convertedProperties = new();
            ObjectTypeNameBuilder nameBuilder = new();
            foreach (var prop in jObject.Properties())
            {
                if (TryCreateTypeLiteral(prop.Value) is TypeSymbol propType)
                {
                    convertedProperties.Add(new(prop.Name, propType, TypePropertyFlags.Required | TypePropertyFlags.DisallowAny));
                    nameBuilder.AppendProperty(prop.Name, propType.Name, isOptional: false);
                }
                else
                {
                    return null;
                }
            }

            return new ObjectType(nameBuilder.ToString(), TypeSymbolValidationFlags.Default, convertedProperties, additionalPropertiesType: default);
        }

        private static TypeSymbol? TryCreateTypeLiteral(JArray jArray)
        {
            List<ITypeReference> convertedItems = new();
            TupleTypeNameBuilder nameBuilder = new();
            foreach (var item in jArray)
            {
                if (TryCreateTypeLiteral(item) is TypeSymbol itemType)
                {
                    convertedItems.Add(itemType);
                    nameBuilder.AppendItem(itemType.Name);
                }
                else
                {
                    return null;
                }
            }

            return new TupleType(nameBuilder.ToString(), convertedItems.ToImmutableArray(), TypeSymbolValidationFlags.Default);
        }

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

            ErrorType? GenerateAccessError(TypePropertyFlags flags)
            {
                if (flags.HasFlag(TypePropertyFlags.WriteOnly))
                {
                    var writeOnlyDiagnostic = DiagnosticBuilder.ForPosition(propertyExpressionPositionable).WriteOnlyProperty(shouldWarn, baseType, propertyName);
                    diagnostics.Write(writeOnlyDiagnostic);

                    if (writeOnlyDiagnostic.Level == DiagnosticLevel.Error)
                    {
                        return ErrorType.Create(Enumerable.Empty<ErrorDiagnostic>());
                    }
                }

                if (flags.HasFlag(TypePropertyFlags.FallbackProperty))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(propertyExpressionPositionable).FallbackPropertyUsed(propertyName));
                }

                return null;
            };

            // is there a declared property with this name
            var declaredProperty = baseType.Properties.TryGetValue(propertyName);
            if (declaredProperty != null)
            {
                // there is - return its type or any error raised by its use
                return GenerateAccessError(declaredProperty.Flags) ?? declaredProperty.TypeReference.Type;
            }

            // the property is not declared
            // check additional properties
            if (baseType.AdditionalPropertiesType != null)
            {
                // yes - return the additional property type or any error raised by its use
                return GenerateAccessError(baseType.AdditionalPropertiesFlags) ?? baseType.AdditionalPropertiesType.Type;
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

        public static TypeSymbol FlattenType(TypeSymbol typeToFlatten, IPositionable argumentPosition)
        {
            static TypeSymbol FlattenTuple(TypeSymbol flattenInputType, TupleType tupleType, IPositionable argumentPosition)
            {
                List<ITypeReference> flattenedItems = new();
                TupleTypeNameBuilder nameBuilder = new();
                TypeSymbolValidationFlags flags = TypeSymbolValidationFlags.Default;

                foreach (var item in tupleType.Items)
                {
                    if (item is TupleType itemTuple)
                    {
                        foreach (var subItem in itemTuple.Items)
                        {
                            nameBuilder.AppendItem(subItem.Type.Name);
                            flattenedItems.Add(subItem);
                        }
                        flags |= itemTuple.ValidationFlags;
                        continue;
                    }

                    // If we're not dealing with a tuple of tuples, just flatten `type` as if it were a normal array
                    return FlattenArray(flattenInputType, tupleType, argumentPosition);
                }

                return new TupleType(nameBuilder.ToString(), flattenedItems.ToImmutableArray(), flags);
            }

            static TypeSymbol FlattenUnionOfArrays(TypeSymbol flattenInputType, UnionType unionType, IPositionable argumentPosition) => UnionOfFlattened(
                flattenInputType,
                unionType.Members.Select(typeRef => CalculateFlattenedType(unionType, typeRef.Type, argumentPosition)),
                argumentPosition);

            static TypeSymbol FlattenArrayOfUnion(TypeSymbol flattenInputType, UnionType itemUnion, IPositionable argumentPosition)
                => UnionOfFlattened(flattenInputType, itemUnion.Members, argumentPosition);

            static TypeSymbol UnionOfFlattened(TypeSymbol flattenInputType, IEnumerable<ITypeReference> toFlatten, IPositionable argumentPosition)
            {
                List<ITypeReference> flattenedMembers = new();
                TypeSymbolValidationFlags flattenedFlags = TypeSymbolValidationFlags.Default;
                List<ErrorType> errors = new();

                foreach (var member in toFlatten)
                {
                    switch (member.Type)
                    {
                        case AnyType:
                            return LanguageConstants.Array;
                        case ErrorType errorType:
                            errors.Add(errorType);
                            break;
                        case ArrayType arrayType:
                            flattenedMembers.Add(arrayType.Item);
                            if (arrayType is TypedArrayType typedArrayType)
                            {
                                flattenedFlags |= typedArrayType.ValidationFlags;
                            }
                            break;
                        default:
                            errors.Add(ErrorType.Create(DiagnosticBuilder.ForPosition(argumentPosition).ValueCannotBeFlattened(flattenInputType, member.Type)));
                            break;
                    }
                }

                if (errors.Any())
                {
                    return ErrorType.Create(errors.SelectMany(e => e.GetDiagnostics()));
                }

                return new TypedArrayType(TypeHelper.CreateTypeUnion(flattenedMembers), flattenedFlags);
            }

            static TypeSymbol FlattenArray(TypeSymbol flattenInputType, ArrayType arrayType, IPositionable argumentPosition) => arrayType.Item.Type switch
            {
                ErrorType et => et,
                AnyType => LanguageConstants.Array,
                UnionType itemUnion => FlattenArrayOfUnion(flattenInputType, itemUnion, argumentPosition),
                TupleType itemTuple => new TypedArrayType(itemTuple.Item, itemTuple.ValidationFlags),
                ArrayType itemArray => itemArray,
                var otherwise => ErrorType.Create(DiagnosticBuilder.ForPosition(argumentPosition).ValueCannotBeFlattened(flattenInputType, otherwise)),
            };

            static TypeSymbol CalculateFlattenedType(TypeSymbol flattenInputType, TypeSymbol typeToFlatten, IPositionable argumentPosition) => typeToFlatten switch
            {
                AnyType => LanguageConstants.Array,
                TupleType tupleType => FlattenTuple(flattenInputType, tupleType, argumentPosition),
                UnionType unionType => FlattenUnionOfArrays(flattenInputType, unionType, argumentPosition),
                ArrayType arrayType => FlattenArray(flattenInputType, arrayType, argumentPosition),
                _ => ErrorType.Create(DiagnosticBuilder.ForPosition(argumentPosition).ValueCannotBeFlattened(flattenInputType, typeToFlatten)),
            };

            return CalculateFlattenedType(typeToFlatten, typeToFlatten, argumentPosition);
        }

        /// <remarks>
        /// If the provided type is a union of <code>null</code> and one or more other types, this function will return a union with the <code>null</code>
        /// branch removed. For example, <code>null | string</code> would be transformed to <code>string</code>, and <code>null | string | int</code> would be
        /// transformed to <code>string | int</code>.
        /// Otherwise, this method will return null.
        /// </remarks>
        public static TypeSymbol? TryRemoveNullability(TypeSymbol type) => type switch
        {
            UnionType union when union.Members.Where(m => !ReferenceEquals(m.Type, LanguageConstants.Null)).ToImmutableArray() is {} sansNull &&
                sansNull.Length < union.Members.Length => CreateTypeUnion(sansNull),
            _ => null,
        };

        /// <summary>
        /// Determines if the provided candidate type would be assignable to the provided expected type if the former were stripped of its nullability.
        /// </summary>
        /// <remarks>
        /// This function will return <code>false</code> if the provided candidate type is not nullable, even if it would be assignable to the provided expected
        /// type without modification.
        /// </remarks>
        public static bool WouldBeAssignableIfNonNullable(TypeSymbol candidateType, TypeSymbol expectedType, [NotNullWhen(true)] out TypeSymbol? nonNullableCandidateType)
        {
            if (TryRemoveNullability(candidateType) is TypeSymbol nonNullable && TypeValidator.AreTypesAssignable(nonNullable, expectedType))
            {
                nonNullableCandidateType = nonNullable;
                return true;
            }

            nonNullableCandidateType = null;
            return false;
        }

        /// <summary>
        /// Determines the possible range of lengths a supplied IntegerType will have when stringified under the invariant culture.
        /// </summary>
        public static (long minLength, long maxLength) GetMinAndMaxLengthOfStringified(IntegerType integer)
        {
            long minValue = integer.MinValue ?? long.MinValue;
            long minValueLength = minValue.ToString(CultureInfo.InvariantCulture).Length;
            long maxValue = integer.MaxValue ?? long.MaxValue;
            long maxValueLength = maxValue.ToString(CultureInfo.InvariantCulture).Length;

            if (minValue < 0 && maxValue >= 0)
            {
                // if the range of values crosses from negative into positive numbers, the value may be a single digit (`0`)
                return (1, Math.Max(minValueLength, maxValueLength));
            }

            return (Math.Min(minValueLength, maxValueLength), Math.Max(minValueLength, maxValueLength));
        }

        /// <summary>
        /// Determines the possible range of lengths a supplied TypeSymbol will have when stringified under the invariant culture.
        /// </summary>
        public static (long minLength, long? maxLength) GetMinAndMaxLengthOfStringified(TypeSymbol type) => type switch
        {
            _ when ArmFunctionReturnTypeEvaluator.TryEvaluate("string", out _, type.AsEnumerable()) is StringLiteralType stringified
                => (stringified.RawStringValue.Length, stringified.RawStringValue.Length),
            UnionType union when union.Members.Length == 0 => (0, 0),
            UnionType union => union.Members.Select(m => GetMinAndMaxLengthOfStringified(m.Type)).Aggregate((acc, next) => (
                Math.Min(acc.minLength, next.minLength),
                acc.maxLength.HasValue && next.maxLength.HasValue
                    ? Math.Max(acc.maxLength.Value, next.maxLength.Value)
                    : null)),
            StringType @string => (@string.MinLength ?? 0, @string.MaxLength),
            IntegerType integer => GetMinAndMaxLengthOfStringified(integer),
            // 'true' or 'false'
            PrimitiveType pt when pt.Name == LanguageConstants.TypeNameBool => (4, 5),
            // 'null'
            PrimitiveType pt when pt.Name == LanguageConstants.NullKeyword => (4, 4),
            // opening and closing square or curly brackets will at least be present
            ArrayType or ObjectType or DiscriminatedObjectType => (2, null),
            _ => (0, null),
        };

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
