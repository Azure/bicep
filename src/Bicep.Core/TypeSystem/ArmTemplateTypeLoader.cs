// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Expression.Extensions;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.Exceptions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.TypeSystem;

public static class ArmTemplateTypeLoader
{
    public static ITypeReference ToTypeReference(SchemaValidationContext context, ITemplateSchemaNode armTemplateSchemaNode, TypeSymbolValidationFlags flags = TypeSymbolValidationFlags.Default)
    {
        try
        {
            var resolved = TemplateEngine.ResolveSchemaReferences(context, armTemplateSchemaNode);

            var bicepType = ToTypeReferenceIgnoringNullability(context, resolved, flags);

            if (resolved.Nullable?.Value == true)
            {
                bicepType = TypeHelper.CreateTypeUnion(bicepType, LanguageConstants.Null);
            }

            return bicepType;
        }
        catch (TemplateValidationException tve)
        {
            return ErrorType.Create(DiagnosticBuilder.ForDocumentStart().UnresolvableArmJsonType(tve.TemplateErrorAdditionalInfo.Path ?? "<unknown location>", tve.Message));
        }
    }

    private static ITypeReference ToTypeReferenceIgnoringNullability(SchemaValidationContext context, ITemplateSchemaNode withResolvedRefs, TypeSymbolValidationFlags flags)
    {
        if (TryGetResourceDerivedType(context, withResolvedRefs, flags) is ITypeReference resourceDerivedType)
        {
            return resourceDerivedType;
        }

        return withResolvedRefs.Type?.Value switch
        {
            TemplateParameterType.String => GetStringType(withResolvedRefs, flags),
            TemplateParameterType.SecureString => GetStringType(withResolvedRefs, flags | TypeSymbolValidationFlags.IsSecure),
            TemplateParameterType.Int => GetIntegerType(withResolvedRefs, flags),
            TemplateParameterType.Bool => GetBooleanType(withResolvedRefs, flags),
            TemplateParameterType.Array => GetArrayType(context, withResolvedRefs),
            TemplateParameterType.Object => GetObjectType(context, withResolvedRefs, flags),
            TemplateParameterType.SecureObject => GetObjectType(context, withResolvedRefs, flags | TypeSymbolValidationFlags.IsSecure),
            _ => LanguageConstants.Any,
        };
    }

    private static ITypeReference? TryGetResourceDerivedType(SchemaValidationContext context, ITemplateSchemaNode schemaNode, TypeSymbolValidationFlags flags)
    {
        if (schemaNode.Metadata?.Value is JObject metadataObject &&
            metadataObject.TryGetValue(LanguageConstants.MetadataResourceDerivedTypePropertyName, out var resourceType))
        {
            var fallbackType = ToTypeReference(context, new SansMetadata(schemaNode), flags).Type;
            var variant = ResourceDerivedTypeVariant.None;
            string? resourceTypeString = null;
            if (resourceType.GetProperty(LanguageConstants.MetadataResourceDerivedTypePointerPropertyName)
                ?.TryGetStringValue() is string pointerPropVal)
            {
                variant = resourceType.TryGetProperty<bool>(LanguageConstants.MetadataResourceDerivedTypeOutputFlagName)
                    ? ResourceDerivedTypeVariant.Output
                    : ResourceDerivedTypeVariant.Input;
                resourceTypeString = pointerPropVal;
            }
            // The legacy representation uses a string (the type pointer), not an object
            else if (resourceType.TryGetStringValue() is string strVal)
            {
                resourceTypeString = strVal;
            }

            if (resourceTypeString is null)
            {
                return new UnparsableResourceDerivedType(resourceType.ToString(), fallbackType);
            }

            var resourceTypeStringParts = resourceTypeString.Split('#', 2);
            var resourceTypeIdentifier = resourceTypeStringParts[0];
            var internalPointerSegments = resourceTypeStringParts.Length > 1
                ? resourceTypeStringParts[1].Split('/').Select(Bicep.Core.Extensions.StringExtensions.Rfc6901Decode).ToImmutableArray()
                : [];

            return ResourceTypeReference.TryParse(resourceTypeIdentifier) is ResourceTypeReference resourceTypeReference
                ? new UnresolvedResourceDerivedType(resourceTypeReference, internalPointerSegments, fallbackType, variant)
                : new UnparsableResourceDerivedType(resourceTypeIdentifier, fallbackType);
        }

        return null;
    }

    private static TypeSymbol GetStringType(ITemplateSchemaNode schemaNode, TypeSymbolValidationFlags flags)
    {
        if (schemaNode.AllowedValues?.Value is JArray jArray)
        {
            return TryGetLiteralUnionType(jArray, t => t.IsTextBasedJTokenType(), b => b.InvalidUnionTypeMember(LanguageConstants.TypeNameString));
        }

        return TypeFactory.CreateStringType(schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value, validationFlags: flags);
    }

    private static TypeSymbol TryGetLiteralUnionType(JArray allowedValues, Func<JToken, bool> validator, DiagnosticBuilder.DiagnosticBuilderDelegate diagnosticOnMismatch)
    {
        List<TypeSymbol> literalTypeTargets = new();
        foreach (var element in allowedValues)
        {
            if (element.Type == JTokenType.Null)
            {
                // The compiler was erroneously including `null` in the allowedValues array if the overall type was nullable (e.g., `type foo = ('bar' | 'baz')?`)
                // This error has been corrected, but some of those compiled templates may still be out in the wild, so just skip over any `null`s encountered
                continue;
            }

            if (!validator(element) || TypeHelper.TryCreateTypeLiteral(element) is not { } literal)
            {
                return ErrorType.Create(diagnosticOnMismatch(DiagnosticBuilder.ForDocumentStart()));
            }

            literalTypeTargets.Add(literal);
        }

        return TypeHelper.CreateTypeUnion(literalTypeTargets);
    }

    private static TypeSymbol GetIntegerType(ITemplateSchemaNode schemaNode, TypeSymbolValidationFlags flags)
    {
        if (schemaNode.AllowedValues?.Value is JArray jArray)
        {
            return TryGetLiteralUnionType(jArray, t => t.Type == JTokenType.Integer, b => b.InvalidUnionTypeMember(LanguageConstants.TypeNameInt));
        }

        return TypeFactory.CreateIntegerType(schemaNode.MinValue?.Value, schemaNode.MaxValue?.Value, flags);
    }

    private static TypeSymbol GetBooleanType(ITemplateSchemaNode schemaNode, TypeSymbolValidationFlags flags)
    {
        if (schemaNode.AllowedValues?.Value is JArray jArray)
        {
            return TryGetLiteralUnionType(jArray, t => t.Type == JTokenType.Boolean, b => b.InvalidUnionTypeMember(LanguageConstants.TypeNameBool));
        }

        return TypeFactory.CreateBooleanType(flags);
    }

    private static TypeSymbol GetArrayType(SchemaValidationContext context, ITemplateSchemaNode schemaNode)
    {
        if (schemaNode.AllowedValues?.Value is JArray allowedValues)
        {
            return GetArrayLiteralType(allowedValues, schemaNode);
        }

        if (schemaNode.PrefixItems is { } prefixItems)
        {
            TupleTypeNameBuilder nameBuilder = new();
            List<ITypeReference> tupleMembers = new();
            foreach (var prefixItem in prefixItems)
            {
                var (type, typeName) = GetDeferrableTypeInfo(context, prefixItem);
                nameBuilder.AppendItem(typeName);
                tupleMembers.Add(type);
            }

            return new TupleType(nameBuilder.ToString(), [.. tupleMembers], default);
        }

        if (schemaNode.Items?.SchemaNode is { } items)
        {
            if (items.Ref?.Value is not null)
            {
                var (type, typeName) = GetDeferrableTypeInfo(context, items);
                return new TypedArrayType($"{typeName}[]", type, default, schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value);
            }

            return new TypedArrayType(ToTypeReference(context, items), default, schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value);
        }

        // TODO it's possible to encounter an array with a defined prefix and either a schema or a boolean for "items."
        // TupleType does not support an "AdditionalItemsType" for items after the tuple, but when it does, update this type reader to handle the combination of "items" and "prefixItems"

        return TypeFactory.CreateArrayType(schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value);
    }

    private static TypeSymbol GetArrayLiteralType(JArray allowedValues, ITemplateSchemaNode schemaNode)
    {
        // For allowedValues on an array, either all or none of the allowed values need to be arrays.
        if (allowedValues.Any(t => t.Type == JTokenType.Array))
        {
            // If any of the allowed values are arrays, it's a regular union of literals
            return TryGetLiteralUnionType(allowedValues, t => t.Type == JTokenType.Array, b => b.InvalidUnionTypeMember(LanguageConstants.ArrayType));
        }

        // If no allowed values are arrays, the each element in the array must be one of the allowed values provided
        List<TypeSymbol> elements = new();
        foreach (var element in allowedValues)
        {
            // Arrays with constrained but mixed-type literal elements are the only place where `null` is a valid type literal
            if (element.Type == JTokenType.Null)
            {
                elements.Add(LanguageConstants.Null);
            }
            else if (element.Type == JTokenType.Comment)
            {
                continue;
            }
            else if (TypeHelper.TryCreateTypeLiteral(element) is { } literal)
            {
                elements.Add(literal);
            }
            else
            {
                // TryCreateTypeLiteral is exhaustive, so this should never be reached
                return ErrorType.Create(DiagnosticBuilder.ForDocumentStart().TypeExpressionLiteralConversionFailed());
            }
        }

        return new TypedArrayType(TypeHelper.CreateTypeUnion(elements), default, schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value);
    }

    private static (ITypeReference type, string typeName) GetDeferrableTypeInfo(SchemaValidationContext context, ITemplateSchemaNode schemaNode)
        => schemaNode.Ref?.Value switch
        {
            string @ref => (new DeferredTypeReference(() => ToTypeReference(context, schemaNode).Type), @ref.Replace("#/definitions/", "")),
            _ => ToTypeReference(context, schemaNode).Type switch { TypeSymbol concreteType => (concreteType, concreteType.Name) },
        };

    private static TypeSymbol GetObjectType(SchemaValidationContext context, ITemplateSchemaNode schemaNode, TypeSymbolValidationFlags flags)
    {
        if (schemaNode.AllowedValues?.Value is JArray jArray)
        {
            return TryGetLiteralUnionType(jArray, t => t.Type == JTokenType.Object, b => b.InvalidUnionTypeMember(LanguageConstants.ObjectType));
        }

        if (schemaNode.Discriminator is { } discriminator)
        {
            var variants = ImmutableArray.CreateBuilder<TypeSymbol>(discriminator.Mapping.Count);
            foreach (var mappingEntry in discriminator.Mapping)
            {
                var variant = TemplateEngine.ResolveSchemaReferences(context, mappingEntry.Value);
                variants.Add(TryGetResourceDerivedType(context, variant, flags) is UnresolvedResourceDerivedType resourceDerivedObject
                    ? new UnresolvedResourceDerivedPartialObjectType(resourceDerivedObject.TypeReference,
                        resourceDerivedObject.PointerSegments,
                        resourceDerivedObject.Variant,
                        discriminator.PropertyName.Value,
                        mappingEntry.Key)
                    : GetObjectType(
                        context: context,
                        properties: schemaNode.Properties.CoalesceEnumerable().Concat(variant.Properties.CoalesceEnumerable()),
                        requiredProperties: (schemaNode.Required?.Value ?? []).Concat(variant.Required?.Value ?? []),
                        additionalProperties: variant.AdditionalProperties ?? schemaNode.AdditionalProperties,
                        flags));
            }
            return new DiscriminatedObjectType(string.Join(" | ", TypeHelper.GetOrderedTypeNames(variants)), flags, discriminator.PropertyName.Value, variants);
        }

        return GetObjectType(context, schemaNode.Properties, schemaNode.Required?.Value, schemaNode.AdditionalProperties, flags);
    }

    private static TypeSymbol GetObjectType(SchemaValidationContext context,
        IEnumerable<KeyValuePair<string, TemplateTypeDefinition>>? properties,
        IEnumerable<string>? requiredProperties,
        TemplateBooleanOrSchemaNode? additionalProperties,
        TypeSymbolValidationFlags flags)
    {
        static string? GetDescriptionFromMetadata(TemplateGenericProperty<JToken>? metadataNode)
        {
            return metadataNode?.Value is JObject metadataObject &&
                    metadataObject.TryGetValue(LanguageConstants.MetadataDescriptionPropertyName, out var descriptionToken) &&
                    descriptionToken is JValue { Value: string descriptionString }
                ? descriptionString
                : null;
        }

        var requiredProps = requiredProperties is not null ? ImmutableHashSet.CreateRange(requiredProperties) : null;

        ObjectTypeNameBuilder nameBuilder = new();
        List<NamedTypeProperty>? propertyList = null;
        ITypeReference? additionalPropertiesType = LanguageConstants.Any;
        string? additionalPropertiesDescription = null;
        TypePropertyFlags additionalPropertiesFlags = TypePropertyFlags.FallbackProperty;

        if (properties is not null)
        {
            propertyList = new();
            foreach (var (propertyName, schema) in properties)
            {
                // depending on the language version, either only properties included in schemaNode.Required are required,
                // or all of them are (but some may be nullable)
                var required = context.TemplateLanguageVersion?.HasFeature(TemplateLanguageFeature.NullableParameters) == true
                    || (requiredProps?.Contains(propertyName) ?? false);
                var propertyFlags = required ? TypePropertyFlags.Required : TypePropertyFlags.None;
                var description = GetDescriptionFromMetadata(schema.Metadata);

                var (type, typeName) = GetDeferrableTypeInfo(context, schema);
                propertyList.Add(new(propertyName, type, propertyFlags, description));
                nameBuilder.AppendProperty(propertyName, typeName);
            }
        }

        if (additionalProperties is not null)
        {
            additionalPropertiesFlags = TypePropertyFlags.None;

            if (additionalProperties.SchemaNode is { } additionalPropertiesSchema)
            {
                var (type, typeName) = GetDeferrableTypeInfo(context, additionalPropertiesSchema);
                additionalPropertiesType = type;
                additionalPropertiesDescription = GetDescriptionFromMetadata(additionalPropertiesSchema.Metadata);

                nameBuilder.AppendPropertyMatcher(typeName);
            }
            else if (additionalProperties.BooleanValue == false)
            {
                additionalPropertiesType = null;
            }
        }

        if (propertyList is null && additionalProperties is null)
        {
            return flags.HasFlag(TypeSymbolValidationFlags.IsSecure) ? LanguageConstants.SecureObject : LanguageConstants.Object;
        }

        return new ObjectType(nameBuilder.ToString(), flags, propertyList.CoalesceEnumerable(), additionalPropertiesType is not null ? new(additionalPropertiesType, additionalPropertiesFlags, additionalPropertiesDescription) : null);
    }

    private class SansMetadata : ITemplateSchemaNode
    {
        private readonly ITemplateSchemaNode decorated;

        internal SansMetadata(ITemplateSchemaNode toDecorate)
        {
            this.decorated = toDecorate;
        }

        TemplateGenericProperty<JToken>? ITemplateSchemaNode.Metadata => null;

        TemplateGenericProperty<TemplateParameterType>? ITemplateSchemaNode.Type => decorated.Type;

        TemplateGenericProperty<string>? ITemplateSchemaNode.Ref => decorated.Ref;

        TemplateGenericProperty<JArray>? ITemplateSchemaNode.AllowedValues => decorated.AllowedValues;

        TemplateGenericProperty<bool>? ITemplateSchemaNode.Nullable => decorated.Nullable;

        TemplateGenericProperty<long>? ITemplateSchemaNode.MinValue => decorated.MinValue;

        TemplateGenericProperty<long>? ITemplateSchemaNode.MaxValue => decorated.MaxValue;

        TemplateGenericProperty<string>? ITemplateSchemaNode.Pattern => decorated.Pattern;

        TemplateGenericProperty<long>? ITemplateSchemaNode.MinLength => decorated.MinLength;

        TemplateGenericProperty<long>? ITemplateSchemaNode.MaxLength => decorated.MaxLength;

        TemplateTypeDefinition[]? ITemplateSchemaNode.PrefixItems => decorated.PrefixItems;

        TemplateBooleanOrSchemaNode? ITemplateSchemaNode.Items => decorated.Items;

        InsensitiveDictionary<TemplateTypeDefinition>? ITemplateSchemaNode.Properties => decorated.Properties;

        TemplateGenericProperty<string[]>? ITemplateSchemaNode.Required => decorated.Required;

        TemplateBooleanOrSchemaNode? ITemplateSchemaNode.AdditionalProperties => decorated.AdditionalProperties;

        TemplateGenericProperty<bool>? ITemplateSchemaNode.Sealed => decorated.Sealed;

        DiscriminatorConstraintDefinition? ITemplateSchemaNode.Discriminator => decorated.Discriminator;

        TemplateGenericProperty<string>[]? ITemplateSchemaNode.Validate => decorated.Validate;
    }
}
