using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class NoOpDeclarationSyntax : StatementSyntax
    {
        public NoOpDeclarationSyntax(Token newLine)
        {
            this.NewLine = newLine;
        }

        public Token NewLine { get; set; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitNoOpDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.NewLine, this.NewLine);
    }
}
