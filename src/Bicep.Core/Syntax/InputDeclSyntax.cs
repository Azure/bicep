using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class InputDeclSyntax : SyntaxBase
    {
        public InputDeclSyntax(Token inputKeyword, IdentifierSyntax type, IdentifierSyntax identifier, Token semicolon)
        {
            InputKeyword = inputKeyword;
            Type = type;
            Identifier = identifier;
            Semicolon = semicolon;
        }

        public Token InputKeyword { get; }

        public IdentifierSyntax Type { get; }

        public IdentifierSyntax Identifier { get; }

        public Token Semicolon { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitInputDeclSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(InputKeyword, Semicolon);
    }
}