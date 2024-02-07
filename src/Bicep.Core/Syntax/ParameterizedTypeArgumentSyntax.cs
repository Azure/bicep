// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class ParameterizedTypeArgumentSyntax(SyntaxBase expression) : ExpressionSyntax
{
    public SyntaxBase Expression { get; } = expression;

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitParameterizedTypeArgumentSyntax(this);

    public override TextSpan Span => this.Expression.Span;
}
