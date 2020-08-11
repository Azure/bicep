using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class VariableDeclarationSyntax : SyntaxBase
    {
        public VariableDeclarationSyntax(Token variableKeyword, IdentifierSyntax name, Token assignment, SyntaxBase value, Token newLine)
        {
            AssertKeyword(variableKeyword, nameof(variableKeyword), LanguageConstants.VariableKeyword);
            AssertTokenType(assignment, nameof(assignment), TokenType.Assignment);
            AssertTokenType(newLine, nameof(newLine), TokenType.NewLine);

            this.VariableKeyword = variableKeyword;
            this.Name = name;
            this.Assignment = assignment;
            this.Value = value;
            this.NewLine = newLine;
        }

        public Token VariableKeyword { get; }

        public IdentifierSyntax Name { get; }

        public Token Assignment { get; }

        public SyntaxBase Value { get; }

        public Token NewLine { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitVariableDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(VariableKeyword, NewLine);
    }
}