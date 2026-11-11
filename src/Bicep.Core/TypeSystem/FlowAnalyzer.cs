// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Numerics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem;

public class FlowAnalyzer(IBinder binder, ITypeManager typeManager)
{
    private readonly record struct FlowComparison(
        SyntaxBase Operand,
        FlowReference Reference,
        BinaryOperator Operator,
        FlowLiteral Literal,
        bool Negated);

    private readonly ConcurrentDictionary<SyntaxBase, FlowState> states = new();

    public bool IsConditionKnownTrue(SyntaxBase condition, SyntaxBase context)
        => TryGetKnownValue(condition, GetFlowState(context)) is true;

    public bool IsKnownNonNull(SyntaxBase syntax)
        => TryGetReference(syntax) is { } reference && GetFlowState(syntax).NonNullReferences.Contains(reference);

    public FlowState GetFlowState(SyntaxBase syntax) => states.GetOrAdd(syntax, ComputeFlowState);

    public bool IsWithinTernaryBranch(SyntaxBase syntax)
        => GetEnclosingTernaryBranches(syntax).Any();

    public bool IsInDeadTernaryBranch(SyntaxBase syntax)
        => GetEnclosingTernaryBranches(syntax).Any(pair =>
            typeManager.GetTypeInfo(pair.ternary.ConditionExpression) is BooleanLiteralType condition &&
            ReferenceEquals(pair.branch, condition.Value ? pair.ternary.FalseExpression : pair.ternary.TrueExpression));

    private IEnumerable<(TernaryOperationSyntax ternary, SyntaxBase branch)> GetEnclosingTernaryBranches(SyntaxBase syntax)
    {
        var current = syntax;
        while (binder.GetParent(current) is { } parent)
        {
            if (parent is TernaryOperationSyntax ternary &&
                (ReferenceEquals(current, ternary.TrueExpression) || ReferenceEquals(current, ternary.FalseExpression)))
            {
                yield return (ternary, current);
            }
            current = parent;
        }
    }

    private FlowState ComputeFlowState(SyntaxBase syntax)
    {
        if (binder.GetParent(syntax) is not { } parent)
        {
            return FlowState.Empty;
        }

        // Derive facts from the reference's location, not the order in which types are queried.
        var state = GetFlowState(parent);
        return parent switch
        {
            IfConditionSyntax conditional when ReferenceEquals(syntax, conditional.Body)
                => AddCondition(state, conditional.ConditionExpression, true),
            TernaryOperationSyntax ternary when ReferenceEquals(syntax, ternary.TrueExpression)
                => AddCondition(state, ternary.ConditionExpression, true),
            TernaryOperationSyntax ternary when ReferenceEquals(syntax, ternary.FalseExpression)
                => AddCondition(state, ternary.ConditionExpression, false),
            BinaryOperationSyntax binary when ReferenceEquals(syntax, binary.RightExpression) &&
                binary.Operator is BinaryOperator.LogicalAnd or BinaryOperator.LogicalOr
                => AddCondition(state, binary.LeftExpression, binary.Operator == BinaryOperator.LogicalAnd),
            _ => state,
        };
    }

    private FlowState AddCondition(FlowState state, SyntaxBase condition, bool value)
    {
        var (expression, negated) = UnwrapCondition(condition);
        var conditionValue = value != negated;

        if (TryGetComparison(expression) is { Literal: NullFlowLiteral } nullComparison)
        {
            state = state with { NonNullReferences = AddNullFact(state.NonNullReferences, nullComparison, conditionValue) };
        }

        if (TryGetPredicate(expression) is { } predicate)
        {
            var (operand, predicateNegated) = UnwrapPredicate(predicate);
            state = state with { PredicateValues = state.PredicateValues.SetItem(operand, conditionValue != predicateNegated) };
        }

        if (expression is BinaryOperationSyntax { Operator: BinaryOperator.LogicalAnd } and && conditionValue)
        {
            state = AddCondition(state, and.LeftExpression, true);
            state = AddCondition(state, and.RightExpression, true);
        }
        else if (expression is BinaryOperationSyntax { Operator: BinaryOperator.LogicalOr } or && !conditionValue)
        {
            state = AddCondition(state, or.LeftExpression, false);
            state = AddCondition(state, or.RightExpression, false);
        }

        return state;
    }

    private static ImmutableHashSet<FlowReference> AddNullFact(ImmutableHashSet<FlowReference> nonNull, FlowComparison comparison, bool conditionValue)
    {
        if (conditionValue != comparison.Negated)
        {
            // A null result doesn't say which part of the path is null, so only the exact reference is forgotten.
            return nonNull.Remove(comparison.Reference);
        }

        if (!ContainsSafeAccess(comparison.Operand))
        {
            return nonNull.Add(comparison.Reference);
        }

        // A non-null safe-access result proves every reference along the path is non-null.
        for (FlowReference? reference = comparison.Reference; reference is not null; reference = (reference as PropertyFlowReference)?.Parent)
        {
            nonNull = nonNull.Add(reference);
        }

        return nonNull;
    }

    private bool? TryGetKnownValue(SyntaxBase condition, FlowState state)
    {
        var (expression, negated) = UnwrapCondition(condition);

        if (TryGetPredicate(expression) is { } predicate)
        {
            var (operand, predicateNegated) = UnwrapPredicate(predicate);
            if (state.PredicateValues.TryGetValue(operand, out var knownValue))
            {
                return knownValue != (negated != predicateNegated);
            }
        }

        bool? result = expression switch
        {
            BooleanLiteralSyntax literal => literal.Value,
            BinaryOperationSyntax { Operator: BinaryOperator.LogicalAnd } and
                => And(TryGetKnownValue(and.LeftExpression, state), TryGetKnownValue(and.RightExpression, state)),
            BinaryOperationSyntax { Operator: BinaryOperator.LogicalOr } or
                => Or(TryGetKnownValue(or.LeftExpression, state), TryGetKnownValue(or.RightExpression, state)),
            _ when TryGetComparison(expression) is { Literal: NullFlowLiteral } nullComparison &&
                state.NonNullReferences.Contains(nullComparison.Reference) => nullComparison.Negated,
            _ => null,
        };

        return negated ? Invert(result) : result;
    }

    private FlowPredicate? TryGetPredicate(SyntaxBase syntax)
    {
        syntax = UnwrapParentheses(syntax);
        switch (syntax)
        {
            case UnaryOperationSyntax { Operator: UnaryOperator.Not } unary:
                return TryGetPredicate(unary.Expression) is { } operand ? new NegatedPredicate(operand) : null;
            case VariableAccessSyntax when TryGetTrackedSymbol(syntax) is { } symbol:
                return new BooleanSymbolPredicate(symbol);
            case BooleanLiteralSyntax literal:
                return new BooleanLiteralPredicate(literal.Value);
            case BinaryOperationSyntax { Operator: BinaryOperator.LogicalAnd or BinaryOperator.LogicalOr } logical:
                var left = TryGetPredicate(logical.LeftExpression);
                var right = TryGetPredicate(logical.RightExpression);
                return left is not null && right is not null
                    ? new LogicalPredicate(logical.Operator, left, right)
                    : null;
            case BinaryOperationSyntax when TryGetComparison(syntax) is { Reference: SymbolFlowReference { Symbol: var symbol } } comparison:
                // `x == true` and `x == false` are canonicalized to `x` and `!x` so that all spellings share one fact.
                var (predicate, negated) = comparison.Literal is BooleanFlowLiteral { Value: var literalValue }
                    ? ((FlowPredicate)new BooleanSymbolPredicate(symbol), comparison.Negated == literalValue)
                    : (new LiteralComparisonPredicate(symbol, comparison.Operator, comparison.Literal), comparison.Negated);
                return negated ? new NegatedPredicate(predicate) : predicate;
            default:
                return null;
        }
    }

    private static (FlowPredicate Operand, bool Negated) UnwrapPredicate(FlowPredicate predicate)
    {
        var negated = false;
        while (predicate is NegatedPredicate negatedPredicate)
        {
            predicate = negatedPredicate.Operand;
            negated = !negated;
        }

        return (predicate, negated);
    }

    private FlowComparison? TryGetComparison(SyntaxBase syntax)
    {
        if (UnwrapParentheses(syntax) is not BinaryOperationSyntax comparison)
        {
            return null;
        }

        var (operation, negated) = comparison.Operator switch
        {
            BinaryOperator.Equals => (BinaryOperator.Equals, false),
            BinaryOperator.NotEquals => (BinaryOperator.Equals, true),
            BinaryOperator.EqualsInsensitive => (BinaryOperator.EqualsInsensitive, false),
            BinaryOperator.NotEqualsInsensitive => (BinaryOperator.EqualsInsensitive, true),
            _ => ((BinaryOperator?)null, false),
        };
        if (operation is not { } normalizedOperation)
        {
            return null;
        }

        var left = UnwrapParentheses(comparison.LeftExpression);
        var right = UnwrapParentheses(comparison.RightExpression);
        if (TryGetReference(left) is { } leftReference && TryGetLiteral(right) is { } rightLiteral)
        {
            return new(left, leftReference, normalizedOperation, rightLiteral, negated);
        }

        if (TryGetReference(right) is { } rightReference && TryGetLiteral(left) is { } leftLiteral)
        {
            return new(right, rightReference, normalizedOperation, leftLiteral, negated);
        }

        return null;
    }

    private FlowReference? TryGetReference(SyntaxBase syntax)
    {
        syntax = UnwrapParentheses(syntax);
        if (TryGetTrackedSymbol(syntax) is { } symbol)
        {
            return new SymbolFlowReference(symbol);
        }

        if (syntax is not (PropertyAccessSyntax { PropertyName.IsValid: true } or ArrayAccessSyntax { FromEndMarker: null }))
        {
            return null;
        }

        var access = (AccessExpressionSyntax)syntax;
        var propertyName = access is ArrayAccessSyntax arrayAccess
            ? (UnwrapParentheses(arrayAccess.IndexExpression) as StringSyntax)?.TryGetLiteralValue()
            : access.TryGetPropertyName();

        return propertyName is not null && TryGetReference(access.BaseExpression) is { } parent
            ? new PropertyFlowReference(parent, propertyName)
            : null;
    }

    private static bool ContainsSafeAccess(SyntaxBase syntax)
    {
        while (UnwrapParentheses(syntax) is AccessExpressionSyntax access)
        {
            if (access.IsSafeAccess)
            {
                return true;
            }

            syntax = access.BaseExpression;
        }

        return false;
    }

    private static FlowLiteral? TryGetLiteral(SyntaxBase syntax) => syntax switch
    {
        StringSyntax value when value.TryGetLiteralValue() is { } literal => new StringFlowLiteral(literal),
        IntegerLiteralSyntax value => new IntegerFlowLiteral(value.Value),
        UnaryOperationSyntax { Operator: UnaryOperator.Minus, Expression: IntegerLiteralSyntax value }
            => new IntegerFlowLiteral(-new BigInteger(value.Value)),
        BooleanLiteralSyntax value => new BooleanFlowLiteral(value.Value),
        NullLiteralSyntax => NullFlowLiteral.Instance,
        _ => null,
    };

    private static bool? And(bool? left, bool? right)
        => (left, right) switch {
            (false, _) => false,
            (_, false) => false,
            (true, true) => true,
            _ => null
        };

    private static bool? Or(bool? left, bool? right)
        => (left, right) switch {
            (true, _) => true,
            (_, true) => true,
            (false, false) => false,
            _ => null
        };

    private static bool? Invert(bool? value) => value is { } known ? !known : null;

    private Symbol? TryGetTrackedSymbol(SyntaxBase syntax)
    {
        if (UnwrapParentheses(syntax) is not VariableAccessSyntax variable)
        {
            return null;
        }

        return binder.GetSymbolInfo(variable) is ParameterSymbol or VariableSymbol or LocalVariableSymbol
            ? binder.GetSymbolInfo(variable)
            : null;
    }

    private static (SyntaxBase expression, bool negated) UnwrapCondition(SyntaxBase syntax)
    {
        var negated = false;
        while (true)
        {
            syntax = UnwrapParentheses(syntax);
            if (syntax is not UnaryOperationSyntax { Operator: UnaryOperator.Not } unary)
            {
                return (syntax, negated);
            }

            negated = !negated;
            syntax = unary.Expression;
        }
    }

    private static SyntaxBase UnwrapParentheses(SyntaxBase syntax)
    {
        while (syntax is ParenthesizedExpressionSyntax parenthesized)
        {
            syntax = parenthesized.Expression;
        }

        return syntax;
    }
}
