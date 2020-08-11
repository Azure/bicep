using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class OutputDeclarationSyntax : SyntaxBase
    {
        public OutputDeclarationSyntax(Token outputKeyword, IdentifierSyntax name, TypeSyntax type, Token assignment, SyntaxBase value, Token newLine)
        {
            AssertKeyword(outputKeyword, nameof(outputKeyword), LanguageConstants.OutputKeyword);
            AssertTokenType(assignment, nameof(assignment), TokenType.Assignment);
            AssertTokenType(newLine, nameof(newLine), TokenType.NewLine);
            
            this.OutputKeyword = outputKeyword;
            this.Name = name;
            this.Type = type;
            this.Assignment = assignment;
            this.Value = value;
            this.NewLine = newLine;
        }

        public Token OutputKeyword { get; }

        public IdentifierSyntax Name { get; }

        public TypeSyntax Type { get; }

        public Token Assignment { get; }

        public SyntaxBase Value { get; }

        public Token NewLine { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitOutputDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(OutputKeyword, NewLine);
    }
}