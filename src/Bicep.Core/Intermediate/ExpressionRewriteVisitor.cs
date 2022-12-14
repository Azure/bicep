// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Intermediate;

public abstract partial class ExpressionRewriteVisitor : IExpressionVisitor
{
    protected Expression Replace(Expression expression)
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();

        current = null;
        expression.Accept(this);

        if (current is null)
        {
            throw new InvalidOperationException($"Expected {nameof(current)} to not be null");
        }

        return current;
    }

    private Expression? current;

    private bool TryRewrite(Expression expression, out Expression rewritten)
    {
        rewritten = Replace(expression);

        return !object.ReferenceEquals(expression, rewritten);
    }

    private bool TryRewrite(ImmutableArray<Expression> expressions, out ImmutableArray<Expression> newExpressions)
    {
        var hasChanges = false;
        var newExpressionList = new List<Expression>();
        foreach (var expression in expressions)
        {
            hasChanges |= TryRewrite(expression, out var newExpression);
            newExpressionList.Add(newExpression);
        }

        newExpressions = hasChanges ? newExpressionList.ToImmutableArray() : expressions;
        return hasChanges;
    }

    private void ReplaceCurrent<TExpression>(TExpression expression, Func<TExpression, Expression> replaceFunc)
        where TExpression : Expression
    {
        if (current is not null)
        {
            throw new InvalidOperationException($"Expected {nameof(current)} to be null");
        }

        current = replaceFunc(expression);
    }
}