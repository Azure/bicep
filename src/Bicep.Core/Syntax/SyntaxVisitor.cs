using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public abstract class SyntaxVisitor
    {
        public void Visit(SyntaxBase node)
        {
            node.Accept(this);
        }

        public virtual void VisitToken(Token token)
        {
        }

        public virtual void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            VisitToken(syntax.ParameterKeyword);
            Visit(syntax.Name);
            Visit(syntax.Type);

            if (syntax.Assignment != null)
            {
                VisitToken(syntax.Assignment);
            }

            if (syntax.Value != null)
            {
                Visit(syntax.Value);
            }

            VisitToken(syntax.NewLine);
        }

        public virtual void VisitNoOpDeclarationSyntax(NoOpDeclarationSyntax syntax)
        {
            VisitToken(syntax.NewLine);
        }

        public virtual void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            VisitToken(syntax.Identifier);
        }

        public virtual void VisitTypeSyntax(TypeSyntax syntax)
        {
            this.VisitToken(syntax.Identifier);
        }

        public virtual void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
            VisitToken(syntax.Literal);
        }

        public virtual void VisitStringSyntax(StringSyntax syntax)
        {
            VisitToken(syntax.StringToken);
        }

        public virtual void VisitProgramSyntax(ProgramSyntax syntax)
        {
            foreach (var statement in syntax.Statements)
            {
                Visit(statement);
            }

            VisitToken(syntax.EndOfFile);
        }

        public virtual void VisitNumericLiteralSyntax(NumericLiteralSyntax syntax)
        {
            VisitToken(syntax.Literal);
        }

        public virtual void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
        {
            VisitToken(syntax.Literal);
        }

        public virtual void VisitSkippedTokensTriviaSyntax(SkippedTokensTriviaSyntax syntax)
        {
            foreach (var token in syntax.Tokens)
            {
                VisitToken(token);
            }
        }
    }
}