using Bicep.Parser;

namespace Bicep.Syntax
{
    public class NumericLiteralSyntax : SyntaxBase
    {
        public NumericLiteralSyntax(Token literal, int value)
        {
            Literal = literal;
            Value = value;
        }

        public Token Literal { get; }

        public int Value { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitNumericLiteralSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Literal, Literal);
    }
}