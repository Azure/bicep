// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.Intermediate;

public abstract record Expression()
{
    public abstract void Accept(IExpressionVisitor visitor);
}

public record BooleanLiteralExpression(
    bool Value
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitBooleanLiteralExpression(this);
}

public record IntegerLiteralExpression(
    long Value
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitIntegerLiteralExpression(this);
}

public record StringLiteralExpression(
    string Value
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitStringLiteralExpression(this);
}

public record NullLiteralExpression() : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitNullLiteralExpression(this);
}

public record SyntaxExpression(
    SyntaxBase Syntax
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitSyntaxExpression(this);
}

public record InterpolatedStringExpression(
    ImmutableArray<string> SegmentValues,
    ImmutableArray<Expression> Expressions
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitInterpolatedStringExpression(this);
}

public record ObjectProperty(
    Expression Key,
    Expression Value);

public record ObjectExpression(
    ImmutableArray<ObjectProperty> Properties
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitObjectExpression(this);
}

public record ArrayExpression(
    ImmutableArray<Expression> Items
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitArrayExpression(this);
}
