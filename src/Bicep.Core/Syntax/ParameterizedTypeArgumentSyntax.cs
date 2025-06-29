// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class ParameterizedTypeArgumentSyntax : TypeSyntax
{
    public ParameterizedTypeArgumentSyntax(SyntaxBase expression)
    {
        this.Expression = expression;
    }

    public SyntaxBase Expression { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitParameterizedTypeArgumentSyntax(this);

    public override TextSpan Span => this.Expression.Span;
}
