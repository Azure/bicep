using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ParenthesizedExpressionSyntax : SyntaxBase, IExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(Token openParen, SyntaxBase expression, Token closeParen)
        {
            AssertTokenType(openParen, nameof(openParen), TokenType.LeftParen);
            AssertTokenType(closeParen, nameof(closeParen), TokenType.RightParen);

            this.OpenParen = openParen;
            this.Expression = expression;
            this.CloseParen = closeParen;
        }
        
        public Token OpenParen { get; }

        public SyntaxBase Expression { get; }

        public Token CloseParen { get; }


        public override void Accept(SyntaxVisitor visitor) => visitor.VisitParenthesizedExpressionSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.OpenParen, this.CloseParen);
    }
}
