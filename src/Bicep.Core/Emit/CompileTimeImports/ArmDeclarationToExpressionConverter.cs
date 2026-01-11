// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Extensions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit.CompileTimeImports;

internal class ArmDeclarationToExpressionConverter
{
    private const string VariablesFunctionName = "variables";
    private const string CopyIndexFunctionName = "copyindex";

    private readonly SchemaValidationContext schemaContext;
    private readonly ImmutableDictionary<string, TemplateFunction> functionsByFullyQualifiedName;
    private readonly TemplateVariablesEvaluator variablesEvaluator;
    private readonly ImmutableDictionary<ArmIdentifier, string> armIdentifierToSymbolNameMapping;
    private readonly SyntaxBase? sourceSyntax;
    private string? activeCopyLoopName;

    internal ArmDeclarationToExpressionConverter(Template template,
        ImmutableDictionary<ArmIdentifier, string> armIdentifierToSymbolNameMapping,
        SyntaxBase? sourceSyntax) : this(SchemaValidationContext.ForTemplate(template),
            template.GetFunctionDefinitions().ToImmutableDictionary(fd => fd.Key, fd => fd.Function, StringComparer.OrdinalIgnoreCase),
            new(template),
            armIdentifierToSymbolNameMapping,
            sourceSyntax)
    { }

    private ArmDeclarationToExpressionConverter(SchemaValidationContext schemaContext,
        ImmutableDictionary<string, TemplateFunction> functionsByFullyQualifiedName,
        TemplateVariablesEvaluator variablesEvaluator,
        ImmutableDictionary<ArmIdentifier, string> armIdentifierToSymbolNameMapping,
        SyntaxBase? sourceSyntax)
    {
        this.schemaContext = schemaContext;
        this.functionsByFullyQualifiedName = functionsByFullyQualifiedName;
        this.variablesEvaluator = variablesEvaluator;
        this.armIdentifierToSymbolNameMapping = armIdentifierToSymbolNameMapping;
        this.sourceSyntax = sourceSyntax;
    }

    internal ArmDeclarationToExpressionConverter WithSourceSyntax(SyntaxBase? sourceSyntax) => new(schemaContext,
        functionsByFullyQualifiedName,
        variablesEvaluator,
        armIdentifierToSymbolNameMapping,
        sourceSyntax);

    internal DeclaredTypeExpression CreateDeclaredTypeExpressionFor(string typePointer)
        => CreateDeclaredTypeExpressionFor(armIdentifierToSymbolNameMapping[new(ArmSymbolType.Type, typePointer)], ArmTemplateHelpers.DereferenceArmType(schemaContext, typePointer));

    internal DeclaredVariableExpression CreateDeclaredVariableExpressionFor(string originalName)
        => new(sourceSyntax,
            armIdentifierToSymbolNameMapping[new(ArmSymbolType.Variable, originalName)],
            Type: null,
            ConvertToVariableValue(originalName),
            // Variables cannot have descriptions in an ARM template -- this is only supported in Bicep
            Description: null,
            // An imported variable is never automatically re-exported
            Exported: null);

    internal DeclaredFunctionExpression CreateDeclaredFunctionExpressionFor(string fullyQualifiedFunctionName)
    {
        var convertedSymbolName = armIdentifierToSymbolNameMapping[new(ArmSymbolType.Function, fullyQualifiedFunctionName)];
        var (namespaceName, functionName) = convertedSymbolName.IndexOf('.') switch
        {
            int delimiterIndex when delimiterIndex > -1 => (convertedSymbolName[..delimiterIndex], convertedSymbolName[(delimiterIndex + 1)..]),
            _ => (EmitConstants.UserDefinedFunctionsNamespace, convertedSymbolName),
        };

        if (!functionsByFullyQualifiedName.TryGetValue(fullyQualifiedFunctionName, out var function))
        {
            throw new InvalidOperationException($"Function {fullyQualifiedFunctionName} was not found in template.");
        }

        return new(sourceSyntax,
            namespaceName,
            functionName,
            CreateLambdaExpressionFor(function),
            Description: function.Metadata?.Value is JObject @object &&
                @object.TryGetValue(LanguageConstants.MetadataDescriptionPropertyName, StringComparison.OrdinalIgnoreCase, out var descriptionToken) &&
                descriptionToken is JValue { Value: string description }
                    ? ExpressionFactory.CreateStringLiteral(description, sourceSyntax)
                    : null,
            // An imported function is never automatically re-exported
            Exported: null);
    }

    internal Expression ConvertToExpression(LanguageExpression armExpression) => armExpression switch
    {
        JTokenExpression jTokenExpression => ConvertToExpression(ImmutableDictionary<JToken, LanguageExpression>.Empty, jTokenExpression.Value),
        FunctionExpression functionExpression => ConvertToExpression(functionExpression),
        _ => throw new InvalidOperationException($"Encountered an unrecognized LanguageExpression of type {armExpression.GetType().Name}"),
    };

    #region typeConversion
    private DeclaredTypeExpression CreateDeclaredTypeExpressionFor(string convertedSymbolName, ITemplateSchemaNode schemaNode)
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

    private TypeExpression ConvertToTypeExpression(ITemplateSchemaNode schemaNode) => schemaNode.Nullable?.Value switch
    {
        true => new NullableTypeExpression(sourceSyntax, ConvertToTypeExpressionIgnoringNullability(schemaNode)),
        false => new NonNullableTypeExpression(sourceSyntax, ConvertToTypeExpressionIgnoringNullability(schemaNode)),
        _ => ConvertToTypeExpressionIgnoringNullability(schemaNode),
    };

    private record TypeModifiers(Expression? Description,
        Expression? Metadata,
        Expression? Secure,
        Expression? MinLength,
        Expression? MaxLength,
        Expression? MinValue,
        Expression? MaxValue,
        Expression? Sealed);

    private TypeModifiers GetTypeModifiers(ITemplateSchemaNode schemaNode) => new(
        Description: GetDescription(schemaNode) is string description ? ExpressionFactory.CreateStringLiteral(description, sourceSyntax) : null,
        Metadata: schemaNode.Metadata?.Value is JToken md && ConvertToExpression(ImmutableDictionary<JToken, LanguageExpression>.Empty, md) is ObjectExpression @object
            ? ExcludingPropertiesNamed(@object, LanguageConstants.MetadataDescriptionPropertyName, LanguageConstants.MetadataExportedPropertyName)
            : null,
        Secure: schemaNode.Type?.Value switch
        {
            TemplateParameterType.SecureString or TemplateParameterType.SecureObject => ExpressionFactory.CreateBooleanLiteral(true, sourceSyntax),
            _ => null,
        },
        MinLength: schemaNode.MinLength?.Value is long minLength
            ? ExpressionFactory.CreateIntegerLiteral(minLength, sourceSyntax)
            : null,
        MaxLength: schemaNode.MaxLength?.Value is long maxLength
            ? ExpressionFactory.CreateIntegerLiteral(maxLength, sourceSyntax)
            : null,
        MinValue: schemaNode.MinValue?.Value is long minValue
            ? ExpressionFactory.CreateIntegerLiteral(minValue, sourceSyntax)
            : null,
        MaxValue: schemaNode.MaxValue?.Value is long maxValue
            ? ExpressionFactory.CreateIntegerLiteral(maxValue, sourceSyntax)
            : null,
        Sealed: schemaNode.AdditionalProperties?.BooleanValue is false
            ? ExpressionFactory.CreateBooleanLiteral(true, sourceSyntax)
            : null);

    private TypeExpression ConvertToTypeExpressionIgnoringNullability(ITemplateSchemaNode schemaNode)
    {
        // drop constraints that would have already been processed: Nullable, Min/MaxValue, Min/MaxLength, Pattern
        var bicepType = ArmTemplateTypeLoader.ToTypeReference(schemaContext, new TemplateTypeDefinition
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
        }).Type;

        if (schemaNode.AllowedValues?.Value is { } allowedValues)
        {
            // "allowedValues" in ARM has two meanings:
            //   1. { "type": "array", "allowedValues": ["foo", "bar"] } means that the node permits an array containing any subset of the allowed values
            //   2. { "type": <type>, "allowedValues": [<legal values for <type>>] } means that the node permits any single one of the allowed values

            // in either case, the contents of allowedValues will be the members of a literal type union, and each item must therefore be a valid type expression
            var memberTypes = allowedValues.Select(ConvertAllowedValueToTypeExpression).ToImmutableArray();

            // If we've hit the former case, the Bicep type symbol for the node will be an ArrayType whose item type is a union (e.g., ('foo' | 'bar')[])
            if (bicepType is ArrayType bicepArrayType)
            {
                var itemTypeExpression = GetAllowedValuesTypeExpression(memberTypes, bicepArrayType.Item.Type);

                return new ArrayTypeExpression(sourceSyntax,
                    new TypedArrayType(itemTypeExpression.ExpressedType, TypeSymbolValidationFlags.Default),
                    itemTypeExpression);
            }

            // If we've hit the latter case, the Bicep type symbol for the node will be a UnionType
            return GetAllowedValuesTypeExpression(memberTypes, bicepType);
        }

        if (schemaNode.Ref?.Value is string @ref)
        {
            return new SynthesizedTypeAliasReferenceExpression(sourceSyntax, armIdentifierToSymbolNameMapping[new(ArmSymbolType.Type, @ref)], bicepType);
        }

        return schemaNode.Type?.Value switch
        {
            TemplateParameterType.String or
            TemplateParameterType.SecureString => new AmbientTypeReferenceExpression(sourceSyntax, LanguageConstants.TypeNameString, bicepType),
            TemplateParameterType.Int => new AmbientTypeReferenceExpression(sourceSyntax, LanguageConstants.TypeNameInt, bicepType),
            TemplateParameterType.Bool => new AmbientTypeReferenceExpression(sourceSyntax, LanguageConstants.TypeNameBool, bicepType),
            TemplateParameterType.Array => ConvertArrayNodeToTypeExpression(schemaNode),
            TemplateParameterType.Object or
            TemplateParameterType.SecureObject => ConvertObjectNodeToTypeExpression(schemaNode),
            _ => throw new InvalidOperationException($"Unrecognized ARM template type: {schemaNode.Type?.Value}"),
        };
    }

    private TypeExpression GetAllowedValuesTypeExpression(ImmutableArray<TypeExpression> memberTypes, TypeSymbol bicepType)
    {
        if (memberTypes.Length == 1)
        {
            return memberTypes.Single();
        }

        if (bicepType is not UnionType bicepUnionType)
        {
            throw new InvalidOperationException("This should have been handled by ArmTemplateTypeLoader");
        }

        return new UnionTypeExpression(sourceSyntax, bicepUnionType, memberTypes);
    }

    private static string? GetDescription(ITemplateSchemaNode schemaNode)
        => GetMetadataProperty(schemaNode, LanguageConstants.MetadataDescriptionPropertyName) is JValue { Value: string description }
            ? description
            : null;

    private static JToken? GetMetadataProperty(ITemplateSchemaNode schemaNode, string propertyName)
        => (schemaNode.Metadata?.Value as JObject)?.TryGetValue(propertyName, StringComparison.InvariantCultureIgnoreCase, out var property) == true
            ? property
            : null;

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
                propertyExpressions.Select(p => new NamedTypeProperty(p.PropertyName, p.Value.ExpressedType))),
            propertyExpressions,
            AdditionalPropertiesExpression: null);
    }

    private TupleTypeExpression ConvertAllowedValueToTypeExpression(JArray @array)
    {
        var itemExpressions = @array.Select(childToken => new TupleTypeItemExpression(sourceSyntax, ConvertAllowedValueToTypeExpression(childToken)))
            .ToImmutableArray();

        return new(sourceSyntax,
            new([.. itemExpressions.Select(i => i.Value.ExpressedType)], TypeSymbolValidationFlags.Default),
            itemExpressions);
    }

    private TypeExpression ConvertArrayNodeToTypeExpression(ITemplateSchemaNode schemaNode)
    {
        if (schemaNode.PrefixItems is { } prefixItems)
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
                new([.. itemExpressions.Select(i => i.Value.ExpressedType)],
                    TypeSymbolValidationFlags.Default),
                itemExpressions);
        }

        if (schemaNode.Items?.SchemaNode is { } itemsSchemaNode)
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
            .Select(kvp =>
            {
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
        if (schemaNode.AdditionalProperties?.SchemaNode is { } additionalPropertiesSchema)
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

        if (schemaNode.Discriminator is { } discriminatorConstraint)
        {
            var unionMembers = discriminatorConstraint.Mapping.OrderByAscendingOrdinalInsensitively(kvp => kvp.Key)
                .Select(kvp => ConvertToTypeExpression(kvp.Value))
                .ToImmutableArray();

            return new DiscriminatedObjectTypeExpression(sourceSyntax,
                new(string.Empty,
                    TypeSymbolValidationFlags.Default,
                    discriminatorConstraint.PropertyName.Value,
                    unionMembers.Select(expression => expression.ExpressedType)),
                unionMembers);
        }

        return new ObjectTypeExpression(sourceSyntax,
            new(string.Empty,
                TypeSymbolValidationFlags.Default,
                properties.Select(pe => new NamedTypeProperty(pe.PropertyName, pe.Value.ExpressedType)),
                addlProperties is not null
                    ? new TypeProperty(addlProperties.Value.ExpressedType, Description: addlProperties.Description is StringLiteralExpression stringLiteral ? stringLiteral.Value : null)
                    : null
            ),
            properties,
            addlProperties);
    }

    private ObjectExpression? ExcludingPropertiesNamed(ObjectExpression @object, params string[] keysToExclude)
    {
        var toExclude = keysToExclude.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

        return @object.Properties.Where(p => p.TryGetKeyText() is not string key || !toExclude.Contains(key)).ToImmutableArray() switch
        {
            var props when props.Length == 0 => null,
            var props when props.Length == @object.Properties.Length => @object,
            var props => new(@object.SourceSyntax, props),
        };
    }
    #endregion typeConversion

    #region variableConversion
    private Expression ConvertToVariableValue(string originalName)
    {
        if (variablesEvaluator.TryGetUnevaluatedDeclaringToken(originalName) is JToken singularDeclaration)
        {
            return ConvertToExpression(ExpressionsEngine.ParseLanguageExpressionsRecursive(singularDeclaration), singularDeclaration);
        }

        if (variablesEvaluator.TryGetUnevaluatedCopyDeclaration(originalName) is not { } copyDeclaration)
        {
            throw new InvalidOperationException($"Variable {originalName} was not found in template.");
        }

        this.activeCopyLoopName = originalName;
        var expression = new ForLoopExpression(sourceSyntax,
            new FunctionCallExpression(sourceSyntax, "range",
            [
                ExpressionFactory.CreateIntegerLiteral(0, sourceSyntax),
                ConvertToExpression(ExpressionsEngine.ParseLanguageExpressionsRecursive(copyDeclaration.CountToken), copyDeclaration.CountToken),
            ]),
            ConvertToExpression(ExpressionsEngine.ParseLanguageExpressionsRecursive(copyDeclaration.ValueItemToken), copyDeclaration.ValueItemToken),
            null,
            null);
        this.activeCopyLoopName = null;
        return expression;
    }

    private Expression ConvertToExpression(IReadOnlyDictionary<JToken, LanguageExpression> parsedExpressions, JToken toConvert)
    {
        ObjectPropertyExpression convertObjectProperty(JProperty property)
        {
            // The ExpressionsEngine.ParseLanguageExpressionsRecursive method represent key name lookups
            // by storing the JProperty, rather than the key string.
            var key = parsedExpressions.TryGetValue(property, out var keyExpression) ?
                ConvertToExpression(keyExpression) :
                ConvertToExpression(parsedExpressions, property.Name);

            return new(
                sourceSyntax,
                key,
                ConvertToExpression(parsedExpressions, property.Value));
        }


        if (parsedExpressions.TryGetValue(toConvert, out var armExpression))
        {
            return ConvertToExpression(armExpression);
        }

        return toConvert switch
        {
            JObject objectToCovert => ExpressionFactory.CreateObject(
                objectToCovert.Properties().Select(convertObjectProperty),
                sourceSyntax),
            JArray arrayToConvert => ExpressionFactory.CreateArray(
                arrayToConvert.Select(item => ConvertToExpression(parsedExpressions, item)),
                sourceSyntax),
            _ => toConvert.Type switch
            {
                JTokenType.Integer => ExpressionFactory.CreateIntegerLiteral(toConvert.ToObject<long>(), sourceSyntax),
                // there's no Bicep expression that corresponds to .Float, so use a `json('<float>')` function expression
                JTokenType.Float => new FunctionCallExpression(sourceSyntax,
                    "json",
                    [ExpressionFactory.CreateStringLiteral(toConvert.ToString())]),
                JTokenType.Boolean => ExpressionFactory.CreateBooleanLiteral(toConvert.ToObject<bool>(), sourceSyntax),
                JTokenType.Null => new NullLiteralExpression(sourceSyntax),
                // everything else (.String, .Date, .Uri, etc.) is some specialization of string
                _ => ExpressionFactory.CreateStringLiteral(toConvert.ToString(), sourceSyntax),
            },
        };
    }

    private Expression ConvertToExpression(FunctionExpression func)
    {
        if (func.Properties?.LastOrDefault() is LanguageExpression outermostPropertyAccess)
        {
            var baseExpression = ConvertToExpression(new FunctionExpression(func.Function,
                func.Parameters,
                [.. func.Properties.Take(func.Properties.Length - 1)]));

            return ConvertToExpression(outermostPropertyAccess) switch
            {
                StringLiteralExpression stringLiteralPropertyName when Lexer.IsValidIdentifier(stringLiteralPropertyName.Value) => new PropertyAccessExpression(sourceSyntax, baseExpression, stringLiteralPropertyName.Value, AccessExpressionFlags.None),
                Expression otherwise => new ArrayAccessExpression(sourceSyntax, baseExpression, otherwise, AccessExpressionFlags.None),
            };
        }

        if (armIdentifierToSymbolNameMapping.TryGetValue(new(ArmSymbolType.Function, func.Function), out var userDefinedFunctionName))
        {
            var (namespaceName, functionName) = userDefinedFunctionName.IndexOf('.') switch
            {
                int delimiterIndex when delimiterIndex > -1 => (userDefinedFunctionName[..delimiterIndex], userDefinedFunctionName[(delimiterIndex + 1)..]),
                _ => (EmitConstants.UserDefinedFunctionsNamespace, userDefinedFunctionName),
            };

            return new SynthesizedUserDefinedFunctionCallExpression(sourceSyntax,
                namespaceName,
                functionName,
                [.. func.Parameters.Select(ConvertToExpression)]);
        }

        return func.Function.ToLowerInvariant() switch
        {
            VariablesFunctionName => ConvertToExpression(func.Parameters.Single()) switch
            {
                StringLiteralExpression constantVariableName => new SynthesizedVariableReferenceExpression(sourceSyntax,
                    armIdentifierToSymbolNameMapping[new(ArmSymbolType.Variable, constantVariableName.Value)]),
                // if the argument to variables() was itself a runtime-evaluated expression, just treat this as a function call
                Expression otherwise => new FunctionCallExpression(sourceSyntax, VariablesFunctionName, [otherwise]),
            },
            CopyIndexFunctionName when variablesEvaluator.TryEvaluate(func.Parameters[0]) is JValue { Value: string copyIndexName } &&
                StringComparer.OrdinalIgnoreCase.Equals(activeCopyLoopName, copyIndexName) => func.Parameters.Skip(1).FirstOrDefault() switch
                {
                    LanguageExpression startIndexExpression => new BinaryExpression(sourceSyntax,
                        BinaryOperator.Add,
                        new CopyIndexExpression(sourceSyntax, armIdentifierToSymbolNameMapping[new(ArmSymbolType.Variable, activeCopyLoopName)]),
                        ConvertToExpression(startIndexExpression)),
                    _ => new CopyIndexExpression(sourceSyntax, armIdentifierToSymbolNameMapping[new(ArmSymbolType.Variable, activeCopyLoopName)]),
                },
            // this is less robust than decompilation analysis (e.g., the "add" function will not be transformed to a binary expression), but since this expression is produced only to be lightly manipulated and recompiled to ARM JSON, it's fine to be lax here
            // this choice should be revisited if this converter is used outside of the Bicep.Core.Emit namespace
            _ => new FunctionCallExpression(sourceSyntax, func.Function, [.. func.Parameters.Select(ConvertToExpression)]),
        };
    }
    #endregion variableConversion

    #region functionConversion
    private LambdaExpression CreateLambdaExpressionFor(TemplateFunction function)
    {
        var parameterNames = ImmutableArray.CreateBuilder<string>();
        var parameterTypes = ImmutableArray.CreateBuilder<TypeExpression?>();

        foreach (var functionParameter in function.Parameters.CoalesceEnumerable())
        {
            parameterNames.Add(functionParameter.Name.Value);
            parameterTypes.Add(ConvertToTypeExpression(functionParameter));
        }

        return new(sourceSyntax,
            parameterNames.ToImmutable(),
            parameterTypes.ToImmutable(),
            ConvertToExpression(ExpressionsEngine.ParseLanguageExpressionsRecursive(function.Output.Value.Value), function.Output.Value.Value),
            ConvertToTypeExpression(function.Output));
    }
    #endregion functionConversion
}
