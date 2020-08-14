using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class NullLiteralSyntax : SyntaxBase, IExpressionSyntax, ILiteralSyntax
    {
        public NullLiteralSyntax(Token nullKeyword)
        {
            this.AssertTokenType(nullKeyword, nameof(nullKeyword), TokenType.NullKeyword);

            this.NullKeyword = nullKeyword;
        }

        public Token NullKeyword { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitNullLiteralSyntax(this);

        public override TextSpan Span => this.NullKeyword.Span;
    }
}