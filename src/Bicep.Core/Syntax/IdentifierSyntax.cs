using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class IdentifierSyntax : SyntaxBase
    {
        public IdentifierSyntax(Token identifier)
        {
            Identifier = identifier;
        }

        public Token Identifier { get; }

        public string IdentifierName => Identifier.Text;

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitIdentifierSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Identifier, Identifier);
    }
}