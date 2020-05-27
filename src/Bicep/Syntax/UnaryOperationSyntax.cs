using Bicep.Parser;

namespace Bicep.Syntax
{
    public class UnaryOperationSyntax : SyntaxBase
    {
        public UnaryOperationSyntax(Token operatorToken, SyntaxBase expression)
        {
            Operator = operatorToken;
            Expression = expression;
        }

        public Token Operator { get; }

        public SyntaxBase Expression { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitUnaryOperationSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Operator, Expression);
    }
}