using Bicep.Parser;

namespace Bicep.Syntax
{
    public class IdentifierSyntax : SyntaxBase
    {
        public IdentifierSyntax(Token identifier)
        {
            Identifier = identifier;
        }

        public Token Identifier { get; }

        public string GetName() => Identifier.Text;

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitIdentifierSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Identifier, Identifier);
    }
}