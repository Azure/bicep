using System.Collections.Generic;
using System.Linq;
using Bicep.Parser;

namespace Bicep.Syntax
{
    public class ObjectSyntax : SyntaxBase
    {
        public ObjectSyntax(Token openBrace, SeparatedSyntaxList properties, Token closeBrace)
        {
            OpenBrace = openBrace;
            Properties = properties;
            CloseBrace = closeBrace;
        }

        public Token OpenBrace { get; }

        public SeparatedSyntaxList Properties { get; }

        public Token CloseBrace { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitObjectSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(OpenBrace, CloseBrace);
    }
}