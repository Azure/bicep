// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.Intermediate;

public static class ExpressionBuilder
{
    public static Expression Build(SyntaxBase syntax)
    {
        return syntax switch {
            StringSyntax x when !x.IsInterpolated() => new StringLiteralExpression(x.SegmentValues[0]),
            StringSyntax x => new InterpolatedStringExpression(x.SegmentValues, x.Expressions.Select(Build).ToImmutableArray()),
            IntegerLiteralSyntax x when TryGetIntegerLiteralValue(x) is {} val => new IntegerLiteralExpression(val),
            UnaryOperationSyntax x when TryGetIntegerLiteralValue(x) is {} val => new IntegerLiteralExpression(val),
            BooleanLiteralSyntax x => new BooleanLiteralExpression(x.Value),
            NullLiteralSyntax => new NullLiteralExpression(),
            ParenthesizedExpressionSyntax x => Build(x.Expression),
            ObjectSyntax x => new ObjectExpression(x.Properties.Select(x => new ObjectProperty(Build(x.Key), Build(x.Value))).ToImmutableArray()),
            ArraySyntax x => new ArrayExpression(x.Items.Select(x => Build(x.Value)).ToImmutableArray()),
            _ => new SyntaxExpression(syntax),
        };
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
}
