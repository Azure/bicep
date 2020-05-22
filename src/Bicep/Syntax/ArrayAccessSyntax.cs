using Bicep.Parser;

namespace Bicep.Syntax
{
    public class ArrayAccessSyntax : SyntaxBase
    {
        public ArrayAccessSyntax(SyntaxBase parent, Token openSquare, SyntaxBase property, Token closeSquare)
        {
            Parent = parent;
            OpenSquare = openSquare;
            Property = property;
            CloseSquare = closeSquare;
        }

        public SyntaxBase Parent { get; }

        public Token OpenSquare { get; }

        public SyntaxBase Property { get; }

        public Token CloseSquare { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitArrayAccessSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Parent, CloseSquare);
    }
}