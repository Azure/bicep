using System.Collections.Generic;
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
            this.VisitToken(syntax.ParameterKeyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);

            if (syntax.Modifier != null)
            {
                this.Visit(syntax.Modifier);
            }

            this.VisitToken(syntax.NewLine);
        }

        public virtual void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
        {
            this.VisitToken(syntax.DefaultKeyword);
            this.Visit(syntax.DefaultValue);
        }

        public virtual void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            this.VisitToken(syntax.VariableKeyword);
            this.Visit(syntax.Name);
            this.VisitToken(syntax.Assignment);
            this.Visit(syntax.Value);
            this.VisitToken(syntax.NewLine);
        }

        public virtual void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            this.VisitToken(syntax.ResourceKeyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.VisitToken(syntax.Assignment);
            this.Visit(syntax.Body);
            this.VisitToken(syntax.NewLine);
        }

        public virtual void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            this.VisitToken(syntax.OutputKeyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.VisitToken(syntax.Assignment);
            this.Visit(syntax.Value);
            this.VisitToken(syntax.NewLine);
        }

        public virtual void VisitNoOpDeclarationSyntax(NoOpDeclarationSyntax syntax)
        {
            this.VisitToken(syntax.NewLine);
        }

        public virtual void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            this.VisitToken(syntax.Identifier);
        }

        public virtual void VisitTypeSyntax(TypeSyntax syntax)
        {
            this.VisitToken(syntax.Identifier);
        }

        public virtual void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
            this.VisitToken(syntax.Literal);
        }

        public virtual void VisitStringSyntax(StringSyntax syntax)
        {
            this.VisitToken(syntax.StringToken);
        }

        public virtual void VisitProgramSyntax(ProgramSyntax syntax)
        {
            this.VisitNodes(syntax.Statements);
            this.VisitToken(syntax.EndOfFile);
        }

        public virtual void VisitNumericLiteralSyntax(NumericLiteralSyntax syntax)
        {
            this.VisitToken(syntax.Literal);
        }

        public virtual void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
        {
            this.VisitToken(syntax.Literal);
        }

        public virtual void VisitSkippedTokensTriviaSyntax(SkippedTokensTriviaSyntax syntax)
        {
            this.VisitTokens(syntax.Tokens);
        }

        public virtual void VisitObjectSyntax(ObjectSyntax syntax)
        {
            this.VisitToken(syntax.OpenBrace);
            this.VisitTokens(syntax.NewLines);
            this.VisitNodes(syntax.Properties);
            this.VisitToken(syntax.CloseBrace);
        }

        public virtual void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            this.Visit(syntax.Identifier);
            this.VisitToken(syntax.Colon);
            this.Visit(syntax.Value);
            this.VisitTokens(syntax.NewLines);
        }

        public virtual void VisitArraySyntax(ArraySyntax syntax)
        {
            this.VisitToken(syntax.OpenBracket);
            this.VisitTokens(syntax.NewLines);
            this.VisitNodes(syntax.Items);
            this.VisitToken(syntax.CloseBracket);
        }

        public virtual void VisitArrayItemSyntax(ArrayItemSyntax syntax)
        {
            this.Visit(syntax.Value);
            this.VisitTokens(syntax.NewLines);
        }

        protected void VisitTokens(IEnumerable<Token> tokens)
        {
            foreach (Token token in tokens)
            {
                this.VisitToken(token);
            }
        }

        protected void VisitNodes(IEnumerable<SyntaxBase> nodes)
        {
            foreach (SyntaxBase node in nodes)
            {
                this.Visit(node);
            }
        }
    }
}