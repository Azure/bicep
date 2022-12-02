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
            case StringSyntax @string:
                if (@string.TryGetLiteralValue() is {} stringValue)
                {
                    return new StringLiteralExpression(stringValue);
                }

                return new InterpolatedStringExpression(
                    @string.SegmentValues,
                    @string.Expressions.Select(Convert).ToImmutableArray());
            case IntegerLiteralSyntax x when TryGetIntegerLiteralValue(x) is {} val:
                return new IntegerLiteralExpression(val);
            case UnaryOperationSyntax x when TryGetIntegerLiteralValue(x) is {} val:
                return new IntegerLiteralExpression(val);
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
            default:
                return new SyntaxExpression(syntax);
        }
    }

    private static long? TryGetIntegerLiteralValue(SyntaxBase syntax)
    {
        // We depend here on the fact that type validation has already checked that the integer is valid.
        return syntax switch {
            // Because integerSyntax.Value is a ulong type, it is always positive. we need to first cast it to a long in order to negate it.
            UnaryOperationSyntax { Operator: UnaryOperator.Minus } unary when unary.Expression is IntegerLiteralSyntax x => x.Value switch {
                <= long.MaxValue => -(long)x.Value,
                (ulong)long.MaxValue + 1 => long.MinValue,
                _ => null,
            },
            IntegerLiteralSyntax x => x.Value switch {
                <= long.MaxValue => (long)x.Value,
                _ => null,
            },
            _ => null,
        };
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
                            var resourceIdExpression = new ResourceIdExpression(StaticResourcePropertyKind.Id, resource, indexContext);

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

    public IndexReplacementContext? TryGetReplacementContext(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
    {
        var inaccessibleLocals = this.context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(nameSyntax, newContext);
        var inaccessibleLocalLoops = inaccessibleLocals.Select(local => GetEnclosingForExpression(local)).Distinct().ToList();

        switch (inaccessibleLocalLoops.Count)
        {
            case 0:
                // moving the name expression does not produce any inaccessible locals (no locals means no loops)
                // regardless if there is an index expression or not, we don't need to append replacements
                return null;

            case 1 when indexExpression is not null:
                // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                var @for = inaccessibleLocalLoops.Single();
                var localReplacements = this.localReplacements;
                foreach (var local in inaccessibleLocals)
                {
                    var replacementValue = GetLoopVariable(local, @for, Convert(indexExpression));
                    localReplacements = localReplacements.SetItem(local, replacementValue);
                }

                return new(localReplacements, Convert(indexExpression));

            default:
                throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index replacement.");
        }
    }
}
