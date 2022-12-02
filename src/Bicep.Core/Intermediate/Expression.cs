// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Intermediate;

public record IndexReplacementContext(
    ImmutableDictionary<LocalVariableSymbol, Expression> LocalReplacements,
    Expression Index);

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

public record TernaryExpression(
    Expression Condition,
    Expression True,
    Expression False
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitTernaryExpression(this);
}

public record BinaryExpression(
    BinaryOperator Operator,
    Expression Left,
    Expression Right
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitBinaryExpression(this);
}

public record UnaryExpression(
    UnaryOperator Operator,
    Expression Expression
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitUnaryExpression(this);
}

public record FunctionCallExpression(
    string Name,
    ImmutableArray<Expression> Parameters
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitFunctionCallExpression(this);
}

public record ArrayAccessExpression(
    Expression Base,
    Expression Access
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitArrayAccessExpression(this);
}

public record PropertyAccessExpression(
    Expression Base,
    string PropertyName
) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitPropertyAccessExpression(this);
}

public enum StaticResourcePropertyKind {
    Id,
    Name,
    Type,
    ApiVersion,
}

public record ResourceIdExpression(
    StaticResourcePropertyKind Kind,
    ResourceMetadata Metadata,
    IndexReplacementContext? IndexContext) : Expression
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitResourceIdExpression(this);
}
