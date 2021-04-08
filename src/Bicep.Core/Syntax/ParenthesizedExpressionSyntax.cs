// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(Token openParen, SyntaxBase expression, SyntaxBase closeParen)
        {
            AssertTokenType(openParen, nameof(openParen), TokenType.LeftParen);
            AssertSyntaxType(closeParen, nameof(closeParen), typeof(Token), typeof(SkippedTriviaSyntax));

            if (closeParen is Token closeParenToken)
            {
                AssertTokenType(closeParenToken, nameof(closeParen), TokenType.RightParen);
            }

            this.OpenParen = openParen;
            this.Expression = expression;
            this.CloseParen = closeParen;
        }
        
        public Token OpenParen { get; }

        public SyntaxBase Expression { get; }

        public SyntaxBase CloseParen { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitParenthesizedExpressionSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.OpenParen, this.CloseParen);
    }
}

