// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Expression.Extensions;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.Exceptions;
using Bicep.Core.Diagnostics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem;

public static class ArmTemplateTypeLoader
{
    public static TypeSymbol ToTypeSymbol(SchemaValidationContext context, ITemplateSchemaNode armTemplateSchemaNode, TypeSymbolValidationFlags flags = TypeSymbolValidationFlags.Default)
    {
        try
        {
            var resolved = TemplateEngine.ResolveSchemaReferences(context, armTemplateSchemaNode);

            if (resolved.Type.Value == TemplateParameterType.SecureString || resolved.Type.Value == TemplateParameterType.SecureObject)
            {
                flags = TypeSymbolValidationFlags.IsSecure | flags;
            }

            var bicepType = resolved.Type.Value switch
            {
                TemplateParameterType.String or
                TemplateParameterType.SecureString => GetStringType(resolved, flags),
                TemplateParameterType.Int => GetIntegerType(resolved, flags),
                TemplateParameterType.Bool => GetBooleanType(resolved, flags),
                TemplateParameterType.Array => GetArrayType(context, resolved),
                TemplateParameterType.Object or
                TemplateParameterType.SecureObject => GetObjectType(context, resolved, flags),
                _ => ErrorType.Empty(),
            };

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

    private static TypeSymbol GetStringType(ITemplateSchemaNode schemaNode, TypeSymbolValidationFlags flags)
    {
        if (schemaNode.AllowedValues?.Value is JArray jArray)
        {
            return TryGetLiteralUnionType(jArray, t => t.IsTextBasedJTokenType(), b => b.InvalidUnionTypeMember(LanguageConstants.TypeNameString));
        }

        return TypeFactory.CreateStringType(schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value, flags);
    }

    private static TypeSymbol TryGetLiteralUnionType(JArray allowedValues, Func<JToken, bool> validator, DiagnosticBuilder.ErrorBuilderDelegate diagnosticOnMismatch)
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

            return new TupleType(nameBuilder.ToString(), tupleMembers.ToImmutableArray(), default);
        }

        if (schemaNode.Items?.SchemaNode is { } items)
        {
            if (items.Ref?.Value is not null)
            {
                var (type, typeName) = GetDeferrableTypeInfo(context, items);
                return new TypedArrayType($"{typeName}[]", type, default, schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value);
            }

            return new TypedArrayType(ToTypeSymbol(context, items), default, schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value);
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
            string @ref => (new DeferredTypeReference(() => ToTypeSymbol(context, schemaNode)), @ref.Replace("#/definitions/", "")),
            _ => ToTypeSymbol(context, schemaNode) switch { TypeSymbol concreteType => (concreteType, concreteType.Name) },
        };

    private static TypeSymbol GetObjectType(SchemaValidationContext context, ITemplateSchemaNode schemaNode, TypeSymbolValidationFlags flags)
    {
        if (schemaNode.AllowedValues?.Value is JArray jArray)
        {
            return TryGetLiteralUnionType(jArray, t => t.Type == JTokenType.Object, b => b.InvalidUnionTypeMember(LanguageConstants.ObjectType));
        }

        ObjectTypeNameBuilder nameBuilder = new();
        List<TypeProperty> properties = new();
        ITypeReference? additionalPropertiesType = LanguageConstants.Any;
        TypePropertyFlags additionalPropertiesFlags = TypePropertyFlags.FallbackProperty;

        if (schemaNode.Properties is { } propertySchemata)
        {
            foreach (var (propertyName, schema) in propertySchemata)
            {
                // depending on the language version, either only properties included in schemaNode.Required are required,
                // or all of them are (but some may be nullable)
                var required = context.TemplateLanguageVersion?.HasFeature(TemplateLanguageFeature.NullableParameters) == true
                    || (schemaNode.Required?.Value.Contains(propertyName) ?? false);
                var propertyFlags = required ? TypePropertyFlags.Required : TypePropertyFlags.None;
                var description = schema.Metadata?.Value is JObject metadataObject &&
                    metadataObject.TryGetValue(LanguageConstants.MetadataDescriptionPropertyName, out var descriptionToken) &&
                    descriptionToken is JValue { Value: string descriptionString }
                        ? descriptionString
                        : null;

                var (type, typeName) = GetDeferrableTypeInfo(context, schema);

                properties.Add(new(propertyName, type, propertyFlags, description));
                nameBuilder.AppendProperty(propertyName, typeName);
            }
        }

        if (schemaNode.AdditionalProperties is { } addlProps)
        {
            additionalPropertiesFlags = TypePropertyFlags.None;

            if (addlProps.SchemaNode is { } additionalPropertiesSchema)
            {
                var typeInfo = GetDeferrableTypeInfo(context, additionalPropertiesSchema);
                additionalPropertiesType = typeInfo.type;
                nameBuilder.AppendPropertyMatcher("*", typeInfo.typeName);
            }
            else if (addlProps.BooleanValue == false)
            {
                additionalPropertiesType = null;
            }
        }

        if (properties.Count == 0 && schemaNode.AdditionalProperties is null)
        {
            return flags.HasFlag(TypeSymbolValidationFlags.IsSecure) ? LanguageConstants.SecureObject : LanguageConstants.Object;
        }

        return new ObjectType(nameBuilder.ToString(), flags, properties, additionalPropertiesType, additionalPropertiesFlags);
    }
}
