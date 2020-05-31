using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ArraySyntax : SyntaxBase
    {
        public ArraySyntax(Token openSquare, SeparatedSyntaxList items, Token closeSquare)
        {
            OpenSquare = openSquare;
            Items = items;
            CloseSquare = closeSquare;
        }

        public Token OpenSquare { get; }

        public SeparatedSyntaxList Items { get; }

        public Token CloseSquare { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitArraySyntax(this);

        public override TextSpan Span
            => TextSpan.Between(OpenSquare, CloseSquare);
    }
}