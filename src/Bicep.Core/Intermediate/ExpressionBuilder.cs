// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.DataFlow;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using static Bicep.Core.Emit.ScopeHelper;

namespace Bicep.Core.Intermediate;

public class ExpressionBuilder
{
    private static readonly ImmutableHashSet<string> NonAzResourcePropertiesToOmit = [
        LanguageConstants.ResourceDependsOnPropertyName,
    ];

    private static readonly ImmutableHashSet<string> AzResourcePropertiesToOmit = [
        AzResourceTypeProvider.ResourceNamePropertyName,
        LanguageConstants.ResourceScopePropertyName,
        LanguageConstants.ResourceParentPropertyName,
        LanguageConstants.ResourceDependsOnPropertyName,
    ];

    private static readonly ImmutableHashSet<string> ModulePropertiesToOmit = [
        AzResourceTypeProvider.ResourceNamePropertyName,
        LanguageConstants.ModuleParamsPropertyName,
        LanguageConstants.ModuleExtensionConfigsPropertyName,
        LanguageConstants.ResourceScopePropertyName,
        LanguageConstants.ResourceDependsOnPropertyName,
    ];

    private readonly ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements;

    public EmitterContext Context { get; }

    public ExpressionBuilder(EmitterContext Context, ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements)
    {
        this.Context = Context;
        this.localReplacements = localReplacements;
    }

    public ExpressionBuilder(EmitterContext Context)
        : this(Context, [])
    {
    }

    public Expression Convert(SyntaxBase syntax)
    {
        var expression = ConvertWithoutLowering(syntax);

        return ExpressionLoweringVisitor.Lower(expression);
    }

    private Expression ConvertWithoutLowering(SyntaxBase syntax)
    {
        switch (syntax)
        {
            case StringSyntax @string:
                {
                    if (@string.TryGetLiteralValue() is { } stringValue)
                    {
                        return new StringLiteralExpression(@string, stringValue);
                    }

                    return new InterpolatedStringExpression(
                        @string,
                        @string.SegmentValues,
                        [.. @string.Expressions.Select(ConvertWithoutLowering)]);
                }
            case IntegerLiteralSyntax @int:
                {
                    var literalValue = SafeConvertIntegerValue(@int, isNegative: false);
                    return new IntegerLiteralExpression(@int, literalValue);
                }
            case UnaryOperationSyntax { Operator: UnaryOperator.Minus } unary when unary.Expression is IntegerLiteralSyntax @int:
                {
                    var literalValue = SafeConvertIntegerValue(@int, isNegative: true);
                    return new IntegerLiteralExpression(unary, literalValue);
                }
            case BooleanLiteralSyntax @bool:
                return new BooleanLiteralExpression(@bool, @bool.Value);
            case NullLiteralSyntax:
                return new NullLiteralExpression(syntax);
            case ParenthesizedExpressionSyntax x:
                return ConvertWithoutLowering(x.Expression);
            case NonNullAssertionSyntax assertion:
                return ConvertWithoutLowering(assertion.BaseExpression);
            case ObjectSyntax @object:
                return ConvertObject(@object);
            case ArraySyntax array:
                return ConvertArray(array);
            case TernaryOperationSyntax ternary:
                return new TernaryExpression(
                    ternary,
                    ConvertWithoutLowering(ternary.ConditionExpression),
                    ConvertWithoutLowering(ternary.TrueExpression),
                    ConvertWithoutLowering(ternary.FalseExpression));
            case BinaryOperationSyntax binary:
                return new BinaryExpression(
                    binary,
                    binary.Operator,
                    ConvertWithoutLowering(binary.LeftExpression),
                    ConvertWithoutLowering(binary.RightExpression));
            case UnaryOperationSyntax unary:
                return new UnaryExpression(
                    unary,
                    unary.Operator,
                    ConvertWithoutLowering(unary.Expression));
            case FunctionCallSyntaxBase function:
                return ConvertFunction(function);
            case ArrayAccessSyntax arrayAccess:
                // TODO: should we lower an arrayAccess with constant property name to a propertyAccess?
                return ConvertArrayAccess(arrayAccess);
            case PropertyAccessSyntax propertyAccess:
                return ConvertPropertyAccess(propertyAccess);
            case VariableAccessSyntax variableAccess:
                return ConvertVariableAccess(variableAccess);
            case ResourceAccessSyntax resourceAccess:
                return ConvertResourceAccess(resourceAccess);
            case LambdaSyntax lambda:
                var variables = lambda.GetLocalVariables();

                return new LambdaExpression(
                    lambda,
                    [.. variables.Select(x => x.Name.IdentifierName)],
                    [.. variables.Select<LocalVariableSyntax, TypeExpression?>(x => null)],
                    ConvertWithoutLowering(lambda.Body),
                    null);
            case TypedLambdaSyntax lambda:
                var typedVariables = lambda.GetLocalVariables();

                return new LambdaExpression(
                    lambda,
                    [.. typedVariables.Select(x => x.Name.IdentifierName)],
                    [.. typedVariables.Select(x => ConvertTypeWithoutLowering(x.Type))],
                    ConvertWithoutLowering(lambda.Body),
                    ConvertTypeWithoutLowering(lambda.ReturnType));

            case ForSyntax forSyntax:
                return new ForLoopExpression(
                    forSyntax,
                    ConvertWithoutLowering(forSyntax.Expression),
                    ConvertWithoutLowering(forSyntax.Body),
                    null,
                    null);

            case IfConditionSyntax conditionSyntax:
                return new ConditionExpression(
                    conditionSyntax,
                    ConvertWithoutLowering(conditionSyntax.ConditionExpression),
                    ConvertWithoutLowering(conditionSyntax.Body));

            case MetadataDeclarationSyntax metadata:
                return EvaluateDecorators(metadata, new DeclaredMetadataExpression(
                    metadata,
                    metadata.Name.IdentifierName,
                    ConvertWithoutLowering(metadata.Value)));

            case ExtensionDeclarationSyntax extension:
                var symbol = GetDeclaredSymbol<ExtensionNamespaceSymbol>(extension);
                return EvaluateDecorators(extension, new ExtensionExpression(
                    extension,
                    symbol.Name,
                    GetTypeInfo<NamespaceType>(extension).Settings,
                    extension.Config is not null ? ConvertWithoutLowering(extension.Config) : null));

            case ParameterDeclarationSyntax parameter:
                return EvaluateDecorators(parameter, new DeclaredParameterExpression(
                    parameter,
                    parameter.Name.IdentifierName,
                    ConvertTypeWithoutLowering(parameter.Type),
                    parameter.Modifier is ParameterDefaultValueSyntax defaultValue ? ConvertWithoutLowering(defaultValue.DefaultValue) : null));

            case VariableDeclarationSyntax variable:
                return EvaluateDecorators(variable, new DeclaredVariableExpression(
                    variable,
                    variable.Name.IdentifierName,
                    variable.Type != null ? ConvertTypeWithoutLowering(variable.Type) : null,
                    ConvertWithoutLowering(variable.Value)));

            case FunctionDeclarationSyntax function:
                return EvaluateDecorators(function, new DeclaredFunctionExpression(
                    function,
                    EmitConstants.UserDefinedFunctionsNamespace,
                    function.Name.IdentifierName,
                    ConvertWithoutLowering(function.Lambda)));

            case OutputDeclarationSyntax output:
                return EvaluateDecorators(output, new DeclaredOutputExpression(
                    output,
                    output.Name.IdentifierName,
                    ConvertTypeWithoutLowering(output.Type),
                    ConvertWithoutLowering(output.Value)));

            case AssertDeclarationSyntax assert:
                return EvaluateDecorators(assert, new DeclaredAssertExpression(
                    assert,
                    assert.Name.IdentifierName,
                    ConvertWithoutLowering(assert.Value)));

            case ResourceDeclarationSyntax resource:
                return EvaluateDecorators(resource, ConvertResource(resource));

            case ModuleDeclarationSyntax module:
                return EvaluateDecorators(module, ConvertModule(module));

            case TypeDeclarationSyntax typeDeclaration:
                return EvaluateDecorators(typeDeclaration, new DeclaredTypeExpression(typeDeclaration,
                    typeDeclaration.Name.IdentifierName,
                    ConvertTypeWithoutLowering(typeDeclaration.Value)));

            case ObjectTypePropertySyntax typeProperty:
                return EvaluateDecorators(typeProperty, new ObjectTypePropertyExpression(typeProperty,
                    typeProperty.TryGetKeyText() ?? throw new ArgumentException("Unable to resolve name of object type property"),
                    ConvertTypeWithoutLowering(typeProperty.Value)));

            case ObjectTypeAdditionalPropertiesSyntax typeAdditionalProperties:
                return EvaluateDecorators(typeAdditionalProperties, new ObjectTypeAdditionalPropertiesExpression(typeAdditionalProperties,
                    ConvertTypeWithoutLowering(typeAdditionalProperties.Value)));

            case TupleTypeItemSyntax tupleItem:
                return EvaluateDecorators(tupleItem, new TupleTypeItemExpression(tupleItem, ConvertTypeWithoutLowering(tupleItem.Value)));

            case ProgramSyntax program:
                return ConvertProgram(program);

            default:
                throw new ArgumentException($"Failed to convert syntax of type {syntax.GetType()}");
        }
    }

    private TypeExpression ConvertTypeWithoutLowering(SyntaxBase syntax)
        => syntax switch
        {
            TypeVariableAccessSyntax variableAccess => Context.SemanticModel.Binder.GetSymbolInfo(syntax) switch
            {
                AmbientTypeSymbol ambientType => new AmbientTypeReferenceExpression(syntax, ambientType.Name, UnwrapType(ambientType.Type)),
                TypeAliasSymbol typeAlias => new TypeAliasReferenceExpression(syntax, typeAlias, UnwrapType(typeAlias.Type)),
                ImportedTypeSymbol importedSymbol => new ImportedTypeReferenceExpression(syntax, importedSymbol, UnwrapType(importedSymbol.Type)),
                Symbol otherwise => throw new ArgumentException($"Encountered unexpected symbol of type {otherwise.GetType()} in a type expression."),
                _ => throw new ArgumentException($"Unable to locate symbol for name '{variableAccess.Name.IdentifierName}'.")
            },
            StringTypeLiteralSyntax @string => new StringLiteralTypeExpression(@string, GetTypeInfo<StringLiteralType>(@string)),
            IntegerTypeLiteralSyntax @int => new IntegerLiteralTypeExpression(@int, GetTypeInfo<IntegerLiteralType>(@int)),
            BooleanTypeLiteralSyntax @bool => new BooleanLiteralTypeExpression(@bool, GetTypeInfo<BooleanLiteralType>(@bool)),
            UnaryTypeOperationSyntax unaryOperation => Context.SemanticModel.GetTypeInfo(unaryOperation) switch
            {
                IntegerLiteralType intOperation => new IntegerLiteralTypeExpression(syntax, intOperation),
                BooleanLiteralType boolOperation => new BooleanLiteralTypeExpression(syntax, boolOperation),
                _ => throw new ArgumentException($"Failed to convert syntax of type {syntax.GetType()}"),
            },
            NullTypeLiteralSyntax @null => new NullLiteralTypeExpression(@null, GetTypeInfo<NullType>(@null)),
            ResourceTypeSyntax resource => new ResourceTypeExpression(resource, GetTypeInfo<ResourceType>(resource)),
            ObjectTypeSyntax objectTypeSyntax => new ObjectTypeExpression(syntax,
                GetTypeInfo<ObjectType>(syntax),
                [.. objectTypeSyntax.Properties.Select(p => ConvertWithoutLowering<ObjectTypePropertyExpression>(p))],
                objectTypeSyntax.AdditionalProperties is SyntaxBase addlPropertiesSyntax
                    ? ConvertWithoutLowering<ObjectTypeAdditionalPropertiesExpression>(addlPropertiesSyntax)
                    : null),
            TupleTypeSyntax tupleTypeSyntax => new TupleTypeExpression(syntax,
                GetTypeInfo<TupleType>(syntax),
                [.. tupleTypeSyntax.Items.Select(i => ConvertWithoutLowering<TupleTypeItemExpression>(i))]),
            ArrayTypeSyntax arrayTypeSyntax => new ArrayTypeExpression(syntax,
                GetTypeInfo<ArrayType>(syntax),
                ConvertTypeWithoutLowering(arrayTypeSyntax.Item.Value)),
            NullableTypeSyntax nullableTypeSyntax => new NullableTypeExpression(syntax, ConvertTypeWithoutLowering(nullableTypeSyntax.Base)),
            UnionTypeSyntax unionTypeSyntax when Context.SemanticModel.GetTypeInfo(unionTypeSyntax) is DiscriminatedObjectType discriminatedObjectType =>
                new DiscriminatedObjectTypeExpression(
                    syntax,
                    discriminatedObjectType,
                    [.. unionTypeSyntax.Members.Select(m => ConvertTypeWithoutLowering(m.Value))]),
            UnionTypeSyntax unionTypeSyntax when Context.SemanticModel.GetTypeInfo(unionTypeSyntax) is UnionType unionType
                => new UnionTypeExpression(syntax, unionType, [.. unionTypeSyntax.Members.Select(m => ConvertTypeWithoutLowering(m.Value))]),
            UnionTypeSyntax unionTypeSyntax => Context.SemanticModel.GetTypeInfo(unionTypeSyntax) switch
            {
                ErrorType errorType => throw new ArgumentException($"Failed to convert syntax of type {syntax.GetType()}"),
                UnionType unionType => new UnionTypeExpression(syntax, unionType, [.. unionTypeSyntax.Members.Select(m => ConvertTypeWithoutLowering(m.Value))]),
                // If a union type expression's members all refer to the same literal value, the type of the expression will be a single literal rather than a union
                TypeSymbol otherwise => new UnionTypeExpression(syntax,
                    new UnionType(string.Empty, [otherwise]),
                    [.. unionTypeSyntax.Members.Select(m => ConvertTypeWithoutLowering(m.Value))]),
            },
            ParenthesizedTypeSyntax parenthesizedExpression => ConvertTypeWithoutLowering(parenthesizedExpression.Expression),
            NonNullableTypeSyntax nonNullableTypeSyntax => new NonNullableTypeExpression(nonNullableTypeSyntax, ConvertTypeWithoutLowering(nonNullableTypeSyntax.Base)),
            TypePropertyAccessSyntax propertyAccess => ConvertTypePropertyAccess(propertyAccess),
            TypeAdditionalPropertiesAccessSyntax additionalPropertiesAccess => ConvertTypeAdditionalPropertiesAccess(additionalPropertiesAccess),
            TypeArrayAccessSyntax arrayAccess => ConvertTypeArrayAccess(arrayAccess),
            TypeItemsAccessSyntax itemsAccess => ConvertTypeItemsAccess(itemsAccess),
            ParameterizedTypeInstantiationSyntaxBase parameterizedTypeInstantiation
                => Context.SemanticModel.TypeManager.TryGetReifiedType(parameterizedTypeInstantiation) is TypeExpression reified
                    ? reified
                    : throw new ArgumentException($"Failed to reify parameterized type invocation."),
            _ => throw new ArgumentException($"Failed to convert syntax of type {syntax.GetType()}"),
        };

    private static TypeSymbol UnwrapType(TypeSymbol type) => type switch
    {
        TypeType tt => tt.Unwrapped,
        _ => type,
    };

    private TypeExpression ConvertTypePropertyAccess(TypePropertyAccessSyntax syntax)
        => ConvertTypePropertyAccess(syntax, syntax.BaseExpression, syntax.PropertyName.IdentifierName);

    private TypeExpression ConvertTypePropertyAccess(SyntaxBase syntax, SyntaxBase baseExpression, string propertyName)
        => Context.SemanticModel.GetSymbolInfo(baseExpression) switch
        {
            BuiltInNamespaceSymbol builtIn => TryGetPropertyType(builtIn, propertyName) switch
            {
                TypeType typeType when builtIn is { Type: NamespaceType nsType } => new FullyQualifiedAmbientTypeReferenceExpression(syntax, nsType.ExtensionName, propertyName, typeType.Unwrapped),
                _ => throw new ArgumentException($"Property '{propertyName}' of symbol '{builtIn.Name}' was not found or was not valid."),
            },
            WildcardImportSymbol wildcardImport => TryGetPropertyType(wildcardImport, propertyName) switch
            {
                TypeType typeType => new WildcardImportTypePropertyReferenceExpression(syntax, wildcardImport, propertyName, typeType.Unwrapped),
                _ => throw new ArgumentException($"Property '{propertyName}' of symbol '{wildcardImport.Name}' was not found or was not valid."),
            },
            _ => ConvertTypePropertyAccess(syntax, ConvertTypeWithoutLowering(baseExpression), propertyName),
        };

    private static TypeReferencePropertyAccessExpression ConvertTypePropertyAccess(SyntaxBase syntax, TypeExpression baseExpression, string propertyName)
    {
        if (baseExpression.ExpressedType is not ObjectType baseObject || !baseObject.Properties.TryGetValue(propertyName, out var typeProperty))
        {
            throw new ArgumentException($"Property '{propertyName}' of type '{baseExpression.ExpressedType.Name}' was not found or was not valid.");
        }

        return new TypeReferencePropertyAccessExpression(syntax, baseExpression, propertyName, typeProperty.TypeReference.Type);
    }

    private TypeExpression ConvertTypeArrayAccess(TypeArrayAccessSyntax syntax) => Context.SemanticModel.GetTypeInfo(syntax.IndexExpression) switch
    {
        StringLiteralType @string => ConvertTypePropertyAccess(syntax, syntax.BaseExpression, @string.RawStringValue),
        IntegerLiteralType @int => ConvertTypeArrayAccess(syntax, ConvertTypeWithoutLowering(syntax.BaseExpression), @int.Value),
        _ => throw new ArgumentException("Array access syntax is not permitted in type expressions unless the indexing expression can be folded to a constant string or integer at compile time."),
    };

    private static TypeReferenceIndexAccessExpression ConvertTypeArrayAccess(TypeArrayAccessSyntax syntax, TypeExpression baseExpression, long index)
    {
        if (baseExpression.ExpressedType is not TupleType baseTuple || index < 0 || baseTuple.Items.Length <= index)
        {
            throw new ArgumentException($"Index {index} of type '{baseExpression.ExpressedType.Name}' was not found or was not valid.");
        }

        return new TypeReferenceIndexAccessExpression(syntax, baseExpression, index, baseTuple.Items[(int)index].Type);
    }

    private TypeReferenceAdditionalPropertiesAccessExpression ConvertTypeAdditionalPropertiesAccess(TypeAdditionalPropertiesAccessSyntax syntax)
    {
        var baseExpression = ConvertTypeWithoutLowering(syntax.BaseExpression);

        if (baseExpression.ExpressedType is not ObjectType objectType || objectType.AdditionalProperties is null || !objectType.HasExplicitAdditionalPropertiesType)
        {
            throw new ArgumentException($"The additional properties type of type '{baseExpression.ExpressedType.Name}' was not found or was not valid.");
        }

        return new TypeReferenceAdditionalPropertiesAccessExpression(syntax, baseExpression, objectType.AdditionalProperties.TypeReference.Type);
    }

    private TypeReferenceItemsAccessExpression ConvertTypeItemsAccess(TypeItemsAccessSyntax syntax)
    {
        var baseExpression = ConvertTypeWithoutLowering(syntax.BaseExpression);

        if (baseExpression.ExpressedType is not TypedArrayType arrayType)
        {
            throw new ArgumentException($"The element type of type '{baseExpression.ExpressedType.Name}' was not found or was not valid.");
        }

        return new TypeReferenceItemsAccessExpression(syntax, baseExpression, arrayType.Item.Type);
    }

    private TypeSymbol? TryGetPropertyType(INamespaceSymbol namespaceSymbol, string propertyName)
        => namespaceSymbol.TryGetNamespaceType() is NamespaceType type && type.Properties.TryGetValue(propertyName, out var property)
            ? property.TypeReference.Type
            : null;

    private TExpression ConvertWithoutLowering<TExpression>(SyntaxBase syntax)
        where TExpression : Expression
    {
        if (ConvertWithoutLowering(syntax) is not TExpression converted)
        {
            throw new ArgumentException($"Failed to convert syntax of type {syntax.GetType()} to expression of type {nameof(TExpression)}.");
        }

        return converted;
    }

    private TSymbol GetDeclaredSymbol<TSymbol>(SyntaxBase syntax)
        where TSymbol : DeclaredSymbol
    {
        if (!Context.SemanticModel.Root.DeclarationsBySyntax.TryGetValue(syntax, out var symbol) ||
            symbol is not TSymbol declaredSymbol)
        {
            throw new ArgumentException($"Failed to find symbol for syntax of type {syntax.GetType()}");
        }

        return declaredSymbol;
    }

    private TType GetTypeInfo<TType>(SyntaxBase syntax)
        where TType : TypeSymbol
    {
        if (Context.SemanticModel.GetTypeInfo(syntax) is not TType typeSymbol)
        {
            throw new ArgumentException($"Failed to find type symbol for syntax of type {syntax.GetType()}");
        }

        return typeSymbol;
    }

    private Expression EvaluateDecorators(DecorableSyntax decorable, Expression target)
    {
        var result = target;
        foreach (var decoratorSyntax in decorable.Decorators)
        {
            var symbol = Context.SemanticModel.GetSymbolInfo(decoratorSyntax.Expression);

            if (symbol is FunctionSymbol decoratorSymbol && decoratorSymbol.DeclaringObject is NamespaceType namespaceType)
            {
                var argumentTypes = decoratorSyntax.Arguments
                    .Select(argument => Context.SemanticModel.TypeManager.GetTypeInfo(argument))
                    .ToArray();

                // There should be exact one matching decorator since there's no errors.
                var decorator = namespaceType.DecoratorResolver.GetMatches(decoratorSymbol, argumentTypes).Single();

                result = decorator.Evaluate(ConvertWithoutLowering<FunctionCallExpression>(decoratorSyntax.Expression), result);
            }
        }

        return result;
    }

    private ProgramExpression ConvertProgram(ProgramSyntax syntax)
    {
        var metadataArray = Context.SemanticModel.Root.MetadataDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredMetadataExpression>()
            .ToImmutableArray();

        var declaredExtensions = Context.SemanticModel.Root.ExtensionDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<ExtensionExpression>()
            .ToImmutableArray();

        var implicitExtension = Context.SemanticModel.Binder.NamespaceResolver.ImplicitNamespaces
            .Select(x => x.Value.TryGetNamespaceType())
            .WhereNotNull()
            .Where(x =>
                // The 'az' and 'sys' namespaces do not utilize the extensibility contract and do not need to be emitted in the template
                !LanguageConstants.IdentifierComparer.Equals(x.Settings.BicepExtensionName, SystemNamespaceType.BuiltInName) &&
                !LanguageConstants.IdentifierComparer.Equals(x.Settings.BicepExtensionName, AzNamespaceType.BuiltInName))
            .Select(x => new ExtensionExpression(null, x.Name, x.Settings, null))
            .ToImmutableArray();

        var typeDefinitions = Context.SemanticModel.Root.TypeDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredTypeExpression>()
            .ToImmutableArray();

        var parameters = Context.SemanticModel.Root.ParameterDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredParameterExpression>()
            .ToImmutableArray();

        var functionVariables = Context.FunctionVariables
            .OrderBy(x => x.Value.Name, LanguageConstants.IdentifierComparer)
            .Select(x => new DeclaredVariableExpression(x.Key, x.Value.Name, null, x.Value.Value))
            .ToImmutableArray();

        var variables = Context.SemanticModel.Root.VariableDeclarations
            .Where(x => !Context.SemanticModel.SymbolsToInline.VariablesToInline.Contains(x))
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredVariableExpression>()
            .ToImmutableArray();

        var resources = Context.SemanticModel.DeclaredResources
            .Where(x => !Context.SemanticModel.SymbolsToInline.ExistingResourcesToInline.Contains(x.Symbol))
            .Select(x => ConvertWithoutLowering(x.Symbol.DeclaringSyntax))
            .OfType<DeclaredResourceExpression>()
            .ToImmutableArray();

        var modules = Context.SemanticModel.Root.ModuleDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredModuleExpression>()
            .ToImmutableArray();

        var outputs = Context.SemanticModel.Root.OutputDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredOutputExpression>()
            .ToImmutableArray();

        var asserts = Context.SemanticModel.Root.AssertDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredAssertExpression>()
            .ToImmutableArray();

        var functions = Context.SemanticModel.Root.FunctionDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredFunctionExpression>()
            .ToImmutableArray();

        return new ProgramExpression(
            syntax,
            metadataArray,
            [.. declaredExtensions, .. implicitExtension],
            typeDefinitions,
            parameters,
            functionVariables.AddRange(variables),
            functions,
            resources,
            modules,
            outputs,
            asserts);
    }

    private record LoopExpressionContext(string Name, SyntaxBase SourceSyntax, Expression LoopExpression);

    private ObjectPropertyExpression CreateModuleNameExpression(ModuleSymbol symbol, ObjectSyntax objectBody)
    {
        if (objectBody.TryGetPropertyByName(AzResourceTypeProvider.ResourceNamePropertyName) is { } namePropertySyntax)
        {
            // the user has specified an explicit name for the module - use it
            return ConvertObjectProperty(namePropertySyntax);
        }

        return new ObjectPropertyExpression(
            SourceSyntax: null,
            Key: new StringLiteralExpression(SourceSyntax: null, Value: AzResourceTypeProvider.ResourceNamePropertyName),
            Value: ExpressionFactory.CreateGeneratedModuleName(symbol));
    }

    private DeclaredModuleExpression ConvertModule(ModuleDeclarationSyntax syntax)
    {
        var symbol = GetDeclaredSymbol<ModuleSymbol>(syntax);
        Expression? condition = null;
        LoopExpressionContext? loop = null;

        // Unwrap the 'real' body if there's a condition / for loop
        var body = syntax.Value;
        if (body is ForSyntax @for)
        {
            loop = new(symbol.Name, @for, ConvertWithoutLowering(@for.Expression));
            body = @for.Body;
        }

        if (body is IfConditionSyntax @if)
        {
            condition = ConvertWithoutLowering(@if.ConditionExpression);
            body = @if.Body;
        }

        var objectBody = (ObjectSyntax)body;

        var properties = objectBody.Properties
            .Where(x => x.TryGetKeyText() is not { } key || !ModulePropertiesToOmit.Contains(key))
            .Select(ConvertObjectProperty)
            .Append(CreateModuleNameExpression(symbol, objectBody));
        Expression bodyExpression = new ObjectExpression(body, [.. properties]);

        var parameters = objectBody.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName);
        var extensionConfigs = objectBody.TryGetPropertyByName(LanguageConstants.ModuleExtensionConfigsPropertyName);

        if (condition is not null)
        {
            bodyExpression = new ConditionExpression(condition.SourceSyntax, condition, bodyExpression);
        }

        if (loop is not null)
        {
            bodyExpression = new ForLoopExpression(
                loop.SourceSyntax,
                loop.LoopExpression,
                bodyExpression,
                loop.Name,
                GetBatchSize(syntax));
        }

        return new DeclaredModuleExpression(
            syntax,
            symbol,
            Context.SemanticModel.ModuleScopeData[symbol],
            body,
            bodyExpression,
            parameters is not null ? ConvertWithoutLowering(parameters.Value) : null,
            extensionConfigs is not null ? ConvertWithoutLowering(extensionConfigs.Value) : null,
            BuildDependencyExpressions(symbol, body));
    }

    private ImmutableArray<ResourceDependencyExpression> BuildDependencyExpressions(DeclaredSymbol dependent, SyntaxBase body)
        => [.. Context.ResourceDependencies[dependent]
            .SelectMany(dd => ToDependencyExpressions(dd, body, Context.ResourceDependencies))
            .GroupBy(t => t.Target.Resource)
            .SelectMany(g => g.FirstOrDefault(t => t.Target.IndexExpression is null) is { } dependencyOnCollection
                ? dependencyOnCollection.AsEnumerable()
                : g.Distinct(t => t.Target))
            .OrderBy(t => t.TargetKey)  // order to generate a deterministic template
            .Select(t => t.Expression)];

    public Expression? GetResourceCondition(DeclaredResourceMetadata resourceMetadata)
        => GetResourceBody(resourceMetadata).condition;

    private (Expression? condition, LoopExpressionContext? loop, SyntaxBase body) GetResourceBody(DeclaredResourceMetadata resource)
    {
        Expression? condition = null;
        LoopExpressionContext? loop = null;

        void AddCondition(Expression newCondition)
        {
            if (condition is null)
            {
                condition = newCondition;
            }
            else
            {
                condition = new BinaryExpression(
                    newCondition.SourceSyntax,
                    BinaryOperator.LogicalAnd,
                    condition,
                    newCondition);
            }
        }

        void AddLoop(LoopExpressionContext newLoop)
        {
            if (loop is null)
            {
                loop = newLoop;
            }
            else
            {
                throw new InvalidOperationException("Nested loops are not supported");
            }
        }

        // Note: conditions STACK with nesting.
        //
        // Children inherit the conditions of their parents, etc. This avoids a problem
        // where we emit a dependsOn to something that's not in the template, or not
        // being evaluated in the template.
        var ancestors = this.Context.SemanticModel.ResourceAncestors.GetAncestors(resource);
        foreach (var ancestor in ancestors)
        {
            if (ancestor.AncestorType == ResourceAncestorGraph.ResourceAncestorType.Nested &&
                ancestor.Resource.Symbol.DeclaringResource.Value is IfConditionSyntax ancestorIf)
            {
                AddCondition(ConvertWithoutLowering(ancestorIf.ConditionExpression));
            }

            if (ancestor.AncestorType == ResourceAncestorGraph.ResourceAncestorType.Nested &&
                ancestor.Resource.Symbol.DeclaringResource.Value is ForSyntax ancestorFor)
            {
                AddLoop(new(
                    ExpressionConverter.GetSymbolicName(Context.SemanticModel.ResourceAncestors, ancestor.Resource),
                    ancestorFor,
                    ConvertWithoutLowering(ancestorFor.Expression)));
            }
        }

        // Unwrap the 'real' resource body if there's a condition / for loop
        var body = resource.Symbol.DeclaringResource.Value;
        if (body is ForSyntax @for)
        {
            AddLoop(new(
                ExpressionConverter.GetSymbolicName(Context.SemanticModel.ResourceAncestors, resource),
                @for,
                ConvertWithoutLowering(@for.Expression)));
            body = @for.Body;
        }

        if (body is IfConditionSyntax @if)
        {
            AddCondition(ConvertWithoutLowering(@if.ConditionExpression));
            body = @if.Body;
        }

        return (condition, loop, body);
    }

    private DeclaredResourceExpression ConvertResource(ResourceDeclarationSyntax syntax)
    {
        var resource = Context.SemanticModel.ResourceMetadata.TryLookup(syntax) as DeclaredResourceMetadata
            ?? throw new InvalidOperationException("Failed to find resource in cache");

        var (condition, loop, body) = GetResourceBody(resource);

        var propertiesToOmit = resource.IsAzResource ? AzResourcePropertiesToOmit : NonAzResourcePropertiesToOmit;
        var properties = ((ObjectSyntax)body).Properties
            .Where(x => x.TryGetKeyText() is not { } key || !propertiesToOmit.Contains(key))
            .Select(ConvertObjectProperty);
        Expression bodyExpression = new ObjectExpression(body, [.. properties]);

        if (condition is not null)
        {
            bodyExpression = new ConditionExpression(condition.SourceSyntax, condition, bodyExpression);
        }

        if (loop is not null)
        {
            bodyExpression = new ForLoopExpression(
                loop.SourceSyntax,
                loop.LoopExpression,
                bodyExpression,
                loop.Name,
                GetBatchSize(syntax));
        }

        return new DeclaredResourceExpression(
            syntax,
            resource,
            Context.SemanticModel.ResourceScopeData[resource],
            body,
            bodyExpression,
            BuildDependencyExpressions(resource.Symbol, body),
            []);
    }

    private Expression ConvertArray(ArraySyntax array)
    {
        var hasSpread = false;
        var chunks = new List<Expression>();
        var currentChunk = new List<Expression>();
        void completePreviousChunk()
        {
            if (currentChunk.Count > 0)
            {
                chunks.Add(new ArrayExpression(array, [.. currentChunk]));
                currentChunk.Clear();
            }
        }

        foreach (var child in array.Children)
        {
            if (child is SpreadExpressionSyntax spread)
            {
                completePreviousChunk();
                chunks.Add(ConvertWithoutLowering(spread.Expression));
                hasSpread = true;
            }
            else if (child is ArrayItemSyntax arrayItem)
            {
                currentChunk.Add(ConvertWithoutLowering(arrayItem.Value));
            }
        }
        completePreviousChunk();

        return chunks.Count switch
        {
            0 => new ArrayExpression(array, []),
            // preserve [ ...[ bar ] ] rather than converting it to [ foo: bar ]
            1 when !hasSpread => chunks[0],
            _ => new FunctionCallExpression(array, "flatten", [new ArrayExpression(array, [.. chunks])]),
        };
    }

    private Expression ConvertObject(ObjectSyntax @object)
    {
        var hasSpread = false;
        var chunks = new List<Expression>();
        var currentChunk = new List<ObjectPropertyExpression>();
        void completePreviousChunk()
        {
            if (currentChunk.Count > 0)
            {
                chunks.Add(new ObjectExpression(@object, [.. currentChunk]));
                currentChunk.Clear();
            }
        }

        foreach (var child in @object.Children)
        {
            if (child is SpreadExpressionSyntax spread)
            {
                completePreviousChunk();
                chunks.Add(ConvertWithoutLowering(spread.Expression));
                hasSpread = true;
            }
            else if (child is ObjectPropertySyntax objectProperty)
            {
                currentChunk.Add(ConvertObjectProperty(objectProperty));
            }
        }
        completePreviousChunk();

        return chunks.Count switch
        {
            0 => new ObjectExpression(@object, []),
            // preserve { ...{ foo: bar } } rather than converting it to { foo: bar }
            1 when !hasSpread => chunks[0],
            _ => new FunctionCallExpression(@object, "shallowMerge", [new ArrayExpression(@object, [.. chunks])]),
        };
    }

    private ObjectPropertyExpression ConvertObjectProperty(ObjectPropertySyntax syntax)
    {
        var keyExpression = syntax.Key is IdentifierSyntax identifier ?
            new StringLiteralExpression(identifier, identifier.IdentifierName) :
            ConvertWithoutLowering(syntax.Key);

        return new(syntax, keyExpression, ConvertWithoutLowering(syntax.Value));
    }

    private Expression ConvertFunctionDirect(FunctionCallSyntaxBase functionCall)
    {
        switch (functionCall)
        {
            case FunctionCallSyntax function:
                return new FunctionCallExpression(
                    function,
                    function.Name.IdentifierName,
                    [.. function.Arguments.Select(a => ConvertWithoutLowering(a.Expression))]);

            case InstanceFunctionCallSyntax method:
                var (baseSyntax, indexExpression) = SyntaxHelper.UnwrapArrayAccessSyntax(
                    SyntaxHelper.UnwrapNonNullAssertion(method.BaseExpression));
                var baseSymbol = Context.SemanticModel.GetSymbolInfo(baseSyntax);

                if (baseSymbol is INamespaceSymbol namespaceSymbol)
                {
                    Debug.Assert(indexExpression is null, "Indexing into a namespace should have been blocked by type analysis");
                    return new FunctionCallExpression(
                        method,
                        method.Name.IdentifierName,
                        [.. method.Arguments.Select(a => ConvertWithoutLowering(a.Expression))]);
                }

                var resource = Context.SemanticModel.ResourceMetadata.TryLookup(baseSyntax);
                if (resource is DeclaredResourceMetadata decl && !decl.IsAzResource)
                {
                    Expression nameExpression = indexExpression is { } ?
                        new FunctionCallExpression(baseSyntax, "format", new Expression[] {
                            new StringLiteralExpression(baseSyntax, $"{decl.Symbol.Name}[{{0}}]"),
                            ConvertWithoutLowering(indexExpression),
                        }.ToImmutableArray()) :
                        new StringLiteralExpression(baseSyntax, decl.Symbol.Name);

                    return new FunctionCallExpression(
                        method,
                        "invokeResourceMethod",
                        [
                            nameExpression,
                            new StringLiteralExpression(method.Name, method.Name.IdentifierName),
                            new ArrayExpression(
                                method,
                                [.. method.Arguments.Select(a => ConvertWithoutLowering(a.Expression))]),
                        ]);
                }

                var indexContext = resource switch
                {
                    DeclaredResourceMetadata declaredResource => TryGetReplacementContext(declaredResource.NameSyntax, indexExpression, method),
                    ModuleOutputResourceMetadata moduleOutputResource => TryGetReplacementContext(moduleOutputResource.Module, indexExpression, method),
                    _ => null,
                };

                if (resource is not null)
                {
                    // Handle list<method_name>(...) method on resource symbol - e.g. stgAcc.listKeys()
                    // This is also used for kv.getSecret() - for passing secure values to module parameters
                    return new ResourceFunctionCallExpression(
                        method,
                        new ResourceReferenceExpression(method.BaseExpression, resource, indexContext),
                        method.Name.IdentifierName,
                        [.. method.Arguments.Select(a => ConvertWithoutLowering(a.Expression))]);
                }

                throw new InvalidOperationException($"Unrecognized base expression {baseSymbol?.Kind}");
            default:
                throw new NotImplementedException($"Cannot emit unexpected expression of type {functionCall.GetType().Name}");
        }
    }

    private Expression ConvertFunction(FunctionCallSyntaxBase functionCall)
    {
        if (Context.FunctionVariables.GetValueOrDefault(functionCall) is { } functionVariable)
        {
            return new SynthesizedVariableReferenceExpression(functionCall, functionVariable.Name);
        }

        if (Context.SemanticModel.GetSymbolInfo(functionCall) is DeclaredFunctionSymbol declaredFunction)
        {
            return new UserDefinedFunctionCallExpression(
                functionCall,
                declaredFunction,
                [.. functionCall.Arguments.Select(a => ConvertWithoutLowering(a.Expression))]);
        }

        if (Context.SemanticModel.GetSymbolInfo(functionCall) is ImportedFunctionSymbol importedFunction)
        {
            return new ImportedUserDefinedFunctionCallExpression(
                functionCall,
                importedFunction,
                [.. functionCall.Arguments.Select(a => ConvertWithoutLowering(a.Expression))]);
        }

        if (functionCall is InstanceFunctionCallSyntax instanceFunctionCall &&
            Context.SemanticModel.GetSymbolInfo(instanceFunctionCall.BaseExpression) is WildcardImportSymbol wildcardImport)
        {
            return new WildcardImportInstanceFunctionCallExpression(
                functionCall,
                wildcardImport,
                instanceFunctionCall.Name.IdentifierName,
                [.. functionCall.Arguments.Select(a => ConvertWithoutLowering(a.Expression))]);
        }

        if (Context.SemanticModel.TypeManager.GetMatchedFunctionResultValue(functionCall) is { } functionValue)
        {
            return functionValue;
        }

        var converted = ConvertFunctionDirect(functionCall);
        if (converted is FunctionCallExpression convertedFunction &&
            Context.SemanticModel.TypeManager.GetMatchedFunctionOverload(functionCall) is { Evaluator: { } } functionOverload)
        {
            return functionOverload.Evaluator(convertedFunction);
        }

        return converted;
    }

    private Expression ConvertArrayAccess(ArrayAccessSyntax arrayAccess)
    {
        // if there is an array access on a resource/module reference, we have to generate differently
        // when constructing the reference() function call, the resource name expression needs to have its local
        // variable replaced with <loop array expression>[this array access' index expression]
        if (arrayAccess.BaseExpression is VariableAccessSyntax || arrayAccess.BaseExpression is ResourceAccessSyntax)
        {
            if (Context.SemanticModel.ResourceMetadata.TryLookup(arrayAccess.BaseExpression) is DeclaredResourceMetadata resource &&
                resource.Symbol.IsCollection)
            {
                var indexContext = TryGetReplacementContext(resource, arrayAccess.IndexExpression, arrayAccess);
                return new ResourceReferenceExpression(arrayAccess, resource, indexContext);
            }

            if (Context.SemanticModel.GetSymbolInfo(arrayAccess.BaseExpression) is ModuleSymbol { IsCollection: true } moduleSymbol)
            {
                var indexContext = TryGetReplacementContext(moduleSymbol, arrayAccess.IndexExpression, arrayAccess);
                return new ModuleReferenceExpression(arrayAccess, moduleSymbol, indexContext);
            }
        }

        var convertedBase = ConvertWithoutLowering(arrayAccess.BaseExpression);
        var convertedIndex = ConvertWithoutLowering(arrayAccess.IndexExpression);

        return new ArrayAccessExpression(arrayAccess, convertedBase, convertedIndex, GetAccessExpressionFlags(arrayAccess));
    }

    private AccessExpressionFlags GetAccessExpressionFlags(AccessExpressionSyntax accessExpression)
    {
        var flags = AccessExpressionFlags.None;
        if (accessExpression.IsSafeAccess)
        {
            flags |= AccessExpressionFlags.SafeAccess;
        }

        if (accessExpression is ArrayAccessSyntax arrayAccess &&
            arrayAccess.FromEndMarker is not null)
        {
            flags |= AccessExpressionFlags.FromEnd;
        }

        if (IsAccessExpressionSyntax(accessExpression.BaseExpression))
        {
            flags |= AccessExpressionFlags.Chained;
        }

        return flags;
    }

    private bool IsAccessExpressionSyntax(SyntaxBase syntax) => syntax switch
    {
        AccessExpressionSyntax => true,

        // type transformations with no runtime representation should be unwrapped and inspected
        NonNullAssertionSyntax nonNullAssertion => IsAccessExpressionSyntax(nonNullAssertion.BaseExpression),
        _ when SemanticModelHelper.TryGetFunctionInNamespace(
                Context.SemanticModel,
                SystemNamespaceType.BuiltInName,
                syntax) is { } functionCall &&
            functionCall.NameEquals(LanguageConstants.AnyFunction) &&
            functionCall.Arguments.Length == 1 => IsAccessExpressionSyntax(functionCall.Arguments[0].Expression),

        _ => false,
    };

    private Expression ConvertResourcePropertyAccess(PropertyAccessSyntax sourceSyntax, ResourceMetadata resource, IndexReplacementContext? indexContext, string propertyName, AccessExpressionFlags flags)
        => ExpressionFactory.CreateResourcePropertyAccess(resource, indexContext, propertyName, sourceSyntax, flags);

    private Expression ConvertPropertyAccess(PropertyAccessSyntax propertyAccess)
    {
        var flags = GetAccessExpressionFlags(propertyAccess);

        // Looking for: myResource.someProp (where myResource is a resource declared in-file)
        if (Context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is DeclaredResourceMetadata resource)
        {
            return ConvertResourcePropertyAccess(propertyAccess, resource, null, propertyAccess.PropertyName.IdentifierName, flags);
        }

        // Looking for: myResource[blah].someProp (where myResource is a resource declared in-file)
        if (propertyAccess.BaseExpression is ArrayAccessSyntax propArrayAccess &&
            Context.SemanticModel.ResourceMetadata.TryLookup(propArrayAccess.BaseExpression) is DeclaredResourceMetadata resourceCollection)
        {
            var indexContext = TryGetReplacementContext(resourceCollection, propArrayAccess.IndexExpression, propertyAccess);
            return ConvertResourcePropertyAccess(propertyAccess, resourceCollection, indexContext, propertyAccess.PropertyName.IdentifierName, flags);
        }

        // Looking for: myResource.someProp (where myResource is a parameter of type resource)
        if (Context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ParameterResourceMetadata parameter)
        {
            return ConvertResourcePropertyAccess(propertyAccess, parameter, null, propertyAccess.PropertyName.IdentifierName, flags);
        }

        // Looking for: myMod.outputs.someProp (where someProp is an output of type resource)
        if (propertyAccess.BaseExpression is PropertyAccessSyntax &&
            Context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleOutput &&
            !moduleOutput.Module.IsCollection)
        {
            return ConvertResourcePropertyAccess(propertyAccess, moduleOutput, null, propertyAccess.PropertyName.IdentifierName, flags);
        }

        // Looking for: myMod[blah].outputs.someProp (where someProp is an output of type resource)
        if (propertyAccess.BaseExpression is PropertyAccessSyntax moduleCollectionOutputProperty &&
            moduleCollectionOutputProperty.BaseExpression is PropertyAccessSyntax moduleCollectionOutputs &&
            moduleCollectionOutputs.BaseExpression is ArrayAccessSyntax moduleArrayAccess &&
            Context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleCollectionOutputMetadata &&
            moduleCollectionOutputMetadata.Module.IsCollection)
        {
            var indexContext = TryGetReplacementContext(moduleCollectionOutputMetadata.Module, moduleArrayAccess.IndexExpression, propertyAccess);
            return ConvertResourcePropertyAccess(propertyAccess, moduleCollectionOutputMetadata, indexContext, propertyAccess.PropertyName.IdentifierName, flags);
        }

        // Looking for: expr.outputs.blah (where expr is any expression of type module)
        if (propertyAccess.BaseExpression is PropertyAccessSyntax childPropertyAccess &&
            TypeHelper.SatisfiesCondition(Context.SemanticModel.GetTypeInfo(childPropertyAccess.BaseExpression), x => x is ModuleType) &&
            childPropertyAccess.PropertyName.NameEquals(LanguageConstants.ModuleOutputsPropertyName))
        {
            return new ModuleOutputPropertyAccessExpression(
                propertyAccess,
                ConvertWithoutLowering(propertyAccess.BaseExpression),
                propertyAccess.PropertyName.IdentifierName,
                IsSecureOutput: TypeHelper.SatisfiesCondition(
                    Context.SemanticModel.GetTypeInfo(childPropertyAccess.BaseExpression),
                    x => (x as ModuleType)?.TryGetOutputType(propertyAccess.PropertyName.IdentifierName) is { } outputType &&
                        TypeHelper.IsOrContainsSecureType(outputType)),
                flags);
        }

        // Looking for: expr.blah (where expr is a wildcard import)
        if (Context.SemanticModel.GetSymbolInfo(propertyAccess.BaseExpression) is WildcardImportSymbol wildcardImport)
        {
            return new WildcardImportVariablePropertyReferenceExpression(propertyAccess, wildcardImport, propertyAccess.PropertyName.IdentifierName);
        }

        return new PropertyAccessExpression(
            propertyAccess,
            ConvertWithoutLowering(propertyAccess.BaseExpression),
            propertyAccess.PropertyName.IdentifierName,
            flags);
    }

    private Expression ConvertResourceAccess(ResourceAccessSyntax resourceAccessSyntax)
    {
        if (Context.SemanticModel.ResourceMetadata.TryLookup(resourceAccessSyntax) is { } resource)
        {
            return new ResourceReferenceExpression(resourceAccessSyntax, resource, null);
        }

        throw new NotImplementedException($"Unable to obtain resource metadata when generating a resource access expression.");
    }

    private Expression ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax)
    {
        var symbol = Context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

        switch (symbol)
        {
            case DeclaredSymbol declaredSymbol when Context.SemanticModel.ResourceMetadata.TryLookup(declaredSymbol.DeclaringSyntax) is { } resource:
                return new ResourceReferenceExpression(variableAccessSyntax, resource, null);

            case ParameterSymbol parameterSymbol:
                return new ParametersReferenceExpression(variableAccessSyntax, parameterSymbol);

            case ParameterAssignmentSymbol parameterSymbol:
                if (Context.ExternalInputReferences.ParametersReferences.Contains(parameterSymbol))
                {
                    // we're evaluating a parameter that has an external input function reference, so inline it
                    return ConvertWithoutLowering(parameterSymbol.DeclaringParameterAssignment.Value);
                }
                return new ParametersAssignmentReferenceExpression(variableAccessSyntax, parameterSymbol);

            case VariableSymbol variableSymbol:
                if (Context.SemanticModel.SymbolsToInline.VariablesToInline.Contains(variableSymbol) ||
                    Context.ExternalInputReferences.VariablesReferences.Contains(variableSymbol))
                {
                    // we've got a runtime dependency, or we're evaluating a variable that has an external input function reference,
                    // so we have to inline the variable usage
                    return ConvertWithoutLowering(variableSymbol.DeclaringVariable.Value);
                }

                return new VariableReferenceExpression(variableAccessSyntax, variableSymbol);

            case ModuleSymbol moduleSymbol:
                return new ModuleReferenceExpression(variableAccessSyntax, moduleSymbol, null);

            case LocalVariableSymbol localVariableSymbol:
                return GetLocalVariableExpression(variableAccessSyntax, localVariableSymbol);

            case ImportedVariableSymbol importedSymbol:
                return new ImportedVariableReferenceExpression(variableAccessSyntax, importedSymbol);

            case ExtensionNamespaceSymbol extensionNamespaceSymbol:
                return new ExtensionReferenceExpression(variableAccessSyntax, extensionNamespaceSymbol);

            case ExtensionConfigAssignmentSymbol extensionConfigAssignmentSymbol:
                return new ExtensionConfigAssignmentReferenceExpression(variableAccessSyntax, extensionConfigAssignmentSymbol);

            case BaseParametersSymbol baseParamsSymbol:
                var objectProperties = baseParamsSymbol.ParentAssignments
                    .Select(pa => new ObjectPropertyExpression(
                        pa.DeclaringParameterAssignment,
                        new StringLiteralExpression(pa.DeclaringParameterAssignment.Name, pa.Name),
                        ConvertWithoutLowering(pa.DeclaringParameterAssignment.Value)))
                    .ToImmutableArray();

                return new ObjectExpression(variableAccessSyntax, objectProperties);

            default:
                throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' and type '{symbol?.GetType().Name}' when generating a variable access expression.");

        }
    }

    private Expression GetLocalVariableExpression(SyntaxBase sourceSyntax, LocalVariableSymbol localVariableSymbol)
    {
        if (this.localReplacements.TryGetValue(localVariableSymbol, out var replacement))
        {
            // the current Context has specified an expression to be used for this local variable symbol
            // to override the regular conversion
            return replacement;
        }

        var enclosingSyntax = GetEnclosingDeclaringSyntax(localVariableSymbol);
        switch (enclosingSyntax)
        {
            case ForSyntax @for:
                return GetLoopVariable(localVariableSymbol, @for, new CopyIndexExpression(sourceSyntax, GetCopyIndexName(@for)));
            case LambdaSyntax:
                return new LambdaVariableReferenceExpression(sourceSyntax, localVariableSymbol, IsFunctionLambda: false);
            case TypedLambdaSyntax:
                return new LambdaVariableReferenceExpression(sourceSyntax, localVariableSymbol, IsFunctionLambda: true);
        }

        throw new NotImplementedException($"{nameof(LocalVariableSymbol)} was declared by an unexpected syntax type '{enclosingSyntax?.GetType().Name}'.");
    }

    private string? GetCopyIndexName(ForSyntax @for)
    {
        return this.Context.SemanticModel.Binder.GetParent(@for) switch
        {
            // copyIndex without name resolves to module/resource loop index in the runtime
            ResourceDeclarationSyntax => null,
            ModuleDeclarationSyntax => null,

            // variable copy index has the name of the variable
            VariableDeclarationSyntax variable when variable.Name.IsValid => variable.Name.IdentifierName,

            // output loops are only allowed at the top level and don't have names, either
            OutputDeclarationSyntax => null,

            // the property copy index has the name of the property
            ObjectPropertySyntax property when property.TryGetKeyText() is { } key && ReferenceEquals(property.Value, @for) => key,

            _ => throw new NotImplementedException("Unexpected for-expression grandparent.")
        };
    }

    private SyntaxBase GetEnclosingDeclaringSyntax(LocalVariableSymbol localVariable)
    {
        // we're following the symbol hierarchy rather than syntax hierarchy because
        // this guarantees a single hop in all cases
        var symbolParent = this.Context.SemanticModel.GetSymbolParent(localVariable);
        if (symbolParent is not LocalScope localScope)
        {
            throw new NotImplementedException($"{nameof(LocalVariableSymbol)} has un unexpected parent of type '{symbolParent?.GetType().Name}'.");
        }

        return localScope.DeclaringSyntax;
    }

    private Expression GetLoopItemVariable(ForSyntax @for, Expression index)
    {
        // loop item variable should be replaced with <array expression>[<index expression>]
        var forExpression = ConvertWithoutLowering(@for.Expression);

        return new ArrayAccessExpression(index.SourceSyntax, forExpression, index, AccessExpressionFlags.None);
    }

    private Expression GetLoopVariable(LocalVariableSymbol localVariableSymbol, ForSyntax @for, Expression index)
    {
        return localVariableSymbol.LocalKind switch
        {
            // this is the "item" variable of a for-expression
            // to emit this, we need to index the array expression by the copyIndex() function
            LocalKind.ForExpressionItemVariable => GetLoopItemVariable(@for, index),

            // this is the "index" variable of a for-expression inside a variable block
            // to emit this, we need to return a copyIndex(...) function
            LocalKind.ForExpressionIndexVariable => index,

            _ => throw new NotImplementedException($"Unexpected local variable kind '{localVariableSymbol.LocalKind}'."),
        };
    }

    private ForSyntax GetEnclosingForExpression(LocalVariableSymbol localVariable) => GetEnclosingForExpression(Context.SemanticModel, localVariable);

    private static ForSyntax GetEnclosingForExpression(SemanticModel model, LocalVariableSymbol localVariable)
    {
        // we're following the symbol hierarchy rather than syntax hierarchy because
        // this guarantees a single hop in all cases
        var symbolParent = model.GetSymbolParent(localVariable);
        if (symbolParent is not LocalScope localScope)
        {
            throw new NotImplementedException($"{nameof(LocalVariableSymbol)} has un unexpected parent of type '{symbolParent?.GetType().Name}'.");
        }

        if (localScope.DeclaringSyntax is ForSyntax @for)
        {
            return @for;
        }

        throw new NotImplementedException($"{nameof(LocalVariableSymbol)} was declared by an unexpected syntax type '{localScope.DeclaringSyntax?.GetType().Name}'.");
    }

    private IndexReplacementContext? TryGetReplacementContext(ModuleSymbol module, SyntaxBase? indexExpression, SyntaxBase newContext)
    {
        if (module.TryGetBodyProperty(LanguageConstants.ModuleNamePropertyName) is null)
        {
            // The module has a generated module name.
            return indexExpression is not null
                ? new(ImmutableDictionary<LocalVariableSymbol, Expression>.Empty, this.ConvertWithoutLowering(indexExpression))
                : null;
        }

        return TryGetReplacementContext(module.GetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName), indexExpression, newContext);
    }

    private IndexReplacementContext? TryGetReplacementContext(DeclaredResourceMetadata resource, SyntaxBase? indexExpression, SyntaxBase newContext)
    {
        SyntaxBase movedSyntax = resource.IsAzResource ? SyntaxFactory.CreateArray(GetResourceNameSyntaxSegments(resource)) : resource.Symbol.NameIdentifier;

        return TryGetReplacementContext(movedSyntax, indexExpression, newContext);
    }

    private IndexReplacementContext? TryGetReplacementContext(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
    {
        var inaccessibleLocals = this.Context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(nameSyntax, newContext);
        var inaccessibleLocalLoops = inaccessibleLocals.Select(local => GetEnclosingForExpression(local)).Distinct().ToList();

        switch (inaccessibleLocalLoops.Count)
        {
            case 0:
                // moving the name expression does not produce any inaccessible locals (no locals means no loops)
                // regardless if there is an index expression or not, we don't need to append replacements
                if (indexExpression is null)
                {
                    return null;
                }

                return new(this.localReplacements, ConvertWithoutLowering(indexExpression));

            case 1 when indexExpression is not null:
                // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                var @for = inaccessibleLocalLoops.Single();
                var localReplacements = this.localReplacements;
                foreach (var local in inaccessibleLocals)
                {
                    var replacementValue = GetLoopVariable(local, @for, ConvertWithoutLowering(indexExpression));
                    localReplacements = localReplacements.SetItem(local, replacementValue);
                }

                return new(localReplacements, ConvertWithoutLowering(indexExpression));

            default:
                throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index replacement.");
        }
    }

    private ulong? GetBatchSize(StatementSyntax statement)
    {
        var decorator = SemanticModelHelper.TryGetDecoratorInNamespace(
            Context.SemanticModel,
            statement,
            SystemNamespaceType.BuiltInName,
            LanguageConstants.BatchSizePropertyName);

        if (decorator?.Arguments is { } arguments
            && arguments.Count() == 1
            && arguments.ToList()[0].Expression is IntegerLiteralSyntax integerLiteral)
        {
            return integerLiteral.Value;
        }
        return null;
    }

    private record DependencyExpression(
        string TargetKey, // Used for sorting.
        ResourceDependency Target,
        ResourceDependencyExpression Expression);

    private IEnumerable<DependencyExpression> ToDependencyExpressions(
        ResourceDependency dependency,
        SyntaxBase newContext,
        ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> dependencies)
    {
        foreach (var path in GatherDependencyPaths([dependency], dependencies))
        {
            Expression reference;
            ResourceDependency target;
            string targetKey;
            var localReplacements = this.localReplacements;
            var allNodesInPathAccessCopyIndex = true;

            int i = 0;
            do
            {
                target = path[i];
                targetKey = target.Resource.Name;
                var targetContext = i == 0 ? newContext : path[i - 1].Resource.DeclaringSyntax;
                IndexReplacementContext? indexContext = null;

                switch (target.Resource)
                {
                    case ResourceSymbol resource:
                        {
                            var metadata = Context.SemanticModel.ResourceMetadata.TryLookup(resource.DeclaringSyntax) as DeclaredResourceMetadata
                                ?? throw new InvalidOperationException("Failed to find resource in cache");

                            targetKey = ExpressionConverter.GetSymbolicName(Context.SemanticModel.ResourceAncestors, metadata);
                            indexContext = (resource.IsCollection && target.IndexExpression is null)
                                ? null
                                : new ExpressionBuilder(Context, localReplacements)
                                    .TryGetReplacementContext(metadata, target.IndexExpression, targetContext);
                            reference = new ResourceReferenceExpression(null, metadata, indexContext);
                            break;
                        }
                    case ModuleSymbol module:
                        {
                            indexContext = (module.IsCollection && target.IndexExpression is null)
                                ? null
                                : new ExpressionBuilder(Context, localReplacements)
                                    .TryGetReplacementContext(module, target.IndexExpression, targetContext);
                            reference = new ModuleReferenceExpression(null, module, indexContext);
                            break;
                        }
                    case VariableSymbol variable:
                        {
                            indexContext = (variable.IsCopyVariable && target.IndexExpression is null)
                                ? null
                                : new ExpressionBuilder(Context, localReplacements)
                                    .TryGetReplacementContext(variable.DeclaringVariable.GetBody(), target.IndexExpression, targetContext);
                            reference = new VariableReferenceExpression(null, variable);
                            break;
                        }
                    default:
                        throw new InvalidOperationException($"Found dependency '{target.Resource.Name}' of unexpected type {target.Resource.GetType()}");
                }

                localReplacements = indexContext?.LocalReplacements ?? localReplacements;
                var copyIndexAccesses = indexContext is not null
                    ? CopyIndexExpressionCollector.FindContainedCopyIndexExpressions(indexContext.Index)
                    : [];

                if (copyIndexAccesses.Length > 0 && !allNodesInPathAccessCopyIndex)
                {
                    target = target with { IndexExpression = null };
                    reference = reference switch
                    {
                        ModuleReferenceExpression modRef => modRef with { IndexContext = null },
                        ResourceReferenceExpression resourceRef => resourceRef with { IndexContext = null },
                        _ => reference,
                    };
                }

                allNodesInPathAccessCopyIndex = allNodesInPathAccessCopyIndex && copyIndexAccesses.Length > 0;
            } while (++i < path.Length);

            yield return new(targetKey, target, new(null, reference));
        }
    }

    private class CopyIndexExpressionCollector : ExpressionVisitor
    {
        private readonly ImmutableArray<CopyIndexExpression>.Builder copyIndexExpressions
            = ImmutableArray.CreateBuilder<CopyIndexExpression>();

        private CopyIndexExpressionCollector() { }

        public static ImmutableArray<CopyIndexExpression> FindContainedCopyIndexExpressions(Expression? expression)
        {
            if (expression is null)
            {
#pragma warning disable IDE0301 // Using a simplified collection initialization results in an allocation, whereas using ImmutableArray<T>.Empty does not
                return ImmutableArray<CopyIndexExpression>.Empty;
#pragma warning restore IDE0301
            }

            var visitor = new CopyIndexExpressionCollector();
            expression.Accept(visitor);

            return visitor.copyIndexExpressions.ToImmutable();
        }

        public override void VisitCopyIndexExpression(CopyIndexExpression expression)
        {
            copyIndexExpressions.Add(expression);
        }
    }

    private IEnumerable<ImmutableArray<ResourceDependency>> GatherDependencyPaths(
        ImmutableArray<ResourceDependency> currentPath,
        ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> dependencies)
    {
        if (IsDependencyPathTerminus(currentPath[^1]))
        {
            yield return currentPath;
            yield break;
        }

        foreach (var transitiveDependency in dependencies[currentPath[^1].Resource])
        {
            if (currentPath.Contains(transitiveDependency))
            {
                // We've got a cycle. This should have been caught and blocked earlier
                throw new InvalidOperationException("Dependency cycle detected: "
                    + string.Join(" -> ", currentPath.Select(d => d.Resource.Name)));
            }

            foreach (var transitivePath in GatherDependencyPaths(currentPath.Add(transitiveDependency), dependencies))
            {
                yield return transitivePath;
            }
        }
    }

    private bool IsDependencyPathTerminus(ResourceDependency dependency) => dependency.Resource switch
    {
        ModuleSymbol => true,
        ResourceSymbol r => !r.DeclaringResource.IsExistingResource() ||
            // only use an existing resource as the terminus iff the compilation will include existing resources and the reference is not weak
            (Context.Settings.EnableSymbolicNames && !dependency.WeakReference),
        _ => false,
    };

    /// <summary>
    /// Returns a collection of name segment expressions for the specified resource. Local variable replacements
    /// are performed so the expressions are valid in the language/binding scope of the specified resource.
    /// </summary>
    /// <param name="resource">The resource</param>
    public IEnumerable<SyntaxBase> GetResourceNameSyntaxSegments(DeclaredResourceMetadata resource)
        => GetResourceNameSyntaxSegments(this.Context.SemanticModel, resource);

    public static IEnumerable<SyntaxBase> GetResourceNameSyntaxSegments(SemanticModel model, DeclaredResourceMetadata resource)
    {
        var ancestors = model.ResourceAncestors.GetAncestors(resource);
        var nameExpression = resource.NameSyntax;

        return ancestors
            .Select((x, i) => GetResourceNameAncestorSyntaxSegment(model, resource, i))
            .Concat(nameExpression);
    }

    /// <summary>
    /// Calculates the expression that represents the parent name corresponding to the specified ancestor of the specified resource.
    /// The expressions returned are modified by performing the necessary local variable replacements.
    /// </summary>
    /// <param name="model">The model in which the resource is declared.</param>
    /// <param name="resource">The declared resource metadata</param>
    /// <param name="startingAncestorIndex">the index of the ancestor (0 means the ancestor closest to the root)</param>
    private static SyntaxBase GetResourceNameAncestorSyntaxSegment(SemanticModel model, DeclaredResourceMetadata resource, int startingAncestorIndex)
    {
        var ancestors = model.ResourceAncestors.GetAncestors(resource);
        if (startingAncestorIndex >= ancestors.Length)
        {
            // not enough ancestors
            throw new ArgumentException($"Resource type has {ancestors.Length} ancestor types but name expression was requested for ancestor type at index {startingAncestorIndex}.");
        }

        /*
            * Consider the following example:
            *
            * resource one 'MS.Example/ones@...' = [for (_, i) in range(0, ...) : {
            *   name: name_exp1(i)
            * }]
            *
            * resource two 'MS.Example/ones/twos@...' = [for (_, j) in range(0, ...) : {
            *   parent: one[index_exp2(j)]
            *   name: name_exp2(j)
            * }]
            *
            * resource three 'MS.Example/ones/twos/threes@...' = [for (_, k) in range(0, ...) : {
            *   parent: two[index_exp3(k)]
            *   name: name_exp3(k)
            * }]
            *
            * name_exp* and index_exp* are expressions represented here as functions
            *
            * The name segment expressions for "three" are the following:
            * 0. name_exp1(index_exp2(index_exp3(k)))
            * 1. name_exp2(index_exp3(k))
            * 2. name_exp3(k)
            *
            * (The formula can be generalized to more levels of nesting.)
            *
            * This function can be used to get 0 and 1 above by passing 0 or 1 respectively as the startingAncestorIndex.
            * The name segment 2 above must be obtained from the resource directly.
            *
            * Given that we don't have proper functions in our runtime AND that our expressions don't have side effects,
            * the formula is implemented via local variable replacement.
            */

        // the initial ancestor gives us the base expression
        SyntaxBase? rewritten = ancestors[startingAncestorIndex].Resource.NameSyntax;

        for (int i = startingAncestorIndex; i < ancestors.Length; i++)
        {
            var ancestor = ancestors[i];

            // local variable replacement will be done in Context of the next ancestor
            // or the resource itself if we're on the last ancestor
            var newContext = i < ancestors.Length - 1 ? ancestors[i + 1].Resource : resource;

            rewritten = MoveSyntax(model, rewritten, ancestor.IndexExpression, newContext.Symbol.NameIdentifier);
        }

        return rewritten;
    }

    public static SyntaxBase MoveSyntax(SemanticModel model, SyntaxBase original, SyntaxBase? indexExpression, SyntaxBase newParent)
    {
        DataFlowAnalyzer analyzer = new(model);
        var inaccessibleLocals = analyzer.GetInaccessibleLocalsAfterSyntaxMove(original, newParent);
        var inaccessibleLocalLoops = inaccessibleLocals.Select(local => GetEnclosingForExpression(model, local)).Distinct().ToList();

        switch (inaccessibleLocalLoops.Count)
        {
            case 0:
                /*
                    * Hardcoded index expression resulted in no more local vars to replace.
                    * We can just bail out with the result.
                    */
                return original;

            case 1 when indexExpression is not null:
                if (LocalSymbolDependencyVisitor.GetLocalSymbolDependencies(model, original).SingleOrDefault(s => s.LocalKind == LocalKind.ForExpressionItemVariable) is { } loopItemSymbol)
                {
                    // rewrite the straggler from previous iteration
                    // TODO: Nested loops will require DFA on the ForSyntax.Expression
                    original = SymbolReplacer.Replace(model, new Dictionary<Symbol, SyntaxBase> { [loopItemSymbol] = SyntaxFactory.CreateArrayAccess(GetEnclosingForExpression(model, loopItemSymbol).Expression, indexExpression) }, original);
                }

                // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                var @for = inaccessibleLocalLoops.Single();

                var replacements = inaccessibleLocals.ToDictionary(local => (Symbol)local, local => local.LocalKind switch
                {
                    LocalKind.ForExpressionIndexVariable => indexExpression,
                    LocalKind.ForExpressionItemVariable => SyntaxFactory.CreateArrayAccess(@for.Expression, indexExpression),
                    _ => throw new NotImplementedException($"Unexpected local kind '{local.LocalKind}'.")
                });

                return SymbolReplacer.Replace(model, replacements, original);

            default:
                throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index expression rewriting.");
        }
    }

    public void EmitResourceScopeProperties(ExpressionEmitter expressionEmitter, DeclaredResourceExpression resource)
    {
        if (resource.ScopeData.ResourceScope is DeclaredResourceMetadata scopeResource)
        {
            // emit the resource id of the resource being extended
            var indexContext = TryGetReplacementContext(scopeResource, resource.ScopeData.IndexExpression, resource.BodySyntax);
            expressionEmitter.EmitProperty("scope", () => expressionEmitter.EmitFullyQualifiedResourceId(scopeResource, indexContext));
            return;
        }

        EmitResourceOrModuleScopeProperties(resource.ScopeData, expressionEmitter, resource.BodySyntax);
    }

    /*public void EmitResourceOptionsProperties(ExpressionEmitter expressionEmitter, DeclaredResourceExpression resource)
    {
        if (resource.SourceSyntax is FunctionCallSyntax functionCallSyntax)
        {
            // emit the resource id of the resource being extended
            var indexContext = TryGetReplacementContext(scopeResource, resource.ScopeData.IndexExpression, resource.BodySyntax);
            expressionEmitter.EmitProperty("parent", () => expressionEmitter.EmitUnqualifiedResourceId(scopeResource, indexContext));
            return;
        }
        EmitResourceOrModuleScopeProperties(resource.ScopeData, expressionEmitter, resource.BodySyntax);
    }
*/
    public void EmitModuleScopeProperties(ExpressionEmitter expressionEmitter, DeclaredModuleExpression module)
    {
        EmitResourceOrModuleScopeProperties(module.ScopeData, expressionEmitter, module.BodySyntax);
    }

    private void EmitResourceOrModuleScopeProperties(ScopeData scopeData, ExpressionEmitter expressionEmitter, SyntaxBase newContext)
    {
        switch (scopeData.RequestedScope)
        {
            case ResourceScope.Tenant:
                if (Context.SemanticModel.TargetScope != ResourceScope.Tenant)
                {
                    // emit the "/" to allow cross-scope deployment of a Tenant resource from another deployment scope
                    expressionEmitter.EmitProperty("scope", new JTokenExpression("/"));
                }
                return;
            case ResourceScope.ManagementGroup:
                if (scopeData.ManagementGroupNameProperty is not null)
                {
                    // The template engine expects an unqualified resourceId for the management group scope if deploying at tenant or management group scope
                    var useFullyQualifiedResourceId = Context.SemanticModel.TargetScope != ResourceScope.Tenant && Context.SemanticModel.TargetScope != ResourceScope.ManagementGroup;

                    var indexContext = TryGetReplacementContext(scopeData.ManagementGroupNameProperty, scopeData.IndexExpression, newContext);
                    expressionEmitter.EmitProperty("scope", expressionEmitter.GetManagementGroupResourceId(scopeData.ManagementGroupNameProperty, indexContext, useFullyQualifiedResourceId));
                }
                return;
            case ResourceScope.Subscription:
                if (scopeData.SubscriptionIdProperty is not null)
                {
                    // TODO: It's very suspicious that this doesn't reference scopeData.IndexExpression
                    expressionEmitter.EmitProperty("subscriptionId", scopeData.SubscriptionIdProperty);
                }
                else if (Context.SemanticModel.TargetScope == ResourceScope.ResourceGroup)
                {
                    // TODO: It's very suspicious that this doesn't reference scopeData.IndexExpression
                    expressionEmitter.EmitProperty("subscriptionId", new FunctionExpression("subscription", [], [new JTokenExpression("subscriptionId")]));
                }
                return;
            case ResourceScope.ResourceGroup:
                if (scopeData.SubscriptionIdProperty is not null)
                {
                    var indexContext = TryGetReplacementContext(scopeData.SubscriptionIdProperty, scopeData.IndexExpression, newContext);
                    expressionEmitter.EmitProperty("subscriptionId", () => expressionEmitter.EmitExpression(scopeData.SubscriptionIdProperty, indexContext));
                }
                if (scopeData.ResourceGroupProperty is not null)
                {
                    var indexContext = TryGetReplacementContext(scopeData.ResourceGroupProperty, scopeData.IndexExpression, newContext);
                    expressionEmitter.EmitProperty("resourceGroup", () => expressionEmitter.EmitExpression(scopeData.ResourceGroupProperty, indexContext));
                }
                return;
            case ResourceScope.DesiredStateConfiguration:
            case ResourceScope.Local:
                // These scopes just changes the schema so there are no properties to emit.
                // We don't ever need to throw here because the feature is checked during scope validation.
                return;
            default:
                throw new InvalidOperationException($"Cannot format resourceId for scope {scopeData.RequestedScope}");
        }
    }

    private static long SafeConvertIntegerValue(IntegerLiteralSyntax @int, bool isNegative)
        => (@int.Value, isNegative) switch
        {
            ( <= long.MaxValue, false) => (long)@int.Value,
            ( <= long.MaxValue, true) => -(long)@int.Value,
            // long.MaxValue is 9223372036854775807, whereas long.MinValue is -9223372036854775808, hence this special-case check:
            (1UL + long.MaxValue, true) => long.MinValue,
            _ => throw new NotImplementedException($"Unexpected out-of-range integer value"),
        };
}
