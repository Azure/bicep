// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
            return base.Replace(new PropertyAccessExpression(expression.SourceSyntax, expression.Base, stringLiteral.Value, expression.Flags));
        }

        return base.ReplaceArrayAccessExpression(expression);
    }

    public override Expression ReplaceObjectExpression(ObjectExpression expression)
    {
        if (!expression.Properties.Any(x => x.Condition is {}))
        {
            return base.ReplaceObjectExpression(expression);
        }

        var segments = new List<Expression>();
        var segmentStart = 0;
        for (var i = 0; i < expression.Properties.Length; i++)
        {
            if (expression.Properties[i].Condition is {} condition)
            {
                if (i > segmentStart)
                {
                    segments.Add(new ObjectExpression(expression.SourceSyntax, expression.Properties[segmentStart..i]));
                }

                var property = expression.Properties[i];
                var unconditionalProperty = new ObjectPropertyExpression(property.SourceSyntax, null, property.Key, property.Value);
                var ternary = new TernaryExpression(
                    property.SourceSyntax,
                    condition,
                    new ObjectExpression(property.SourceSyntax, ImmutableArray.Create(unconditionalProperty)),
                    new ObjectExpression(property.SourceSyntax, ImmutableArray<ObjectPropertyExpression>.Empty));

                segments.Add(ternary);
                segmentStart = i + 1;
            }
        }

        if (segmentStart < expression.Properties.Length)
        {
            segments.Add(new ObjectExpression(expression.SourceSyntax, expression.Properties[segmentStart..]));
        }

        return base.Replace(new FunctionCallExpression(
            expression.SourceSyntax,
            "union",
            segments.ToImmutableArray()));
    }
}
