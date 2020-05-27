using Bicep.Parser;

namespace Bicep.Syntax
{
    public class NullLiteralSyntax : SyntaxBase
    {
        public NullLiteralSyntax(Token literal)
        {
            Literal = literal;
        }

        public Token Literal { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitNullLiteralSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Literal, Literal);
    }
}