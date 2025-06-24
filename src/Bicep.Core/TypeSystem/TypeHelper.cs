// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Types;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.TypeSystem
{
    public static class TypeHelper
    {
        private static TypeComparer typeComparer = new();

        /// <summary>
        /// Try to collapse multiple types into a single (non-union) type. Returns null if this is not possible.
        /// </summary>
        public static TypeSymbol? TryCollapseTypes(IEnumerable<ITypeReference> itemTypes)
            => TypeCollapser.TryCollapse(CreateTypeUnion(itemTypes));

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

        public static TypeSymbol CollapseOrCreateTypeUnion(IEnumerable<ITypeReference> itemTypes)
        {
            var unionType = CreateTypeUnion(itemTypes);
            return TypeCollapser.TryCollapse(unionType) ?? unionType;
        }

        public static TypeSymbol CollapseOrCreateTypeUnion(params ITypeReference[] itemTypes)
            => CollapseOrCreateTypeUnion((IEnumerable<ITypeReference>)itemTypes);

        /// <summary>
        /// Makes a type nullable by unioning it with null
        /// </summary>
        public static TypeSymbol MakeNullable(ITypeReference typeReference) => CreateTypeUnion(typeReference, LanguageConstants.Null);

        public static LambdaType CreateLambdaType(IEnumerable<ITypeReference> argumentTypes, IEnumerable<ITypeReference> optionalArgumentTypes, TypeSymbol returnType)
            => new([.. argumentTypes], [.. optionalArgumentTypes], returnType);

        /// <summary>
        /// Returns an ordered enumerable of type names.
        /// </summary>
        /// <param name="types">The types to get ordered names for.</param>
        public static IEnumerable<string> GetOrderedTypeNames(IEnumerable<ITypeReference> types) =>
            types.Select(t => t.Type).Order(typeComparer).Select(t => t.Name);

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
            StringLiteralType or
            IntegerLiteralType or
            BooleanLiteralType or
            NullType => true,

            // A tuple can be a literal only if each item contained therein is also a literal
            TupleType tupleType => tupleType.Items.All(t => IsLiteralType(t.Type)),

            // An object type can be a literal iff:
            //   - All properties are themselves of a literal type
            //   - No properties are optional
            //   - Only explicitly defined properties are accepted (i.e., no additional properties are permitted)
            //
            // The lattermost condition is identified by the object type either not defining an AdditionalPropertiesType
            // or explicitly flagging the AdditionalPropertiesType as a fallback (the default for non-sealed user-defined types)
            ObjectType objectType => (objectType.AdditionalProperties is null || objectType.AdditionalProperties.Flags.HasFlag(TypePropertyFlags.FallbackProperty)) &&
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
            _ => token.Type switch
            {
                JTokenType.Null => LanguageConstants.Null,
                JTokenType.Boolean => token.ToObject<bool>() ? LanguageConstants.True : LanguageConstants.False,
                JTokenType.Integer when token.ToObject<BigInteger>() is BigInteger intVal && long.MinValue <= intVal && intVal <= long.MaxValue => TypeFactory.CreateIntegerLiteralType((long)intVal),
                JTokenType.String or JTokenType.Uri => TypeFactory.CreateStringLiteralType(token.ToString()),
                _ => null,
            },
        };

        private static TypeSymbol? TryCreateTypeLiteral(JObject jObject)
        {
            List<NamedTypeProperty> convertedProperties = new();
            ObjectTypeNameBuilder nameBuilder = new();
            foreach (var prop in jObject.Properties())
            {
                if (TryCreateTypeLiteral(prop.Value) is TypeSymbol propType)
                {
                    convertedProperties.Add(new(prop.Name, propType, TypePropertyFlags.Required | TypePropertyFlags.DisallowAny));
                    nameBuilder.AppendProperty(prop.Name, propType.Name);
                }
                else
                {
                    return null;
                }
            }

            return new ObjectType(nameBuilder.ToString(), TypeSymbolValidationFlags.Default, convertedProperties, additionalProperties: default);
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

            return new TupleType(nameBuilder.ToString(), [.. convertedItems], TypeSymbolValidationFlags.Default);
        }

        public static TypeSymbol GetNamedPropertyType(
            UnionType unionType,
            IPositionable propertyExpressionPositionable,
            string propertyName,
            bool isSafeAccess,
            bool shouldWarn,
            IDiagnosticWriter diagnostics) => TryCollapseTypes(unionType.Members) switch
            {
                ObjectType @object => GetNamedPropertyType(
                    @object,
                    propertyExpressionPositionable,
                    propertyName,
                    isSafeAccess,
                    shouldWarn,
                    diagnostics),
                DiscriminatedObjectType taggedUnion => GetNamedPropertyType(
                    taggedUnion,
                    propertyExpressionPositionable,
                    propertyName,
                    isSafeAccess,
                    shouldWarn,
                    diagnostics),
                null when unionType.Members.All(t => t.Type is ModuleType) &&
                 CreateTypeUnion(unionType.Members.Select(t => ((ModuleType)t.Type).Body)) is UnionType bodyUnion
                    => GetNamedPropertyType(
                        bodyUnion,
                        propertyExpressionPositionable,
                        propertyName,
                        isSafeAccess,
                        shouldWarn,
                        diagnostics),
                // TODO improve later here if necessary - we should be able to block stuff that is obviously wrong
                _ => LanguageConstants.Any,
            };

        public static TypeSymbol GetNamedPropertyType(
            DiscriminatedObjectType discriminatedObjectType,
            IPositionable propertyExpressionPositionable,
            string propertyName,
            bool isSafeAccess,
            bool shouldWarn,
            IDiagnosticWriter diagnostics)
        {
            if (propertyName.Equals(discriminatedObjectType.DiscriminatorProperty.Name))
            {
                return discriminatedObjectType.DiscriminatorProperty.TypeReference.Type;
            }

            TypePropertyFlags flags = TypePropertyFlags.None;
            List<TypeSymbol> propertyTypes = new();
            var declaredOnAny = false;

            foreach (var member in discriminatedObjectType.UnionMembersByKey.Values)
            {
                if (member.Properties.TryGetValue(propertyName, out var memberProperty))
                {
                    declaredOnAny = true;
                    propertyTypes.Add(memberProperty.TypeReference.Type);
                    flags |= memberProperty.Flags;
                }
                else if (member.AdditionalProperties is { } addlProperties)
                {
                    declaredOnAny = true;
                    propertyTypes.Add(addlProperties.TypeReference.Type);
                    flags |= addlProperties.Flags;
                }
                else
                {
                    propertyTypes.Add(LanguageConstants.Null);
                    flags |= TypePropertyFlags.FallbackProperty;
                }
            }

            if (declaredOnAny)
            {
                return GenerateAccessError(flags, discriminatedObjectType, propertyExpressionPositionable, propertyName, isSafeAccess, shouldWarn, diagnostics)
                    ?? TypeHelper.CollapseOrCreateTypeUnion(propertyTypes);
            }

            return GetUnknownPropertyType(
                discriminatedObjectType,
                discriminatedObjectType.UnionMembersByKey.Values.SelectMany(obj => obj.Properties.Values),
                propertyExpressionPositionable,
                propertyName,
                shouldWarn,
                diagnostics);
        }

        private static ErrorType? GenerateAccessError(
            TypePropertyFlags flags,
            TypeSymbol baseType,
            IPositionable propertyExpressionPositionable,
            string propertyName,
            bool isSafeAccess,
            bool shouldWarn,
            IDiagnosticWriter diagnostics)
        {
            if (flags.HasFlag(TypePropertyFlags.WriteOnly))
            {
                var writeOnlyDiagnostic = DiagnosticBuilder.ForPosition(propertyExpressionPositionable).WriteOnlyProperty(shouldWarn, baseType, propertyName);
                diagnostics.Write(writeOnlyDiagnostic);

                if (writeOnlyDiagnostic.IsError())
                {
                    return ErrorType.Empty();
                }
            }

            if (flags.HasFlag(TypePropertyFlags.FallbackProperty))
            {
                diagnostics.Write(DiagnosticBuilder.ForPosition(propertyExpressionPositionable)
                    .FallbackPropertyUsed(shouldDowngrade: isSafeAccess, propertyName));
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the property whose name we can obtain at compile-time.
        /// </summary>
        /// <param name="baseType">The base object type</param>
        /// <param name="propertyExpressionPositionable">The position of the property name expression</param>
        /// <param name="propertyName">The resolved property name</param>
        /// <param name="isSafeAccess">Whether the expression accessing this property uses null-conditional access.</param>
        /// <param name="shouldWarn">Whether diagnostics with a configurable level should be issued as warnings</param>
        /// <param name="diagnostics">Sink for diagnostics are not included in the return type symbol</param>
        public static TypeSymbol GetNamedPropertyType(
            ObjectType baseType,
            IPositionable propertyExpressionPositionable,
            string propertyName,
            bool isSafeAccess,
            bool shouldWarn,
            IDiagnosticWriter diagnostics)
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
                // there is - return its type or any error raised by its use
                return GenerateAccessError(declaredProperty.Flags, baseType, propertyExpressionPositionable, propertyName, isSafeAccess, shouldWarn, diagnostics)
                    ?? declaredProperty.TypeReference.Type;
            }

            // the property is not declared
            // check additional properties
            if (baseType.AdditionalProperties is { } addlProperties)
            {
                // yes - return the additional property type or any error raised by its use
                return GenerateAccessError(addlProperties.Flags, baseType, propertyExpressionPositionable, propertyName, isSafeAccess, shouldWarn, diagnostics)
                    ?? addlProperties.TypeReference.Type;
            }

            return GetUnknownPropertyType(
                baseType,
                baseType.Properties.Values,
                propertyExpressionPositionable,
                propertyName,
                shouldWarn,
                diagnostics);
        }

        private static TypeSymbol GetUnknownPropertyType(
            TypeSymbol baseType,
            IEnumerable<NamedTypeProperty> properties,
            IPositionable propertyExpressionPositionable,
            string propertyName,
            bool shouldWarn,
            IDiagnosticWriter diagnostics)
        {
            var unknownPropertyDiagnostic = GetUnknownPropertyDiagnostic(baseType, properties, propertyName, shouldWarn)
                .Invoke(DiagnosticBuilder.ForPosition(propertyExpressionPositionable));

            diagnostics.Write(unknownPropertyDiagnostic);

            return unknownPropertyDiagnostic.IsError() ? ErrorType.Empty() : LanguageConstants.Any;
        }

        public static DiagnosticBuilder.DiagnosticBuilderDelegate GetUnknownPropertyDiagnostic(ObjectType baseType, string propertyName, bool shouldWarn)
            => GetUnknownPropertyDiagnostic(baseType, baseType.Properties.Values, propertyName, shouldWarn);

        public static DiagnosticBuilder.DiagnosticBuilderDelegate GetUnknownPropertyDiagnostic(
            TypeSymbol baseType,
            IEnumerable<NamedTypeProperty> properties,
            string propertyName,
            bool shouldWarn)
        {
            var availableProperties = properties
                .Where(p => !p.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(p => p.Name)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return availableProperties.Any() switch
            {
                true => SpellChecker.GetSpellingSuggestion(propertyName, availableProperties) switch
                {
                    string suggestedPropertyName when suggestedPropertyName != null =>
                        x => x.UnknownPropertyWithSuggestion(shouldWarn, baseType, propertyName, suggestedPropertyName),
                    _ => x => x.UnknownPropertyWithAvailableProperties(shouldWarn, baseType, propertyName, availableProperties),
                },
                _ => x => x.UnknownProperty(shouldWarn, baseType, propertyName)
            };
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

                return new TupleType(nameBuilder.ToString(), [.. flattenedItems], flags);
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
            UnionType union when union.Members.Where(m => !ReferenceEquals(m.Type, LanguageConstants.Null)).ToImmutableArray() is { } sansNull &&
                sansNull.Length < union.Members.Length => CreateTypeUnion(sansNull),
            _ => null,
        };

        public static bool IsNullable(TypeSymbol type) => TryRemoveNullability(type) is not null;

        public static bool IsRequired(TypeProperty typeProperty)
            => typeProperty.Flags.HasFlag(TypePropertyFlags.Required) && !TypeHelper.IsNullable(typeProperty.TypeReference.Type);

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
            BooleanType => (4, 5),
            // opening and closing square or curly brackets will at least be present
            ArrayType or ObjectType or DiscriminatedObjectType => (2, null),
            _ => (0, null),
        };

        public static ResultWithDiagnosticBuilder<ResourceType> GetResourceTypeFromString(IBinder binder, string stringContent, ResourceTypeGenerationFlags typeGenerationFlags, ResourceType? parentResourceType)
        {
            var colonIndex = stringContent.IndexOf(':');
            if (colonIndex > 0)
            {
                var scheme = stringContent[..colonIndex];
                var typeString = stringContent[(colonIndex + 1)..];

                if (binder.NamespaceResolver.TryGetNamespace(scheme) is not { } namespaceType)
                {
                    return new(span => span.UnknownResourceReferenceScheme(scheme, binder.NamespaceResolver.GetNamespaceNames().OrderBy(x => x, StringComparer.OrdinalIgnoreCase)));
                }

                if (parentResourceType is not null &&
                    parentResourceType.DeclaringNamespace != namespaceType)
                {
                    return new(span => span.ParentResourceInDifferentNamespace(namespaceType.Name, parentResourceType.DeclaringNamespace.Name));
                }

                if (!GetCombinedTypeReference(typeGenerationFlags, parentResourceType, typeString).IsSuccess(out var typeReference, out var builder))
                {
                    return new(builder);
                }

                if (namespaceType.ResourceTypeProvider.TryGetDefinedType(namespaceType, typeReference, typeGenerationFlags) is { } definedResource)
                {
                    return new(definedResource);
                }

                if (namespaceType.ResourceTypeProvider.TryGenerateFallbackType(namespaceType, typeReference, typeGenerationFlags) is { } defaultResource)
                {
                    return new(defaultResource);
                }

                return new(span => span.FailedToFindResourceTypeInNamespace(namespaceType.ExtensionName, typeReference.FormatName()));
            }

            if (!GetCombinedTypeReference(typeGenerationFlags, parentResourceType, stringContent).IsSuccess(out var typeRef, out var errorBuilder))
            {
                return new(errorBuilder);
            }

            var resourceTypes = binder.NamespaceResolver.GetMatchingResourceTypes(typeRef, typeGenerationFlags);
            return resourceTypes.Length switch
            {
                0 => new(span => span.InvalidResourceType()),
                1 => new(resourceTypes[0]),
                _ => new(span => span.AmbiguousResourceTypeBetweenImports(typeRef.FormatName(), resourceTypes.Select(x => x.DeclaringNamespace.Name))),
            };
        }

        public static bool SatisfiesCondition(TypeSymbol typeSymbol, Func<TypeSymbol, bool> conditionFunc)
            => typeSymbol switch
            {
                UnionType unionType => unionType.Members.All(t => conditionFunc(t.Type)),
                _ => conditionFunc(typeSymbol),
            };

        public static FunctionOverload OverloadWithResolvedTypes(ResourceDerivedTypeResolver resolver, ExportedFunctionMetadata exportedFunction)
        {
            FunctionOverloadBuilder builder = new(exportedFunction.Name);
            if (exportedFunction.Description is string description)
            {
                builder = builder.WithGenericDescription(description).WithDescription(description);
            }

            foreach (var param in exportedFunction.Parameters)
            {
                builder = builder.WithRequiredParameter(param.Name,
                    resolver.ResolveResourceDerivedTypes(param.TypeReference.Type),
                    param.Description ?? string.Empty);
            }

            return builder.WithReturnType(resolver.ResolveResourceDerivedTypes(exportedFunction.Return.TypeReference.Type)).Build();
        }

        public static ObjectType CreateDictionaryType(string name, TypeSymbolValidationFlags validationFlags, ITypeReference valueType)
        {
            return new(name, validationFlags, ImmutableArray<NamedTypeProperty>.Empty, new(valueType));
        }

        private static ImmutableArray<ITypeReference> NormalizeTypeList(IEnumerable<ITypeReference> unionMembers)
        {
            HashSet<TypeSymbol> distinctMembers = new();
            bool hasUnrefinedUntypedArrayMember = false;
            foreach (var member in FlattenMembers(unionMembers))
            {
                if (member is AnyType)
                {
                    // a union type with "| any" is the same as "any" type
                    return [LanguageConstants.Any];
                }

                if (hasUnrefinedUntypedArrayMember && member is ArrayType)
                {
                    continue;
                }

                if (member.Equals(LanguageConstants.Array))
                {
                    hasUnrefinedUntypedArrayMember = true;
                    distinctMembers.RemoveWhere(t => t is ArrayType);
                }

                distinctMembers.Add(member);
            }

            return [.. distinctMembers.Order(typeComparer)];
        }

        private static IEnumerable<TypeSymbol> FlattenMembers(IEnumerable<ITypeReference> members) =>
            members.Select(member => member.Type).SelectMany(member => member is UnionType union
                ? FlattenMembers(union.Members)
                : member.AsEnumerable());

        private static string FormatName(IEnumerable<ITypeReference> unionMembers) =>
            unionMembers.Select(m => m.Type.FormatNameForCompoundTypes()).ConcatString(" | ");

        private static ResultWithDiagnosticBuilder<ResourceTypeReference> GetCombinedTypeReference(ResourceTypeGenerationFlags flags, ResourceType? parentResourceType, string typeString)
        {
            if (ResourceTypeReference.TryParse(typeString) is not { } typeReference)
            {
                return new(span => span.InvalidResourceType());
            }

            if (!flags.HasFlag(ResourceTypeGenerationFlags.NestedResource))
            {
                // this is not a syntactically nested resource - return the type reference as-is
                return new(typeReference);
            }

            // we're dealing with a syntactically nested resource here
            if (parentResourceType is null)
            {
                return new(span => span.InvalidAncestorResourceType());
            }

            if (typeReference.TypeSegments.Length > 1)
            {
                // OK this resource is the one that's wrong.
                return new(span => span.InvalidResourceTypeSegment(typeString));
            }

            return new(ResourceTypeReference.Combine(parentResourceType.TypeReference, typeReference));
        }

        private static ObjectType TransformProperties(ObjectType input, Func<NamedTypeProperty, NamedTypeProperty> transformFunc)
        {
            return new ObjectType(
                input.Name,
                input.ValidationFlags,
                input.Properties.Values.Select(transformFunc),
                input.AdditionalProperties,
                input.MethodResolver.functionOverloads);
        }

        public static ObjectType MakeRequiredPropertiesOptional(ObjectType input)
            => TransformProperties(input, p => p with { Flags = p.Flags & ~TypePropertyFlags.Required });

        public static TypeSymbol RemovePropertyFlagsRecursively(TypeSymbol type, TypePropertyFlags flagsToRemove)
            => ModifyPropertyFlagsRecursively(type, f => f & ~flagsToRemove, new());

        private static TType ModifyPropertyFlagsRecursively<TType>(
            TType type,
            Func<TypePropertyFlags, TypePropertyFlags> transformFlags,
            ConcurrentDictionary<ObjectType, ObjectType> transformedObjectCache) where TType : TypeSymbol =>
            type switch
            {
                ObjectType @object => (transformedObjectCache.GetOrAdd(
                    @object,
                    obj => ModifyPropertyFlagsRecursively(obj, transformFlags, transformedObjectCache)) as TType)!,
                _ => type,
            };

        private static ObjectType ModifyPropertyFlagsRecursively(
            ObjectType @object,
            Func<TypePropertyFlags, TypePropertyFlags> transformFlags,
            ConcurrentDictionary<ObjectType, ObjectType> cache) => TransformProperties(@object, property => new(
                property.Name,
                new DeferredTypeReference(
                    () => ModifyPropertyFlagsRecursively(
                    property.TypeReference.Type,
                    transformFlags,
                    cache)),
                transformFlags(property.Flags),
                property.Description));

        /// <summary>
        /// Validates that the supplied pattern is: 1) a syntactically valid regular expression, and 2) compatible with
        /// .NET's non-backtracking regular expression engine.
        /// </summary>
        /// <param name="pattern">The regular expression pattern</param>
        /// <returns>The pattern string iff it can be used with the non-backtracking engine.</returns>
        public static string? AsOptionalValidFiniteRegexPattern(string? pattern)
        {
            if (pattern is not null && TryGetRegularExpressionValidationException(pattern) is null)
            {
                return pattern;
            }

            return null;
        }

        /// <summary>
        /// Attempts to instantiate a <see cref="Regex"/> with the supplied pattern and returns the error raised
        /// thereby.
        /// </summary>
        /// <param name="pattern">The regular expression pattern</param>
        /// <returns>The exception raised by <see cref="Regex.Regex(string, RegexOptions)"/>, if any.</returns>
        public static Exception? TryGetRegularExpressionValidationException(string pattern)
        {
            try
            {
                var _ = new Regex(pattern, RegexOptions.NonBacktracking);
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static bool MatchesPattern(string pattern, string value)
            => Regex.IsMatch(value, pattern, RegexOptions.NonBacktracking);

        public static bool IsOrContainsSecureType(TypeSymbol type)
            => FindPathsToSecureTypeComponents(type, false).Any();

        public static IEnumerable<string> FindPathsToSecureTypeComponents(TypeSymbol type, bool hasTrailingAccessExpressions = false)
            => FindPathsToSecureTypeComponents(
                type,
                hasTrailingAccessExpressions,
                path: "",
                currentlyProcessing: new(ReferenceEqualityComparer.Instance));

        private static IEnumerable<string> FindPathsToSecureTypeComponents(
            TypeSymbol type,
            bool hasTrailingAccessExpressions,
            string path,
            HashSet<TypeSymbol> currentlyProcessing)
        {
            // types can be recursive. cut out early if we've already seen this type
            if (!currentlyProcessing.Add(type))
            {
                yield break;
            }

            if (type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
            {
                yield return path;
            }

            if (type is UnionType union)
            {
                foreach (var variantPath in union.Members.SelectMany(m => FindPathsToSecureTypeComponents(
                    m.Type,
                    hasTrailingAccessExpressions,
                    path,
                    currentlyProcessing)))
                {
                    yield return variantPath;
                }
            }

            // if the expression being visited is dereferencing a specific property or index of this type, we shouldn't warn if the type under inspection
            // *contains* properties or indices that are flagged as secure. We will have already warned if those have been accessed in the expression, and
            // if they haven't, then the value dereferenced isn't sensitive
            //
            //    param p {
            //      prop: {
            //        @secure()
            //        nestedSecret: string
            //        nestedInnocuousProperty: string
            //      }
            //    }
            //
            //    output objectContainingSecrets object = p                     // <-- should be flagged
            //    output propertyContainingSecrets object = p.prop              // <-- should be flagged
            //    output nestedSecret string = p.prop.nestedSecret              // <-- should be flagged
            //    output siblingOfSecret string = p.prop.nestedInnocuousData    // <-- should NOT be flagged
            if (!hasTrailingAccessExpressions)
            {
                switch (type)
                {
                    case ObjectType obj:
                        if (obj.AdditionalProperties?.TypeReference.Type is TypeSymbol addlPropsType)
                        {
                            foreach (var dictMemberPath in FindPathsToSecureTypeComponents(
                                addlPropsType,
                                hasTrailingAccessExpressions,
                                $"{path}.*",
                                currentlyProcessing))
                            {
                                yield return dictMemberPath;
                            }
                        }

                        foreach (var propertyPath in obj.Properties.SelectMany(p => FindPathsToSecureTypeComponents(
                            p.Value.TypeReference.Type,
                            hasTrailingAccessExpressions,
                            $"{path}.{p.Key}",
                            currentlyProcessing)))
                        {
                            yield return propertyPath;
                        }
                        break;
                    case TupleType tuple:
                        foreach (var pathFromIndex in tuple.Items.SelectMany((t, i) => FindPathsToSecureTypeComponents(
                            t.Type,
                            hasTrailingAccessExpressions,
                            $"{path}[{i}]",
                            currentlyProcessing)))
                        {
                            yield return pathFromIndex;
                        }
                        break;
                    case ArrayType array:
                        foreach (var pathFromElement in FindPathsToSecureTypeComponents(
                            array.Item.Type,
                            hasTrailingAccessExpressions,
                            $"{path}[*]",
                            currentlyProcessing))
                        {
                            yield return pathFromElement;
                        }
                        break;
                }
            }

            currentlyProcessing.Remove(type);
        }
    }
}
