// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Intermediate;

public abstract partial class ExpressionVisitor : IExpressionVisitor
{
    public void Visit(Expression expression)
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();

        expression.Accept(this);
    }

    protected void Visit(IEnumerable<Expression> expressions)
    {
        foreach (var expression in expressions)
        {
            this.Visit(expression);
        }
    }
}
