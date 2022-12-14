// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Intermediate;

public class ExpressionLoweringVisitor : ExpressionRewriteVisitor
{
    private ExpressionLoweringVisitor() { }

    public static Expression Lower(Expression expression)
    {
        var visitor = new ExpressionLoweringVisitor();
        
        return visitor.Replace(expression);
    }

    public override Expression ReplaceArrayAccessExpression(ArrayAccessExpression expression)
    {
        if (expression.Access is StringLiteralExpression stringLiteral)
        {
            return base.Replace(new PropertyAccessExpression(expression.Base, stringLiteral.Value));
        }

        return base.ReplaceArrayAccessExpression(expression);
    }
}