using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class IdentifierSyntax : SyntaxBase
    {
        public IdentifierSyntax(Token identifier)
        {
            AssertTokenType(identifier, nameof(identifier), TokenType.Identifier);
            Assert(string.IsNullOrEmpty(identifier.Text) == false, "Identifier must not be null or empty.");

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