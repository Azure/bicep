// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit.CompileTimeImports;

internal class ArmTypeToExpressionConverter
{
    private readonly SchemaValidationContext context;
    private readonly ImmutableDictionary<string, string> typePointerToSymbolNameMapping;
    private readonly SyntaxBase? sourceSyntax;

    internal ArmTypeToExpressionConverter(SchemaValidationContext context, ImmutableDictionary<string, string> typePointerToSymbolNameMapping, SyntaxBase? sourceSyntax)
    {
        this.context = context;
        this.typePointerToSymbolNameMapping = typePointerToSymbolNameMapping;
        this.sourceSyntax = sourceSyntax;
    }

    internal DeclaredTypeExpression ConvertToExpression(string convertedSymbolName, string typePointer)
        => ConvertToExpression(convertedSymbolName, ArmTemplateHelpers.DereferenceArmType(context, typePointer));

    private DeclaredTypeExpression ConvertToExpression(string convertedSymbolName, ITemplateSchemaNode schemaNode)
    {
        var modifiers = GetTypeModifiers(schemaNode);

        return new(sourceSyntax,
            convertedSymbolName,
            ConvertToTypeExpression(schemaNode),
            modifiers.Description,
            modifiers.Metadata,
            modifiers.Secure,
            modifiers.MinLength,
            modifiers.MaxLength,
            modifiers.MinValue,
            modifiers.MaxValue,
            modifiers.Sealed,
            // An imported type is never automatically re-exported
            Exported: null);
    }

    private record TypeModifiers(Expression? Description,
        Expression? Metadata,
        Expression? Secure,
        Expression? MinLength,
        Expression? MaxLength,
        Expression? MinValue,
        Expression? MaxValue,
        Expression? Sealed) {}

    private TypeModifiers GetTypeModifiers(ITemplateSchemaNode schemaNode) => new(
        GetDescription(schemaNode) is string description ? ExpressionFactory.CreateStringLiteral(description, sourceSyntax) : null,
        schemaNode.Metadata?.Value is JObject @object &&
            ConvertToExpression(@object, LanguageConstants.MetadataDescriptionPropertyName, LanguageConstants.MetadataExportedPropertyName) is {} metadataObjectExpression &&
            metadataObjectExpression.Properties.Any()
                ? metadataObjectExpression
                : null,
        schemaNode.Type?.Value switch
        {
            TemplateParameterType.SecureString or TemplateParameterType.SecureObject => ExpressionFactory.CreateBooleanLiteral(true, sourceSyntax),
            _ => null,
        },
        schemaNode.MinLength?.Value is long minLength
            ? ExpressionFactory.CreateIntegerLiteral(minLength, sourceSyntax)
            : null,
        schemaNode.MaxLength?.Value is long maxLength
            ? ExpressionFactory.CreateIntegerLiteral(maxLength, sourceSyntax)
            : null,
        schemaNode.MinValue?.Value is long minValue
            ? ExpressionFactory.CreateIntegerLiteral(minValue, sourceSyntax)
            : null,
        schemaNode.MaxValue?.Value is long maxValue
            ? ExpressionFactory.CreateIntegerLiteral(maxValue, sourceSyntax)
            : null,
        schemaNode.AdditionalProperties?.BooleanValue is false || schemaNode.Items?.BooleanValue is false
            ? ExpressionFactory.CreateBooleanLiteral(true, sourceSyntax)
            : null);

    private TypeExpression ConvertToTypeExpression(ITemplateSchemaNode schemaNode) => schemaNode.Nullable?.Value switch
    {
        true => new NullableTypeExpression(sourceSyntax, ConvertToTypeExpressionIgnoringNullability(schemaNode)),
        false => new NonNullableTypeExpression(sourceSyntax, ConvertToTypeExpressionIgnoringNullability(schemaNode)),
        _ => ConvertToTypeExpressionIgnoringNullability(schemaNode),
    };

    private TypeExpression ConvertToTypeExpressionIgnoringNullability(ITemplateSchemaNode schemaNode)
    {
        // drop constraints that would have already been processed: Nullable, Min/MaxValue, Min/MaxLength, Pattern
        var bicepType = ArmTemplateTypeLoader.ToTypeSymbol(context, new TemplateTypeDefinition
        {
            AdditionalProperties = schemaNode.AdditionalProperties,
            AllowedValues = schemaNode.AllowedValues,
            Discriminator = schemaNode.Discriminator,
            Items = schemaNode.Items,
            PrefixItems = schemaNode.PrefixItems,
            Properties = schemaNode.Properties,
            Ref = schemaNode.Ref,
            Required = schemaNode.Required,
            Sealed = schemaNode.Sealed,
            Type = schemaNode.Type,
        });

        if (schemaNode.AllowedValues?.Value is {} allowedValues)
        {
            // "allowedValues" in ARM has two meanings:
            //   1. { "type": "array", "allowedValues": ["foo", "bar"] } means that the node permits an array containing any subset of the allowed values
            //   2. { "type": <type>, "allowedValues": [<legal values for <type>>] } means that the node permits any single one of the allowed values

            // in either case, the contents of allowedValues will be the members of a literal type union, and each item must therefore be a valid type expression
            var memberTypes = allowedValues.Select(ConvertAllowedValueToTypeExpression).ToImmutableArray();

            // If we've hit the former case, the Bicep type symbol for the node will be an ArrayType whose item type is a union (e.g., ('foo' | 'bar')[])
            if (bicepType is ArrayType bicepArrayType)
            {
                if (bicepArrayType.Item.Type is not UnionType itemUnion)
                {
                    throw new InvalidOperationException("This should have been handled by ArmTemplateTypeLoader");
                }

                return new ArrayTypeExpression(sourceSyntax,
                    new TypedArrayType(itemUnion, TypeSymbolValidationFlags.Default),
                    new UnionTypeExpression(sourceSyntax, itemUnion, memberTypes));
            }

            // If we've hit the latter case, the Bicep type symbol for the node will be a UnionType
            if (bicepType is not UnionType bicepUnionType)
            {
                throw new InvalidOperationException("This should have been handled by ArmTemplateTypeLoader");
            }

            return new UnionTypeExpression(sourceSyntax, bicepUnionType, memberTypes);
        }

        if (schemaNode.Ref?.Value is string @ref)
        {
            return new TypeAliasReferenceExpression(sourceSyntax, typePointerToSymbolNameMapping[@ref], bicepType);
        }

        return schemaNode.Type.Value switch
        {
            TemplateParameterType.String or
            TemplateParameterType.SecureString => new AmbientTypeReferenceExpression(sourceSyntax, LanguageConstants.TypeNameString, bicepType),
            TemplateParameterType.Int => new AmbientTypeReferenceExpression(sourceSyntax, LanguageConstants.TypeNameInt, bicepType),
            TemplateParameterType.Bool => new AmbientTypeReferenceExpression(sourceSyntax, LanguageConstants.TypeNameBool, bicepType),
            TemplateParameterType.Array => ConvertArrayNodeToTypeExpression(schemaNode),
            TemplateParameterType.Object or
            TemplateParameterType.SecureObject => ConvertObjectNodeToTypeExpression(schemaNode),
            _ => throw new InvalidOperationException($"Unrecognized ARM template type: {schemaNode.Type.Value}"),
        };
    }

    private static string? GetDescription(ITemplateSchemaNode schemaNode)
        => GetMetadataProperty(schemaNode, LanguageConstants.MetadataDescriptionPropertyName) is JValue { Value: string description }
            ? description
            : null;

    private static JToken? GetMetadataProperty(ITemplateSchemaNode schemaNode, string propertyName)
        => (schemaNode.Metadata?.Value as JObject)?.TryGetValue(propertyName, StringComparison.InvariantCultureIgnoreCase, out var property) == true
            ? property
            : null;

    private Expression ConvertToExpression(JToken token) => token switch
    {
        JObject @object => ConvertToExpression(@object),
        JArray @array => ExpressionFactory.CreateArray(@array.Select(ConvertToExpression), sourceSyntax),
        JValue jValue => jValue.Value switch
        {
            string @string => ExpressionFactory.CreateStringLiteral(@string, sourceSyntax),
            long @long => ExpressionFactory.CreateIntegerLiteral(@long, sourceSyntax),
            bool @bool => ExpressionFactory.CreateBooleanLiteral(@bool, sourceSyntax),
            null => new NullLiteralExpression(sourceSyntax),
            _ => throw new InvalidOperationException($"Unrecognized JValue value of type {jValue.Value?.GetType().Name}"),
        },
        _ => throw new InvalidOperationException($"Unrecognized JToken of type {token.GetType().Name}")
    };

    private ObjectExpression ConvertToExpression(JObject @object, params string[] keysToExclude)
    {
        var properties = @object.Properties();
        if (keysToExclude.Length > 0)
        {
            properties = properties.Where(p => !keysToExclude.Contains(p.Name));
        }

        return ExpressionFactory.CreateObject(properties.Select(p => ExpressionFactory.CreateObjectProperty(p.Name, ConvertToExpression(p.Value), sourceSyntax)), sourceSyntax);
    }

    private TypeExpression ConvertAllowedValueToTypeExpression(JToken token) => token switch
    {
        JObject @object => ConvertAllowedValueToTypeExpression(@object),
        JArray @array => ConvertAllowedValueToTypeExpression(@array),
        JValue jValue => jValue.Value switch
        {
            string @string => new StringLiteralTypeExpression(sourceSyntax, TypeFactory.CreateStringLiteralType(@string)),
            long @long => new IntegerLiteralTypeExpression(sourceSyntax, TypeFactory.CreateIntegerLiteralType(@long)),
            bool @bool => new BooleanLiteralTypeExpression(sourceSyntax, TypeFactory.CreateBooleanLiteralType(@bool)),
            null => new NullLiteralTypeExpression(sourceSyntax, new NullType()),
            _ => throw new InvalidOperationException($"Unrecognized JValue value of type {jValue.Value?.GetType().Name}"),
        },
        _ => throw new InvalidOperationException($"Unrecognized JToken of type {token.GetType().Name}")
    };

    private ObjectTypeExpression ConvertAllowedValueToTypeExpression(JObject @object)
    {
        var propertyExpressions = @object.Properties()
            .Select(p => new ObjectTypePropertyExpression(sourceSyntax, p.Name, ConvertAllowedValueToTypeExpression(p.Value)))
            .ToImmutableArray();

        return new(sourceSyntax,
            new ObjectType(string.Empty,
                TypeSymbolValidationFlags.Default,
                propertyExpressions.Select(p => new TypeProperty(p.PropertyName, p.Value.ExpressedType)),
                additionalPropertiesType: null),
            propertyExpressions,
            AdditionalPropertiesExpression: null);
    }

    private TupleTypeExpression ConvertAllowedValueToTypeExpression(JArray @array)
    {
        var itemExpressions = @array.Select(childToken => new TupleTypeItemExpression(sourceSyntax, ConvertAllowedValueToTypeExpression(childToken)))
            .ToImmutableArray();

        return new(sourceSyntax,
            new(itemExpressions.Select(i => i.Value.ExpressedType).ToImmutableArray<ITypeReference>(), TypeSymbolValidationFlags.Default),
            itemExpressions);
    }

    private TypeExpression ConvertArrayNodeToTypeExpression(ITemplateSchemaNode schemaNode)
    {
        if (schemaNode.PrefixItems is {} prefixItems)
        {
            var itemExpressions = ImmutableArray.CreateRange(prefixItems.Select(i =>
            {
                var modifiers = GetTypeModifiers(i);
                return new TupleTypeItemExpression(sourceSyntax,
                    ConvertToTypeExpression(i),
                    modifiers.Description,
                    modifiers.Metadata,
                    modifiers.Secure,
                    modifiers.MinLength,
                    modifiers.MaxLength,
                    modifiers.MinValue,
                    modifiers.MaxValue,
                    modifiers.Sealed);
            }));

            return new TupleTypeExpression(sourceSyntax,
                new(ImmutableArray.CreateRange<ITypeReference>(itemExpressions.Select(i => i.Value.ExpressedType)),
                    TypeSymbolValidationFlags.Default),
                itemExpressions);
        }

        if (schemaNode.Items?.SchemaNode is {} itemsSchemaNode)
        {
            var itemTypeExpression = ConvertToTypeExpression(itemsSchemaNode);
            return new ArrayTypeExpression(sourceSyntax,
                new TypedArrayType(itemTypeExpression.ExpressedType, TypeSymbolValidationFlags.Default),
                itemTypeExpression);
        }

        return new AmbientTypeReferenceExpression(sourceSyntax, LanguageConstants.ArrayType, LanguageConstants.Array);
    }

    private TypeExpression ConvertObjectNodeToTypeExpression(ITemplateSchemaNode schemaNode)
    {
        if (schemaNode.Properties is null &&
            schemaNode.AdditionalProperties is null &&
            schemaNode.Sealed is null &&
            schemaNode.Discriminator is null)
        {
            return new AmbientTypeReferenceExpression(sourceSyntax, LanguageConstants.ObjectType, LanguageConstants.Object);
        }

        var properties = ImmutableArray.CreateRange(schemaNode.Properties.CoalesceEnumerable()
            .Select(kvp => {
                var modifiers = GetTypeModifiers(kvp.Value);
                return new ObjectTypePropertyExpression(sourceSyntax,
                    kvp.Key,
                    ConvertToTypeExpression(kvp.Value),
                    modifiers.Description,
                    modifiers.Metadata,
                    modifiers.Secure,
                    modifiers.MinLength,
                    modifiers.MaxLength,
                    modifiers.MinValue,
                    modifiers.MaxValue,
                    modifiers.Sealed);
            }));

        ObjectTypeAdditionalPropertiesExpression? addlProperties = null;
        if (schemaNode.AdditionalProperties?.SchemaNode is {} additionalPropertiesSchema)
        {
            var modifiers = GetTypeModifiers(additionalPropertiesSchema);
            addlProperties = new(sourceSyntax,
                ConvertToTypeExpression(additionalPropertiesSchema),
                modifiers.Description,
                modifiers.Metadata,
                modifiers.Secure,
                modifiers.MinLength,
                modifiers.MaxLength,
                modifiers.MinValue,
                modifiers.MaxValue,
                modifiers.Sealed);
        }

        return new ObjectTypeExpression(sourceSyntax,
            new(string.Empty,
                TypeSymbolValidationFlags.Default,
                properties.Select(pe => new TypeProperty(pe.PropertyName, pe.Value.ExpressedType)),
                addlProperties?.Value.ExpressedType),
            properties,
            addlProperties);
    }
}
