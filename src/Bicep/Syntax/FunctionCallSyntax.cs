using System.Collections.Generic;
using System.Linq;
using Bicep.Parser;

namespace Bicep.Syntax
{
    public class FunctionCallSyntax : SyntaxBase
    {
        public FunctionCallSyntax(SyntaxBase parent, Token openParen, SeparatedSyntaxList arguments, Token closeParen)
        {
            Parent = parent;
            OpenParen = openParen;
            Arguments = arguments;
            CloseParen = closeParen;
        }

        public SyntaxBase Parent { get; }

        public Token OpenParen { get; }

        public SeparatedSyntaxList Arguments { get; }

        public Token CloseParen { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitFunctionCallSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Parent, CloseParen);
    }
}