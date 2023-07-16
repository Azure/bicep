// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.DataFlow;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using static Bicep.Core.Emit.ScopeHelper;

namespace Bicep.Core.Intermediate;

public class ExpressionBuilder
{
    private static readonly ImmutableHashSet<string> NonAzResourcePropertiesToOmit = new[] {
        LanguageConstants.ResourceDependsOnPropertyName,
    }.ToImmutableHashSet();

    private static readonly ImmutableHashSet<string> AzResourcePropertiesToOmit = new[] {
        AzResourceTypeProvider.ResourceNamePropertyName,
        LanguageConstants.ResourceScopePropertyName,
        LanguageConstants.ResourceParentPropertyName,
        LanguageConstants.ResourceDependsOnPropertyName,
    }.ToImmutableHashSet();

    private static readonly ImmutableHashSet<string> ModulePropertiesToOmit = new[] {
        LanguageConstants.ModuleParamsPropertyName,
        LanguageConstants.ResourceScopePropertyName,
        LanguageConstants.ResourceDependsOnPropertyName,
    }.ToImmutableHashSet();

    private readonly ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements;

    public EmitterContext Context { get; }

    public ExpressionBuilder(EmitterContext Context, ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements)
    {
        this.Context = Context;
        this.localReplacements = localReplacements;
    }

    public ExpressionBuilder(EmitterContext Context)
        : this(Context, ImmutableDictionary<LocalVariableSymbol, Expression>.Empty)
    {
    }

    public Expression Convert(SyntaxBase syntax)
    {
        var expresion = ConvertWithoutLowering(syntax);

        return ExpressionLoweringVisitor.Lower(expresion);
    }

    private Expression ConvertWithoutLowering(SyntaxBase syntax)
    {
        switch (syntax)
        {
            case StringSyntax @string: {
                if (@string.TryGetLiteralValue() is {} stringValue)
                {
                    return new StringLiteralExpression(@string, stringValue);
                }

                return new InterpolatedStringExpression(
                    @string,
                    @string.SegmentValues,
                    @string.Expressions.Select(ConvertWithoutLowering).ToImmutableArray());
            }
            case IntegerLiteralSyntax @int: {
                var literalValue = SafeConvertIntegerValue(@int, isNegative: false);
                return new IntegerLiteralExpression(@int, literalValue);
            }
            case UnaryOperationSyntax { Operator: UnaryOperator.Minus } unary when unary.Expression is IntegerLiteralSyntax @int: {
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
                var items = array.Items.Select(x => ConvertWithoutLowering(x.Value));
                return new ArrayExpression(array, items.ToImmutableArray());
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
                    variables.Select(x => x.Name.IdentifierName).ToImmutableArray(),
                    variables.Select<LocalVariableSyntax, TypeExpression?>(x => null).ToImmutableArray(),
                    ConvertWithoutLowering(lambda.Body),
                    null);
            case TypedLambdaSyntax lambda:
                var typedVariables = lambda.GetLocalVariables();

                return new LambdaExpression(
                    lambda,
                    typedVariables.Select(x => x.Name.IdentifierName).ToImmutableArray(),
                    typedVariables.Select(x => ConvertTypeWithoutLowering(x.Type)).ToImmutableArray<TypeExpression?>(),
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

            case ImportDeclarationSyntax import:
                var symbol = GetDeclaredSymbol<ImportedNamespaceSymbol>(import);
                return EvaluateDecorators(import, new DeclaredImportExpression(
                    import,
                    symbol.Name,
                    GetTypeInfo<NamespaceType>(import),
                    import.Config is not null ? ConvertWithoutLowering(import.Config) : null));

            case ParameterDeclarationSyntax parameter:
                return EvaluateDecorators(parameter, new DeclaredParameterExpression(
                    parameter,
                    parameter.Name.IdentifierName,
                    ConvertTypeWithoutLowering(parameter.Type),
                    GetDeclaredSymbol<ParameterSymbol>(parameter),
                    parameter.Modifier is ParameterDefaultValueSyntax defaultValue ? ConvertWithoutLowering(defaultValue.DefaultValue) : null));

            case VariableDeclarationSyntax variable:
                return EvaluateDecorators(variable, new DeclaredVariableExpression(
                    variable,
                    variable.Name.IdentifierName,
                    ConvertWithoutLowering(variable.Value)));

            case FunctionDeclarationSyntax function:
                return EvaluateDecorators(function, new DeclaredFunctionExpression(
                    function,
                    function.Name.IdentifierName,
                    ConvertWithoutLowering(function.Lambda)));

            case OutputDeclarationSyntax output:
                return EvaluateDecorators(output, new DeclaredOutputExpression(
                    output,
                    output.Name.IdentifierName,
                    ConvertTypeWithoutLowering(output.Type),
                    GetDeclaredSymbol<OutputSymbol>(output),
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
                    GetDeclaredSymbol<TypeAliasSymbol>(typeDeclaration),
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
    {
        switch (Context.SemanticModel.Binder.GetSymbolInfo(syntax))
        {
            case AmbientTypeSymbol ambientType:
                return new AmbientTypeReferenceExpression(syntax, ambientType);
            case TypeAliasSymbol typeAlias:
                return new TypeAliasReferenceExpression(syntax, typeAlias);
            case Symbol otherwise:
                throw new ArgumentException($"Encountered unexpected symbol of type {otherwise.GetType()} in a type expression.");
        }

        switch (Context.SemanticModel.GetTypeInfo(syntax))
        {
            case StringLiteralType @string:
                return new StringLiteralTypeExpression(syntax, @string);
            case IntegerLiteralType @int:
                return new IntegerLiteralTypeExpression(syntax, @int);
            case BooleanLiteralType @bool:
                return new BooleanLiteralTypeExpression(syntax, @bool);
            case NullType @null:
                return new NullLiteralTypeExpression(syntax, @null);
            case ResourceType resource:
                return new ResourceTypeExpression(syntax, resource);
        }
        var symbol = Context.SemanticModel.GetSymbolInfo(syntax);
        var typeInfo = Context.SemanticModel.GetTypeInfo(syntax);

        return syntax switch
        {
            ObjectTypeSyntax objectTypeSyntax => new ObjectTypeExpression(syntax,
                GetTypeInfo<ObjectType>(syntax),
                objectTypeSyntax.Properties.Select(p => ConvertWithoutLowering<ObjectTypePropertyExpression>(p)).ToImmutableArray(),
                objectTypeSyntax.AdditionalProperties is SyntaxBase addlPropertiesSyntax
                    ? ConvertWithoutLowering<ObjectTypeAdditionalPropertiesExpression>(addlPropertiesSyntax)
                    : null),
            TupleTypeSyntax tupleTypeSyntax => new TupleTypeExpression(syntax,
                GetTypeInfo<TupleType>(syntax),
                tupleTypeSyntax.Items.Select(i => ConvertWithoutLowering<TupleTypeItemExpression>(i)).ToImmutableArray()),
            ArrayTypeSyntax arrayTypeSyntax => new ArrayTypeExpression(syntax,
                GetTypeInfo<ArrayType>(syntax),
                ConvertTypeWithoutLowering(arrayTypeSyntax.Item.Value)),
            NullableTypeSyntax nullableTypeSyntax => new NullableTypeExpression(syntax, ConvertTypeWithoutLowering(nullableTypeSyntax.Base)),
            UnionTypeSyntax unionTypeSyntax => new UnionTypeExpression(syntax,
                GetTypeInfo<UnionType>(syntax),
                unionTypeSyntax.Members.Select(m => ConvertTypeWithoutLowering(m.Value)).ToImmutableArray()),
            ParenthesizedExpressionSyntax parenthesizedExpression => ConvertTypeWithoutLowering(parenthesizedExpression.Expression),
            NonNullAssertionSyntax nonNullAssertion => new NonNullableTypeExpression(nonNullAssertion, ConvertTypeWithoutLowering(nonNullAssertion.BaseExpression)),
            PropertyAccessSyntax propertyAccess when Context.SemanticModel.GetSymbolInfo(propertyAccess.BaseExpression) is BuiltInNamespaceSymbol namespaceSymbol &&
                namespaceSymbol.TryGetNamespaceType() is NamespaceType namespaceType &&
                namespaceType.TryGetTypeProperty(propertyAccess.PropertyName.IdentifierName) is {} property
                => new FullyQualifiedAmbientTypeReferenceExpression(propertyAccess, namespaceSymbol, property),
            _ => throw new ArgumentException($"Failed to convert syntax of type {syntax.GetType()}"),
        };
    }

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

        var imports = Context.SemanticModel.Root.ImportDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredImportExpression>()
            .ToImmutableArray();

        var typeDefintions = Context.SemanticModel.Root.TypeDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredTypeExpression>()
            .ToImmutableArray();

        var parameters = Context.SemanticModel.Root.ParameterDeclarations
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredParameterExpression>()
            .ToImmutableArray();

        var functionVariables = Context.FunctionVariables
            .OrderBy(x => x.Value.Name, LanguageConstants.IdentifierComparer)
            .Select(x => new DeclaredVariableExpression(x.Key, x.Value.Name, x.Value.Value))
            .ToImmutableArray();

        var variables = Context.SemanticModel.Root.VariableDeclarations
            .Where(x => !Context.VariablesToInline.Contains(x))
            .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
            .OfType<DeclaredVariableExpression>()
            .ToImmutableArray();

        var resources = Context.SemanticModel.DeclaredResources
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
            imports,
            typeDefintions,
            parameters,
            functionVariables.AddRange(variables),
            functions,
            resources,
            modules,
            outputs,
            asserts);
    }

    private record LoopExpressionContext(string Name, SyntaxBase SourceSyntax, Expression LoopExpression);

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
            .Where(x => x.TryGetKeyText() is not {} key || !ModulePropertiesToOmit.Contains(key))
            .Select(ConvertObjectProperty);
        Expression bodyExpression = new ObjectExpression(body, properties.ToImmutableArray());
        var parameters = objectBody.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName);

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

        var dependencies = Context.ResourceDependencies[symbol]
            .Where(ShouldGenerateDependsOn)
            .OrderBy(x => x.Resource.Name) // order to generate a deterministic template
            .Select(x => ToDependencyExpression(x, body))
            .ToImmutableArray();

        return new DeclaredModuleExpression(
            syntax,
            symbol,
            Context.ModuleScopeData[symbol],
            body,
            bodyExpression,
            parameters is not null ? ConvertWithoutLowering(parameters.Value) : null,
            dependencies);
    }

    private DeclaredResourceExpression ConvertResource(ResourceDeclarationSyntax syntax)
    {
        var resource = Context.SemanticModel.ResourceMetadata.TryLookup(syntax) as DeclaredResourceMetadata
            ?? throw new InvalidOperationException("Failed to find resource in cache");

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
        // being evaulated in the template.
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
                AddLoop(new(ancestor.Resource.Symbol.Name, ancestorFor, ConvertWithoutLowering(ancestorFor.Expression)));
            }
        }

        // Unwrap the 'real' resource body if there's a condition / for loop
        var body = resource.Symbol.DeclaringResource.Value;
        if (body is ForSyntax @for)
        {
            AddLoop(new(resource.Symbol.Name, @for, ConvertWithoutLowering(@for.Expression)));
            body = @for.Body;
        }

        if (body is IfConditionSyntax @if)
        {
            AddCondition(ConvertWithoutLowering(@if.ConditionExpression));
            body = @if.Body;
        }

        var propertiesToOmit = resource.IsAzResource ? AzResourcePropertiesToOmit : NonAzResourcePropertiesToOmit;
        var properties = ((ObjectSyntax)body).Properties
            .Where(x => x.TryGetKeyText() is not {} key || !propertiesToOmit.Contains(key))
            .Select(ConvertObjectProperty);
        Expression bodyExpression = new ObjectExpression(body, properties.ToImmutableArray());

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

        var dependencies = Context.ResourceDependencies[resource.Symbol]
            .Where(ShouldGenerateDependsOn)
            .OrderBy(x => x.Resource.Name) // order to generate a deterministic template
            .Select(x => ToDependencyExpression(x, body))
            .ToImmutableArray();

        return new DeclaredResourceExpression(
            syntax,
            resource,
            Context.ResourceScopeData[resource],
            body,
            bodyExpression,
            dependencies);
    }

    private ObjectExpression ConvertObject(ObjectSyntax @object)
        => new ObjectExpression(
            @object,
            @object.Properties.Select(ConvertObjectProperty).ToImmutableArray());

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
                    function.Arguments.Select(a => ConvertWithoutLowering(a.Expression)).ToImmutableArray());

            case InstanceFunctionCallSyntax method:
                var (baseSyntax, indexExpression) = SyntaxHelper.UnwrapArrayAccessSyntax(method.BaseExpression);
                var baseSymbol = Context.SemanticModel.GetSymbolInfo(baseSyntax);

                if (baseSymbol is INamespaceSymbol namespaceSymbol)
                {
                    Debug.Assert(indexExpression is null, "Indexing into a namespace should have been blocked by type analysis");
                    return new FunctionCallExpression(
                        method,
                        method.Name.IdentifierName,
                        method.Arguments.Select(a => ConvertWithoutLowering(a.Expression)).ToImmutableArray());
                }

                var resource = Context.SemanticModel.ResourceMetadata.TryLookup(baseSyntax);
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
                        method.Arguments.Select(a => ConvertWithoutLowering(a.Expression)).ToImmutableArray());
                }

                throw new InvalidOperationException($"Unrecognized base expression {baseSymbol?.Kind}");
            default:
                throw new NotImplementedException($"Cannot emit unexpected expression of type {functionCall.GetType().Name}");
        }
    }

    private Expression ConvertFunction(FunctionCallSyntaxBase functionCall)
    {
        if (Context.Settings.FileKind == BicepSourceFileKind.BicepFile &&
            Context.FunctionVariables.GetValueOrDefault(functionCall) is {} functionVariable)
        {
            return new SynthesizedVariableReferenceExpression(functionCall, functionVariable.Name);
        }

        if (Context.SemanticModel.GetSymbolInfo(functionCall) is DeclaredFunctionSymbol declaredFunction)
        {
            return new UserDefinedFunctionCallExpression(
                functionCall,
                functionCall.Name.IdentifierName,
                functionCall.Arguments.Select(a => ConvertWithoutLowering(a.Expression)).ToImmutableArray());
        }

        if (Context.SemanticModel.TypeManager.GetMatchedFunctionResultValue(functionCall) is {} functionValue)
        {
            return functionValue;
        }

        var converted = ConvertFunctionDirect(functionCall);
        if (converted is FunctionCallExpression convertedFunction &&
            Context.SemanticModel.TypeManager.GetMatchedFunctionOverload(functionCall) is { Evaluator: {} } functionOverload)
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

        // Looking for short-circuitable access chains
        if (!arrayAccess.IsSafeAccess && IsAccessExpressionSyntax(arrayAccess.BaseExpression))
        {
            if (convertedBase is AccessExpression baseAccess)
            {
                return new AccessChainExpression(arrayAccess, baseAccess, ImmutableArray.Create(convertedIndex));
            }

            if (convertedBase is AccessChainExpression accessChain)
            {
                return new AccessChainExpression(arrayAccess, accessChain.FirstLink, accessChain.AdditionalProperties.Append(convertedIndex).ToImmutableArray());
            }
        }

        return new ArrayAccessExpression(arrayAccess, convertedBase, convertedIndex, GetAccessExpressionFlags(arrayAccess, arrayAccess.SafeAccessMarker));
    }

    private bool IsAccessExpressionSyntax(SyntaxBase syntax) => syntax switch
    {
        AccessExpressionSyntax => true,

        // type transformations with no runtime representation should be unwrapped and inspected
        NonNullAssertionSyntax nonNullAssertion => IsAccessExpressionSyntax(nonNullAssertion.BaseExpression),

        _ => false,
    };

    private Expression ConvertResourcePropertyAccess(PropertyAccessSyntax sourceSyntax, ResourceMetadata resource, IndexReplacementContext? indexContext, string propertyName, AccessExpressionFlags flags)
        => ExpressionFactory.CreateResourcePropertyAccess(resource, indexContext, propertyName, sourceSyntax, flags);

    private Expression ConvertPropertyAccess(PropertyAccessSyntax propertyAccess)
    {
        var flags = GetAccessExpressionFlags(propertyAccess, propertyAccess.SafeAccessMarker);

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
            var indexContext = TryGetReplacementContext(moduleCollectionOutputMetadata.NameSyntax, moduleArrayAccess.IndexExpression, propertyAccess);
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
                flags);
        }

        var convertedBase = ConvertWithoutLowering(propertyAccess.BaseExpression);

        // Looking for short-circuitable access chains
        if (!propertyAccess.IsSafeAccess && IsAccessExpressionSyntax(propertyAccess.BaseExpression))
        {
            Expression nextLink = new StringLiteralExpression(propertyAccess.PropertyName, propertyAccess.PropertyName.IdentifierName);

            if (convertedBase is AccessExpression baseAccess)
            {
                return new AccessChainExpression(propertyAccess, baseAccess, ImmutableArray.Create(nextLink));
            }

            if (convertedBase is AccessChainExpression accessChain)
            {
                return new AccessChainExpression(propertyAccess, accessChain.FirstLink, accessChain.AdditionalProperties.Append(nextLink).ToImmutableArray());
            }
        }

        return new PropertyAccessExpression(
            propertyAccess,
            convertedBase,
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
        var name = variableAccessSyntax.Name.IdentifierName;

        var symbol = Context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

        switch (symbol)
        {
            case DeclaredSymbol declaredSymbol when Context.SemanticModel.ResourceMetadata.TryLookup(declaredSymbol.DeclaringSyntax) is {} resource:
                return new ResourceReferenceExpression(variableAccessSyntax, resource, null);

            case ParameterSymbol parameterSymbol:
                return new ParametersReferenceExpression(variableAccessSyntax, parameterSymbol);

            case ParameterAssignmentSymbol parameterSymbol:
                return new ParametersAssignmentReferenceExpression(variableAccessSyntax, parameterSymbol);

            case VariableSymbol variableSymbol:
                if (Context.VariablesToInline.Contains(variableSymbol))
                {
                    // we've got a runtime dependency, so we have to inline the variable usage
                    return ConvertWithoutLowering(variableSymbol.DeclaringVariable.Value);
                }

                return new VariableReferenceExpression(variableAccessSyntax, variableSymbol);

            case ModuleSymbol moduleSymbol:
                return new ModuleReferenceExpression(variableAccessSyntax, moduleSymbol, null);

            case LocalVariableSymbol localVariableSymbol:
                return GetLocalVariableExpression(variableAccessSyntax, localVariableSymbol);

            default:
                throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' when generating a variable access expression.");

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
        => TryGetReplacementContext(GetModuleNameSyntax(module), indexExpression, newContext);

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

    private static SyntaxBase GetModuleNameSyntax(ModuleSymbol moduleSymbol)
    {
        // this condition should have already been validated by the type checker
        return moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName)
            ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
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

    private static bool ShouldGenerateDependsOn(ResourceDependency dependency)
    {
        if (dependency.Kind == ResourceDependencyKind.Transitive)
        {
            // transitive dependencies do not have to be emitted
            return false;
        }

        return dependency.Resource switch
        {   // We only want to add a 'dependsOn' for resources being deployed in this file.
            ResourceSymbol resource => !resource.DeclaringResource.IsExistingResource(),
            ModuleSymbol => true,
            _ => throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}"),
        };
    }

    private ResourceDependencyExpression ToDependencyExpression(ResourceDependency dependency, SyntaxBase newContext)
    {
        switch (dependency.Resource)
        {
            case ResourceSymbol resource: {
                var metadata = Context.SemanticModel.ResourceMetadata.TryLookup(resource.DeclaringSyntax) as DeclaredResourceMetadata
                    ?? throw new InvalidOperationException("Failed to find resource in cache");

                var indexContext = (resource.IsCollection && dependency.IndexExpression is null) ? null :
                    TryGetReplacementContext(metadata, dependency.IndexExpression, newContext);

                var reference = new ResourceReferenceExpression(null, metadata, indexContext);
                return new ResourceDependencyExpression(null, reference);
            }
            case ModuleSymbol module: {
                var indexContext = (module.IsCollection && dependency.IndexExpression is null) ? null :
                    TryGetReplacementContext(module, dependency.IndexExpression, newContext);

                var reference = new ModuleReferenceExpression(null, module, indexContext);
                return new ResourceDependencyExpression(null, reference);
            }
            default:
                throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}");
        }
    }

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
            expressionEmitter.EmitProperty("scope", () => expressionEmitter.EmitUnqualifiedResourceId(scopeResource, indexContext));
            return;
        }

        EmitResourceOrModuleScopeProperties(resource.ScopeData, expressionEmitter, resource.BodySyntax);
    }

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
                    expressionEmitter.EmitProperty("subscriptionId", new FunctionExpression("subscription", Array.Empty<LanguageExpression>(), new LanguageExpression[] { new JTokenExpression("subscriptionId") }));
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
            default:
                throw new InvalidOperationException($"Cannot format resourceId for scope {scopeData.RequestedScope}");
        }
    }

    private static long SafeConvertIntegerValue(IntegerLiteralSyntax @int, bool isNegative)
        => (@int.Value, isNegative) switch {
            (<= long.MaxValue, false) => (long)@int.Value,
            (<= long.MaxValue, true) => -(long)@int.Value,
            // long.MaxValue is 9223372036854775807, whereas long.MinValue is -9223372036854775808, hence this special-case check:
            (1UL + long.MaxValue, true) => long.MinValue,
            _ => throw new NotImplementedException($"Unexpected out-of-range integer value"),
        };

    private static AccessExpressionFlags GetAccessExpressionFlags(SyntaxBase accessExpression, SyntaxBase? safeAccessMarker)
    {
        var flags = AccessExpressionFlags.None;
        if (safeAccessMarker is not null)
        {
            flags |= AccessExpressionFlags.SafeAccess;
        }

        return flags;
    }
}
