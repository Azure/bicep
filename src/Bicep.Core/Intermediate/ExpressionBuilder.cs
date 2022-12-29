// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.DataFlow;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
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

    private readonly EmitterContext context;
    private readonly ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements;

    public ExpressionBuilder(EmitterContext context, ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements)
    {
        this.context = context;
        this.localReplacements = localReplacements;
    }

    public ExpressionBuilder(EmitterContext context)
        : this(context, ImmutableDictionary<LocalVariableSymbol, Expression>.Empty)
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
                var literalValue = @int.Value switch {
                    <= long.MaxValue => (long)@int.Value,
                    // Should have been caught earlier in validation.
                    _ => throw new NotImplementedException($"Unexpected out-of-range integer value"),
                };

                return new IntegerLiteralExpression(@int, literalValue);
            }
            case UnaryOperationSyntax { Operator: UnaryOperator.Minus } unary when unary.Expression is IntegerLiteralSyntax @int: {
                var literalValue = @int.Value switch {
                    <= long.MaxValue => -(long)@int.Value,
                    (ulong)long.MaxValue + 1 => long.MinValue,
                    // Should have been caught earlier in validation.
                    _ => throw new NotImplementedException($"Unexpected out-of-range integer value"),
                };
                return new IntegerLiteralExpression(unary, literalValue);
            }
            case BooleanLiteralSyntax @bool:
                return new BooleanLiteralExpression(@bool, @bool.Value);
            case NullLiteralSyntax:
                return new NullLiteralExpression(syntax);
            case ParenthesizedExpressionSyntax x:
                return ConvertWithoutLowering(x.Expression);
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
                var variableNames = lambda.GetLocalVariables().Select(x => x.Name.IdentifierName);

                return new LambdaExpression(
                    lambda,
                    variableNames.ToImmutableArray(),
                    ConvertWithoutLowering(lambda.Body));

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
                return new DeclaredMetadataExpression(
                    metadata,
                    metadata.Name.IdentifierName,
                    ConvertWithoutLowering(metadata.Value));

            case ImportDeclarationSyntax import:
                var symbol = GetDeclaredSymbol<ImportedNamespaceSymbol>(import);
                return new DeclaredImportExpression(
                    import,
                    symbol.Name,
                    GetTypeInfo<NamespaceType>(import),
                    import.Config is not null ? ConvertWithoutLowering(import.Config) : null);

            case ParameterDeclarationSyntax parameter:
                return new DeclaredParameterExpression(
                    parameter,
                    parameter.Name.IdentifierName,
                    GetDeclaredSymbol<ParameterSymbol>(parameter),
                    parameter.Modifier is ParameterDefaultValueSyntax defaultValue ? ConvertWithoutLowering(defaultValue.DefaultValue) : null);

            case VariableDeclarationSyntax variable:
                return new DeclaredVariableExpression(
                    variable,
                    variable.Name.IdentifierName,
                    ConvertWithoutLowering(variable.Value));

            case OutputDeclarationSyntax output:
                return new DeclaredOutputExpression(
                    output,
                    output.Name.IdentifierName,
                    GetDeclaredSymbol<OutputSymbol>(output),
                    ConvertWithoutLowering(output.Value));

            case ResourceDeclarationSyntax resource:
                return ConvertResource(resource);

            case ModuleDeclarationSyntax module:
                return ConvertModule(module);

            case ProgramSyntax program:
                var metadataArray = context.SemanticModel.Root.MetadataDeclarations
                    .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
                    .OfType<DeclaredMetadataExpression>()
                    .ToImmutableArray();

                var imports = context.SemanticModel.Root.ImportDeclarations
                    .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
                    .OfType<DeclaredImportExpression>()
                    .ToImmutableArray();

                var parameters = context.SemanticModel.Root.ParameterDeclarations
                    .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
                    .OfType<DeclaredParameterExpression>()
                    .ToImmutableArray();

                var variables = context.SemanticModel.Root.VariableDeclarations
                    .Where(x => !context.VariablesToInline.Contains(x))
                    .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
                    .OfType<DeclaredVariableExpression>()
                    .ToImmutableArray();

                var resources = context.SemanticModel.DeclaredResources
                    .Select(x => ConvertWithoutLowering(x.Symbol.DeclaringSyntax))
                    .OfType<DeclaredResourceExpression>()
                    .ToImmutableArray();

                var modules = context.SemanticModel.Root.ModuleDeclarations
                    .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
                    .OfType<DeclaredModuleExpression>()
                    .ToImmutableArray();

                var outputs = context.SemanticModel.Root.OutputDeclarations
                    .Select(x => ConvertWithoutLowering(x.DeclaringSyntax))
                    .OfType<DeclaredOutputExpression>()
                    .ToImmutableArray();

                return new ProgramExpression(
                    program,
                    metadataArray,
                    imports,
                    parameters,
                    variables,
                    resources,
                    modules,
                    outputs);

            default:
                throw new ArgumentException($"Failed to convert syntax of type {syntax.GetType()}");
        }
    }

    private TSymbol GetDeclaredSymbol<TSymbol>(SyntaxBase syntax)
        where TSymbol : DeclaredSymbol
    {
        if (!context.SemanticModel.Root.DeclarationsBySyntax.TryGetValue(syntax, out var symbol) ||
            symbol is not TSymbol declaredSymbol)
        {
            throw new ArgumentException($"Failed to find symbol for syntax of type {syntax.GetType()}");
        }

        return declaredSymbol;
    }

    private TType GetTypeInfo<TType>(SyntaxBase syntax)
        where TType : TypeSymbol
    {
        if (context.SemanticModel.GetTypeInfo(syntax) is not TType typeSymbol)
        {
            throw new ArgumentException($"Failed to find type symbol for syntax of type {syntax.GetType()}");
        }

        return typeSymbol;
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

        var dependencies = context.ResourceDependencies[symbol]
            .Where(ShouldGenerateDependsOn)
            .OrderBy(x => x.Resource.Name) // order to generate a deterministic template
            .Select(x => ToDependencyExpression(x, body))
            .ToImmutableArray();

        return new DeclaredModuleExpression(
            syntax,
            symbol,
            context.ModuleScopeData[symbol],
            body,
            bodyExpression,
            parameters is not null ? ConvertWithoutLowering(parameters.Value) : null,
            dependencies);
    }

    private DeclaredResourceExpression ConvertResource(ResourceDeclarationSyntax syntax)
    {
        var resource = context.SemanticModel.ResourceMetadata.TryLookup(syntax) as DeclaredResourceMetadata
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
        var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
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

        var dependencies = context.ResourceDependencies[resource.Symbol]
            .Where(ShouldGenerateDependsOn)
            .OrderBy(x => x.Resource.Name) // order to generate a deterministic template
            .Select(x => ToDependencyExpression(x, body))
            .ToImmutableArray();

        return new DeclaredResourceExpression(
            syntax,
            resource,
            context.ResourceScopeData[resource],
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
                var baseSymbol = context.SemanticModel.GetSymbolInfo(baseSyntax);

                switch (baseSymbol)
                {
                    case INamespaceSymbol namespaceSymbol:
                        Debug.Assert(indexExpression is null, "Indexing into a namespace should have been blocked by type analysis");
                        return new FunctionCallExpression(
                            method,
                            method.Name.IdentifierName,
                            method.Arguments.Select(a => ConvertWithoutLowering(a.Expression)).ToImmutableArray());
                    case { } _ when context.SemanticModel.ResourceMetadata.TryLookup(baseSyntax) is DeclaredResourceMetadata resource:
                        var indexContext = TryGetReplacementContext(resource.NameSyntax, indexExpression, method);

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
        if (context.FunctionVariables.GetValueOrDefault(functionCall) is {} functionVariable)
        {
            return new SynthesizedVariableReferenceExpression(functionCall, functionVariable.Name);
        }

        if (context.SemanticModel.TypeManager.GetMatchedFunctionResultValue(functionCall) is {} functionValue)
        {
            return functionValue;
        }

        var converted = ConvertFunctionDirect(functionCall);
        if (converted is FunctionCallExpression convertedFunction &&
            context.SemanticModel.TypeManager.GetMatchedFunctionOverload(functionCall) is { Evaluator: {} } functionOverload)
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
            if (context.SemanticModel.ResourceMetadata.TryLookup(arrayAccess.BaseExpression) is DeclaredResourceMetadata resource &&
                resource.Symbol.IsCollection)
            {
                var indexContext = TryGetReplacementContext(resource, arrayAccess.IndexExpression, arrayAccess);
                return new ResourceReferenceExpression(arrayAccess, resource, indexContext);
            }

            if (context.SemanticModel.GetSymbolInfo(arrayAccess.BaseExpression) is ModuleSymbol { IsCollection: true } moduleSymbol)
            {
                var indexContext = TryGetReplacementContext(moduleSymbol, arrayAccess.IndexExpression, arrayAccess);
                return new ModuleReferenceExpression(arrayAccess, moduleSymbol, indexContext);
            }
        }

        return new ArrayAccessExpression(
            arrayAccess,
            ConvertWithoutLowering(arrayAccess.BaseExpression),
            ConvertWithoutLowering(arrayAccess.IndexExpression));
    }

    private Expression ConvertResourcePropertyAccess(PropertyAccessSyntax sourceSyntax, ResourceMetadata resource, IndexReplacementContext? indexContext, string propertyName)
    {
        return new PropertyAccessExpression(
            sourceSyntax,
            new ResourceReferenceExpression(sourceSyntax.BaseExpression, resource, indexContext),
            propertyName);
    }

    private Expression ConvertPropertyAccess(PropertyAccessSyntax propertyAccess)
    {
        // Looking for: myResource.someProp (where myResource is a resource declared in-file)
        if (context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is DeclaredResourceMetadata resource)
        {
            return ConvertResourcePropertyAccess(propertyAccess, resource, null, propertyAccess.PropertyName.IdentifierName);
        }

        // Looking for: myResource[blah].someProp (where myResource is a resource declared in-file)
        if (propertyAccess.BaseExpression is ArrayAccessSyntax propArrayAccess &&
            context.SemanticModel.ResourceMetadata.TryLookup(propArrayAccess.BaseExpression) is DeclaredResourceMetadata resourceCollection)
        {
            var indexContext = TryGetReplacementContext(resourceCollection, propArrayAccess.IndexExpression, propertyAccess);
            return ConvertResourcePropertyAccess(propertyAccess, resourceCollection, indexContext, propertyAccess.PropertyName.IdentifierName);
        }

        // Looking for: myResource.someProp (where myResource is a parameter of type resource)
        if (context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ParameterResourceMetadata parameter)
        {
            return ConvertResourcePropertyAccess(propertyAccess, parameter, null, propertyAccess.PropertyName.IdentifierName);
        }

        // Looking for: myMod.outputs.someProp (where someProp is an output of type resource)
        if (propertyAccess.BaseExpression is PropertyAccessSyntax &&
            context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleOutput &&
            !moduleOutput.Module.IsCollection)
        {
            return ConvertResourcePropertyAccess(propertyAccess, moduleOutput, null, propertyAccess.PropertyName.IdentifierName);
        }

        // Looking for: myMod[blah].outputs.someProp (where someProp is an output of type resource)
        if (propertyAccess.BaseExpression is PropertyAccessSyntax moduleCollectionOutputProperty &&
            moduleCollectionOutputProperty.BaseExpression is PropertyAccessSyntax moduleCollectionOutputs &&
            moduleCollectionOutputs.BaseExpression is ArrayAccessSyntax moduleArrayAccess &&
            context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleCollectionOutputMetadata &&
            moduleCollectionOutputMetadata.Module.IsCollection)
        {
            var indexContext = TryGetReplacementContext(moduleCollectionOutputMetadata.NameSyntax, moduleArrayAccess.IndexExpression, propertyAccess);
            return ConvertResourcePropertyAccess(propertyAccess, moduleCollectionOutputMetadata, indexContext, propertyAccess.PropertyName.IdentifierName);
        }

        // Looking for: expr.outputs.blah (where expr is any expression of type module)
        if (propertyAccess.BaseExpression is PropertyAccessSyntax childPropertyAccess &&
            TypeHelper.SatisfiesCondition(context.SemanticModel.GetTypeInfo(childPropertyAccess.BaseExpression), x => x is ModuleType) &&
            childPropertyAccess.PropertyName.NameEquals(LanguageConstants.ModuleOutputsPropertyName))
        {
            return new ModuleOutputPropertyAccessExpression(
                propertyAccess,
                ConvertWithoutLowering(propertyAccess.BaseExpression),
                propertyAccess.PropertyName.IdentifierName);
        }

        return new PropertyAccessExpression(
            propertyAccess,
            ConvertWithoutLowering(propertyAccess.BaseExpression),
            propertyAccess.PropertyName.IdentifierName);
    }

    private Expression ConvertResourceAccess(ResourceAccessSyntax resourceAccessSyntax)
    {
        if (context.SemanticModel.ResourceMetadata.TryLookup(resourceAccessSyntax) is { } resource)
        {
            return new ResourceReferenceExpression(resourceAccessSyntax, resource, null);
        }

        throw new NotImplementedException($"Unable to obtain resource metadata when generating a resource access expression.");
    }

    private Expression ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax)
    {
        var name = variableAccessSyntax.Name.IdentifierName;

        var symbol = context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

        switch (symbol)
        {
            case DeclaredSymbol declaredSymbol when context.SemanticModel.ResourceMetadata.TryLookup(declaredSymbol.DeclaringSyntax) is {} resource:
                return new ResourceReferenceExpression(variableAccessSyntax, resource, null);

            case ParameterSymbol parameterSymbol:
                return new ParametersReferenceExpression(variableAccessSyntax, parameterSymbol);

            case VariableSymbol variableSymbol:
                if (context.VariablesToInline.Contains(variableSymbol))
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
            // the current context has specified an expression to be used for this local variable symbol
            // to override the regular conversion
            return replacement;
        }

        var enclosingSyntax = GetEnclosingDeclaringSyntax(localVariableSymbol);
        switch (enclosingSyntax)
        {
            case ForSyntax @for:
                return GetLoopVariable(localVariableSymbol, @for, new CopyIndexExpression(sourceSyntax, GetCopyIndexName(@for)));
            case LambdaSyntax lambda:
                return new LambdaVariableReferenceExpression(sourceSyntax, localVariableSymbol);
        }

        throw new NotImplementedException($"{nameof(LocalVariableSymbol)} was declared by an unexpected syntax type '{enclosingSyntax?.GetType().Name}'.");
    }

    private string? GetCopyIndexName(ForSyntax @for)
    {
        return this.context.SemanticModel.Binder.GetParent(@for) switch
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
        var symbolParent = this.context.SemanticModel.GetSymbolParent(localVariable);
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

        return new ArrayAccessExpression(index.SourceSyntax, forExpression, index);
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

    private ForSyntax GetEnclosingForExpression(LocalVariableSymbol localVariable)
    {
        // we're following the symbol hierarchy rather than syntax hierarchy because
        // this guarantees a single hop in all cases
        var symbolParent = this.context.SemanticModel.GetSymbolParent(localVariable);
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

    public IndexReplacementContext? TryGetReplacementContext(ModuleSymbol module, SyntaxBase? indexExpression, SyntaxBase newContext)
        => TryGetReplacementContext(GetModuleNameSyntax(module), indexExpression, newContext);

    public IndexReplacementContext? TryGetReplacementContext(ResourceMetadata resource, SyntaxBase? indexExpression, SyntaxBase newContext)
    {
        if (resource is not DeclaredResourceMetadata declaredResource)
        {
            return null;
        }

        var movedSyntax = context.Settings.EnableSymbolicNames ? declaredResource.Symbol.NameIdentifier : declaredResource.NameSyntax;

        return TryGetReplacementContext(movedSyntax, indexExpression, newContext);
    }

    public IndexReplacementContext? TryGetReplacementContext(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
    {
        var inaccessibleLocals = this.context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(nameSyntax, newContext);
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
                var builder = new ExpressionBuilder(this.context, localReplacements);
                foreach (var local in inaccessibleLocals)
                {
                    var replacementValue = GetLoopVariable(local, @for, builder.ConvertWithoutLowering(indexExpression));
                    localReplacements = localReplacements.SetItem(local, replacementValue);
                }

                return new(localReplacements, builder.ConvertWithoutLowering(indexExpression));

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
            context.SemanticModel,
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
                var metadata = context.SemanticModel.ResourceMetadata.TryLookup(resource.DeclaringSyntax) as DeclaredResourceMetadata
                    ?? throw new InvalidOperationException("Failed to find resource in cache");

                SyntaxBase movedSyntax = context.Settings.EnableSymbolicNames ? metadata.Symbol.NameIdentifier : SyntaxFactory.CreateArray(GetResourceNameSyntaxSegments(metadata));
                var indexContext = (resource.IsCollection && dependency.IndexExpression is null) ? null :
                    TryGetReplacementContext(movedSyntax, dependency.IndexExpression, newContext);

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
    {
        var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
        var nameExpression = resource.NameSyntax;

        return ancestors
            .Select((x, i) => GetResourceNameAncestorSyntaxSegment(resource, i))
            .Concat(nameExpression);
    }

    /// <summary>
    /// Calculates the expression that represents the parent name corresponding to the specified ancestor of the specified resource.
    /// The expressions returned are modified by performing the necessary local variable replacements.
    /// </summary>
    /// <param name="resource">The declared resource metadata</param>
    /// <param name="startingAncestorIndex">the index of the ancestor (0 means the ancestor closest to the root)</param>
    private SyntaxBase GetResourceNameAncestorSyntaxSegment(DeclaredResourceMetadata resource, int startingAncestorIndex)
    {
        var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
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

            // local variable replacement will be done in context of the next ancestor
            // or the resource itself if we're on the last ancestor
            var newContext = i < ancestors.Length - 1 ? ancestors[i + 1].Resource : resource;

            var inaccessibleLocals = this.context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(rewritten, newContext.Symbol.NameIdentifier);
            var inaccessibleLocalLoops = inaccessibleLocals.Select(local => GetEnclosingForExpression(local)).Distinct().ToList();

            switch (inaccessibleLocalLoops.Count)
            {
                case 0:
                    /*
                        * Hardcoded index expression resulted in no more local vars to replace.
                        * We can just bail out with the result.
                        */
                    return rewritten;

                case 1 when ancestor.IndexExpression is not null:
                    if (LocalSymbolDependencyVisitor.GetLocalSymbolDependencies(this.context.SemanticModel, rewritten).SingleOrDefault(s => s.LocalKind == LocalKind.ForExpressionItemVariable) is { } loopItemSymbol)
                    {
                        // rewrite the straggler from previous iteration
                        // TODO: Nested loops will require DFA on the ForSyntax.Expression
                        rewritten = SymbolReplacer.Replace(this.context.SemanticModel, new Dictionary<Symbol, SyntaxBase> { [loopItemSymbol] = SyntaxFactory.CreateArrayAccess(GetEnclosingForExpression(loopItemSymbol).Expression, ancestor.IndexExpression) }, rewritten);
                    }

                    // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                    var @for = inaccessibleLocalLoops.Single();

                    var replacements = inaccessibleLocals.ToDictionary(local => (Symbol)local, local => local.LocalKind switch
                            {
                                LocalKind.ForExpressionIndexVariable => ancestor.IndexExpression,
                                LocalKind.ForExpressionItemVariable => SyntaxFactory.CreateArrayAccess(@for.Expression, ancestor.IndexExpression),
                                _ => throw new NotImplementedException($"Unexpected local kind '{local.LocalKind}'.")
                            });

                    rewritten = SymbolReplacer.Replace(this.context.SemanticModel, replacements, rewritten);

                    break;

                default:
                    throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index expression rewriting.");
            }
        }

        return rewritten;
    }
}
