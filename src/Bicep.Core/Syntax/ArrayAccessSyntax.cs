using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ArrayAccessSyntax : SyntaxBase, IExpressionSyntax
    {
        public ArrayAccessSyntax(SyntaxBase baseExpression, Token openSquare, SyntaxBase indexExpression, Token closeSquare)
        {
            AssertTokenType(openSquare, nameof(openSquare), TokenType.LeftSquare);
            AssertTokenType(closeSquare, nameof(closeSquare), TokenType.RightSquare);

            this.BaseExpression = baseExpression;
            this.OpenSquare = openSquare;
            this.IndexExpression = indexExpression;
            this.CloseSquare = closeSquare;
        }

        public SyntaxBase BaseExpression { get; }

        public Token OpenSquare { get; }

        public SyntaxBase IndexExpression { get; }

        public Token CloseSquare { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitArrayAccessSyntax(this);

        public override TextSpan Span => TextSpan.Between(BaseExpression, CloseSquare);
        
        public ExpressionKind ExpressionKind => ExpressionKind.Operator;
    }
}