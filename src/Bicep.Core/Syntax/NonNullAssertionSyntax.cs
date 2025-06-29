// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class NonNullAssertionSyntax : ExpressionSyntax
{
    public NonNullAssertionSyntax(SyntaxBase baseExpression, Token assertionOperator)
    {
        AssertTokenType(assertionOperator, nameof(assertionOperator), TokenType.Exclamation);

        this.BaseExpression = baseExpression;
        this.AssertionOperator = assertionOperator;
    }

    public SyntaxBase BaseExpression { get; }

    public Token AssertionOperator { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitNonNullAssertionSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.BaseExpression, this.AssertionOperator);
}
