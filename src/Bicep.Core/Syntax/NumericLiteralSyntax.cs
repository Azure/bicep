using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class NumericLiteralSyntax : SyntaxBase, IExpressionSyntax
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

        public ExpressionKind ExpressionKind => ExpressionKind.SimpleLiteral;
    }
}