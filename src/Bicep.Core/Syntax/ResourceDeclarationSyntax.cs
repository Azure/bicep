using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ResourceDeclarationSyntax : SyntaxBase
    {
        public ResourceDeclarationSyntax(Token resourceKeyword, IdentifierSyntax name, StringSyntax type, Token assignment, SyntaxBase body, Token newLine)
        {
            AssertTokenType(resourceKeyword, nameof(resourceKeyword), TokenType.ResourceKeyword);
            AssertTokenType(assignment, nameof(assignment), TokenType.Assignment);
            AssertTokenType(newLine, nameof(newLine), TokenType.NewLine);

            this.ResourceKeyword = resourceKeyword;
            this.Name = name;
            this.Type = type;
            this.Assignment = assignment;
            this.Body = body;
            this.NewLine = newLine;
        }

        public Token ResourceKeyword { get; }

        public IdentifierSyntax Name { get; }

        public StringSyntax Type { get; }

        public Token Assignment { get; }

        public SyntaxBase Body { get; }

        public Token NewLine { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitResourceDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(ResourceKeyword, NewLine);
    }
}