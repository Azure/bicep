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

public abstract record Expression(
    SyntaxBase? SourceSyntax)
{
    public abstract void Accept(IExpressionVisitor visitor);
}

public record BooleanLiteralExpression(
    SyntaxBase? SourceSyntax,
    bool Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitBooleanLiteralExpression(this);
}

public record IntegerLiteralExpression(
    SyntaxBase? SourceSyntax,
    long Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitIntegerLiteralExpression(this);
}

public record StringLiteralExpression(
    SyntaxBase? SourceSyntax,
    string Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitStringLiteralExpression(this);
}

public record NullLiteralExpression(
    SyntaxBase? SourceSyntax
): Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitNullLiteralExpression(this);
}

public record SyntaxExpression(
    SyntaxBase Syntax
) : Expression(Syntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitSyntaxExpression(this);
}

public record InterpolatedStringExpression(
    SyntaxBase? SourceSyntax,
    ImmutableArray<string> SegmentValues,
    ImmutableArray<Expression> Expressions
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitInterpolatedStringExpression(this);
}

public record ObjectPropertyExpression(
    SyntaxBase? SourceSyntax,
    Expression Key,
    Expression Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitObjectPropertyExpression(this);
}

public record ObjectExpression(
    SyntaxBase? SourceSyntax,
    ImmutableArray<ObjectPropertyExpression> Properties
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitObjectExpression(this);
}

public record ArrayExpression(
    SyntaxBase? SourceSyntax,
    ImmutableArray<Expression> Items
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitArrayExpression(this);
}

public record TernaryExpression(
    SyntaxBase? SourceSyntax,
    Expression Condition,
    Expression True,
    Expression False
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitTernaryExpression(this);
}

public record BinaryExpression(
    SyntaxBase? SourceSyntax,
    BinaryOperator Operator,
    Expression Left,
    Expression Right
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitBinaryExpression(this);
}

public record UnaryExpression(
    SyntaxBase? SourceSyntax,
    UnaryOperator Operator,
    Expression Expression
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitUnaryExpression(this);
}

public record FunctionCallExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    ImmutableArray<Expression> Parameters
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitFunctionCallExpression(this);
}

public record ArrayAccessExpression(
    SyntaxBase? SourceSyntax,
    Expression Base,
    Expression Access
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitArrayAccessExpression(this);
}

public record PropertyAccessExpression(
    SyntaxBase? SourceSyntax,
    Expression Base,
    string PropertyName
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitPropertyAccessExpression(this);
}

public record ResourceReferenceExpression(
    SyntaxBase? SourceSyntax,
    ResourceMetadata Metadata,
    IndexReplacementContext? IndexContext
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitResourceReferenceExpression(this);
}

public record ModuleReferenceExpression(
    SyntaxBase? SourceSyntax,
    ModuleSymbol Module,
    IndexReplacementContext? IndexContext
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitModuleReferenceExpression(this);
}

public record ModuleOutputPropertyAccessExpression(
    SyntaxBase? SourceSyntax,
    Expression Base,
    string PropertyName)
: Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitModuleOutputPropertyAccessExpression(this);
}

public record VariableReferenceExpression(
    SyntaxBase? SourceSyntax,
    VariableSymbol Variable
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitVariableReferenceExpression(this);
}

public record ParametersReferenceExpression(
    SyntaxBase? SourceSyntax,
    ParameterSymbol Parameter
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitParametersReferenceExpression(this);
}

public record LambdaVariableReferenceExpression(
    SyntaxBase? SourceSyntax,
    LocalVariableSymbol Variable
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitLambdaVariableReferenceExpression(this);
}

public record ForLoopExpression(
    SyntaxBase? SourceSyntax,
    Expression Expression,
    Expression Body
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitForLoopExpression(this);
}

public record CopyIndexExpression(
    SyntaxBase? SourceSyntax,
    string? Name
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitCopyIndexExpression(this);
}

public record LambdaExpression(
    SyntaxBase? SourceSyntax,
    ImmutableArray<string> Parameters,
    Expression Body
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitLambdaExpression(this);
}
