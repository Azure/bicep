// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Intermediate;

public class ExpressionBuilder
{
    private readonly EmitterContext context;
    private readonly ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements;

    public ExpressionBuilder(EmitterContext context, ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements)
    {
        this.context = context;
        this.localReplacements = localReplacements;
    }

    public Expression Convert(SyntaxBase syntax)
    {
        switch (syntax)
        {
            case StringSyntax @string: {
                if (@string.TryGetLiteralValue() is {} stringValue)
                {
                    return new StringLiteralExpression(stringValue);
                }

                return new InterpolatedStringExpression(
                    @string.SegmentValues,
                    @string.Expressions.Select(Convert).ToImmutableArray());
            }
            case IntegerLiteralSyntax @int: {
                var literalValue = @int.Value switch {
                    <= long.MaxValue => (long)@int.Value,
                    // Should have been caught earlier in validation.
                    _ => throw new NotImplementedException($"Unexpected out-of-range integer value"),
                };

                return new IntegerLiteralExpression(literalValue);
            }
            case UnaryOperationSyntax { Operator: UnaryOperator.Minus } unary when unary.Expression is IntegerLiteralSyntax @int: {
                var literalValue = @int.Value switch {
                    <= long.MaxValue => -(long)@int.Value,
                    (ulong)long.MaxValue + 1 => long.MinValue,
                    // Should have been caught earlier in validation.
                    _ => throw new NotImplementedException($"Unexpected out-of-range integer value"),
                };
                return new IntegerLiteralExpression(literalValue);
            }
            case BooleanLiteralSyntax @bool:
                return new BooleanLiteralExpression(@bool.Value);
            case NullLiteralSyntax:
                return new NullLiteralExpression();
            case ParenthesizedExpressionSyntax x:
                return Convert(x.Expression);
            case ObjectSyntax @object:
                var properties = @object.Properties.Select(x => new ObjectProperty(Convert(x.Key), Convert(x.Value)));
                return new ObjectExpression(properties.ToImmutableArray());
            case ArraySyntax array:
                var items = array.Items.Select(x => Convert(x.Value));
                return new ArrayExpression(items.ToImmutableArray());
            case TernaryOperationSyntax ternary:
                return new TernaryExpression(
                    Convert(ternary.ConditionExpression),
                    Convert(ternary.TrueExpression),
                    Convert(ternary.FalseExpression));
            case BinaryOperationSyntax binary:
                return new BinaryExpression(
                    binary.Operator,
                    Convert(binary.LeftExpression),
                    Convert(binary.RightExpression));
            case UnaryOperationSyntax unary:
                return new UnaryExpression(
                    unary.Operator,
                    Convert(unary.Expression));
            case FunctionCallSyntaxBase function:
                return ConvertFunction(function);
            case ArrayAccessSyntax arrayAccess:
                // TODO: should we lower an arrayAccess with constant property name to a propertyAccess?
                return ConvertArrayAccess(arrayAccess);
            case PropertyAccessSyntax propertyAccess:
                return ConvertPropertyAccess(propertyAccess);
            case UnboundVariableAccessSyntax unboundVariableAccess:
                var varProperties = new Expression[] { new StringLiteralExpression(unboundVariableAccess.Name.IdentifierName) };
                return new FunctionCallExpression("variables", varProperties.ToImmutableArray());
            case VariableAccessSyntax variableAccess:
                return ConvertVariableAccess(variableAccess);
            case ResourceAccessSyntax resourceAccess:
                return ConvertResourceAccess(resourceAccess);
            case LambdaSyntax lambda:
                var variableNames = lambda.GetLocalVariables().Select(x => x.Name.IdentifierName);

                return new LambdaExpression(
                    variableNames.ToImmutableArray(),
                    Convert(lambda.Body));
            default:
                return new SyntaxExpression(syntax);
        }
    }

    private Expression ConvertFunction(FunctionCallSyntaxBase functionCall)
    {
        var symbol = context.SemanticModel.GetSymbolInfo(functionCall);
        if (symbol is FunctionSymbol &&
            context.SemanticModel.TypeManager.GetMatchedFunctionOverload(functionCall) is { Evaluator: { } } functionOverload)
        {
            return Convert(functionOverload.Evaluator(functionCall,
                symbol,
                context.SemanticModel.GetTypeInfo(functionCall),
                context.FunctionVariables.GetValueOrDefault(functionCall),
                context.SemanticModel.TypeManager.GetMatchedFunctionResultValue(functionCall)));
        }

        switch (functionCall)
        {
            case FunctionCallSyntax function:
                return new FunctionCallExpression(
                    function.Name.IdentifierName,
                    function.Arguments.Select(a => Convert(a.Expression)).ToImmutableArray());

            case InstanceFunctionCallSyntax method:
                var (baseSyntax, indexExpression) = SyntaxHelper.UnwrapArrayAccessSyntax(method.BaseExpression);
                var baseSymbol = context.SemanticModel.GetSymbolInfo(baseSyntax);

                switch (baseSymbol)
                {
                    case INamespaceSymbol namespaceSymbol:
                        Debug.Assert(indexExpression is null, "Indexing into a namespace should have been blocked by type analysis");
                        return new FunctionCallExpression(
                            method.Name.IdentifierName,
                            method.Arguments.Select(a => Convert(a.Expression)).ToImmutableArray());
                    case { } _ when context.SemanticModel.ResourceMetadata.TryLookup(baseSyntax) is DeclaredResourceMetadata resource:
                        if (method.Name.IdentifierName.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix))
                        {
                            var indexContext = TryGetReplacementContext(resource.NameSyntax, indexExpression, method);

                            // Handle list<method_name>(...) method on resource symbol - e.g. stgAcc.listKeys()
                            var convertedArgs = method.Arguments.SelectArray(a => Convert(a.Expression));
                            var resourceIdExpression = new ResourceIdExpression(resource, indexContext);

                            var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");
                            var apiVersionExpression = new StringLiteralExpression(apiVersion);

                            var listArgs = convertedArgs.Length switch
                            {
                                0 => new Expression[] { resourceIdExpression, apiVersionExpression, },
                                _ => new Expression[] { resourceIdExpression, }.Concat(convertedArgs),
                            };

                            return new FunctionCallExpression(
                                method.Name.IdentifierName,
                                listArgs.ToImmutableArray());
                        }

                        break;
                }
                throw new InvalidOperationException($"Unrecognized base expression {baseSymbol?.Kind}");
            default:
                throw new NotImplementedException($"Cannot emit unexpected expression of type {functionCall.GetType().Name}");
        }
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
                return new ResourceReferenceExpression(resource, indexContext);
            }

            if (context.SemanticModel.GetSymbolInfo(arrayAccess.BaseExpression) is ModuleSymbol { IsCollection: true } moduleSymbol)
            {
                var indexContext = TryGetReplacementContext(GetModuleNameSyntax(moduleSymbol), arrayAccess.IndexExpression, arrayAccess);
                return new ModuleReferenceExpression(moduleSymbol, indexContext);
            }
        }

        return new ArrayAccessExpression(
            Convert(arrayAccess.BaseExpression),
            Convert(arrayAccess.IndexExpression));
    }

    private Expression ConvertResourcePropertyAccess(ResourceMetadata resource, IndexReplacementContext? indexContext, string propertyName)
    {
        return new PropertyAccessExpression(
            new ResourceReferenceExpression(resource, indexContext),
            propertyName);
    }

    private Expression ConvertPropertyAccess(PropertyAccessSyntax propertyAccess)
    {
        // Looking for: myResource.someProp (where myResource is a resource declared in-file)
        if (context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is DeclaredResourceMetadata resource)
        {
            return ConvertResourcePropertyAccess(resource, null, propertyAccess.PropertyName.IdentifierName);
        }

        // Looking for: myResource[blah].someProp (where myResource is a resource declared in-file)
        if (propertyAccess.BaseExpression is ArrayAccessSyntax propArrayAccess &&
            context.SemanticModel.ResourceMetadata.TryLookup(propArrayAccess.BaseExpression) is DeclaredResourceMetadata resourceCollection)
        {
            var indexContext = TryGetReplacementContext(resourceCollection, propArrayAccess.IndexExpression, propertyAccess);
            return ConvertResourcePropertyAccess(resourceCollection, indexContext, propertyAccess.PropertyName.IdentifierName);
        }

        // Looking for: myResource.someProp (where myResource is a parameter of type resource)
        if (context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ParameterResourceMetadata parameter)
        {
            return ConvertResourcePropertyAccess(parameter, null, propertyAccess.PropertyName.IdentifierName);
        }

        // Looking for: myMod.outputs.someProp (where someProp is an output of type resource)
        if (propertyAccess.BaseExpression is PropertyAccessSyntax &&
            context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleOutput &&
            !moduleOutput.Module.IsCollection)
        {
            return ConvertResourcePropertyAccess(moduleOutput, null, propertyAccess.PropertyName.IdentifierName);
        }

        // Looking for: myMod[blah].outputs.someProp (where someProp is an output of type resource)
        if (propertyAccess.BaseExpression is PropertyAccessSyntax moduleCollectionOutputProperty &&
            moduleCollectionOutputProperty.BaseExpression is PropertyAccessSyntax moduleCollectionOutputs &&
            moduleCollectionOutputs.BaseExpression is ArrayAccessSyntax moduleArrayAccess &&
            context.SemanticModel.ResourceMetadata.TryLookup(propertyAccess.BaseExpression) is ModuleOutputResourceMetadata moduleCollectionOutputMetadata &&
            moduleCollectionOutputMetadata.Module.IsCollection)
        {
            var indexContext = TryGetReplacementContext(moduleCollectionOutputMetadata.NameSyntax, moduleArrayAccess.IndexExpression, propertyAccess);
            return ConvertResourcePropertyAccess(moduleCollectionOutputMetadata, indexContext, propertyAccess.PropertyName.IdentifierName);
        }

        // Looking for: expr.outputs.blah (where expr is any expression of type module)
        if (propertyAccess.BaseExpression is PropertyAccessSyntax childPropertyAccess &&
            TypeHelper.SatisfiesCondition(context.SemanticModel.GetTypeInfo(childPropertyAccess.BaseExpression), x => x is ModuleType) &&
            childPropertyAccess.PropertyName.NameEquals(LanguageConstants.ModuleOutputsPropertyName))
        {
            return new ModuleOutputPropertyAccessExpression(
                Convert(propertyAccess.BaseExpression),
                propertyAccess.PropertyName.IdentifierName);
        }

        return new PropertyAccessExpression(
            Convert(propertyAccess.BaseExpression),
            propertyAccess.PropertyName.IdentifierName);
    }

    private Expression ConvertResourceAccess(ResourceAccessSyntax resourceAccessSyntax)
    {
        if (context.SemanticModel.ResourceMetadata.TryLookup(resourceAccessSyntax) is { } resource)
        {
            return new ResourceReferenceExpression(resource, null);
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
                return new ResourceReferenceExpression(resource, null);

            case ParameterSymbol parameterSymbol:
                return new ParametersReferenceExpression(parameterSymbol);

            case VariableSymbol variableSymbol:
                return new VariableReferenceExpression(variableSymbol);

            case ModuleSymbol moduleSymbol:
                return new ModuleReferenceExpression(moduleSymbol, null);

            case LocalVariableSymbol localVariableSymbol:
                return GetLocalVariableExpression(localVariableSymbol);

            default:
                throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' when generating a variable access expression.");

        }
    }

    private Expression GetLocalVariableExpression(LocalVariableSymbol localVariableSymbol)
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
                return GetLoopVariable(localVariableSymbol, @for, new CopyIndexExpression(GetCopyIndexName(@for)));
            case LambdaSyntax lambda:
                return new LambdaVariableReferenceExpression(localVariableSymbol);
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
        var forExpression = Convert(@for.Expression);

        return new ArrayAccessExpression(forExpression, index);
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

    public ForSyntax GetEnclosingForExpression(LocalVariableSymbol localVariable)
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

                return new(this.localReplacements, Convert(indexExpression));

            case 1 when indexExpression is not null:
                // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                var @for = inaccessibleLocalLoops.Single();
                var localReplacements = this.localReplacements;
                var builder = new ExpressionBuilder(this.context, localReplacements);
                foreach (var local in inaccessibleLocals)
                {
                    var replacementValue = GetLoopVariable(local, @for, builder.Convert(indexExpression));
                    localReplacements = localReplacements.SetItem(local, replacementValue);
                }

                return new(localReplacements, builder.Convert(indexExpression));

            default:
                throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index replacement.");
        }
    }

    public static SyntaxBase GetModuleNameSyntax(ModuleSymbol moduleSymbol)
    {
        // this condition should have already been validated by the type checker
        return moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName)
            ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
    }
}
