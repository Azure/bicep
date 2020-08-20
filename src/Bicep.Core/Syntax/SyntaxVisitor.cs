using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public abstract class SyntaxVisitor
    {
        public void Visit(SyntaxBase? node)
        {
            if (node == null)
            {
                return;
            }

            VisitInternal(node);
        }

        protected virtual void VisitInternal(SyntaxBase node)
        {
            node.Accept(this);
        }

        public void VisitToken(Token? token)
        {
            if (token == null)
            {
                return;
            }

            VisitTokenInternal(token);
        }

        protected virtual void VisitTokenInternal(Token token)
        {
            foreach (var syntaxTrivia in token.LeadingTrivia)
            {
                this.VisitSyntaxTrivia(syntaxTrivia);
            }

            foreach (var syntaxTrivia in token.TrailingTrivia)
            {
                this.VisitSyntaxTrivia(syntaxTrivia);
            }
        }

        public virtual void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia)
        {

        }

        public virtual void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            this.VisitToken(syntax.ParameterKeyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Modifier);
            this.VisitToken(syntax.NewLine);
        }

        public virtual void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
        {
            this.VisitToken(syntax.AssignmentToken);
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
            for (var i = 0; i < syntax.Expressions.Length; i++)
            {
                this.VisitToken(syntax.StringTokens[i]);
                this.Visit(syntax.Expressions[i]);
            }
            this.VisitToken(syntax.StringTokens.Last());
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
            this.VisitToken(syntax.NullKeyword);
        }

        public virtual void VisitSkippedTokensTriviaSyntax(SkippedTokensTriviaSyntax syntax)
        {
            this.VisitTokens(syntax.Tokens);
        }

        public virtual void VisitObjectSyntax(ObjectSyntax syntax)
        {
            this.VisitToken(syntax.OpenBrace);
            this.VisitTokens(syntax.NewLines);
            this.VisitNodes(syntax.Children);
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
            this.VisitNodes(syntax.Children);
            this.VisitToken(syntax.CloseBracket);
        }

        public virtual void VisitArrayItemSyntax(ArrayItemSyntax syntax)
        {
            this.Visit(syntax.Value);
            this.VisitTokens(syntax.NewLines);
        }

        public virtual void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
        {
            this.Visit(syntax.ConditionExpression);
            this.VisitToken(syntax.Question);
            this.Visit(syntax.TrueExpression);
            this.VisitToken(syntax.Colon);
            this.Visit(syntax.FalseExpression);
        }

        public virtual void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            this.Visit(syntax.LeftExpression);
            this.VisitToken(syntax.OperatorToken);
            this.Visit(syntax.RightExpression);
        }

        public virtual void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            this.VisitToken(syntax.OperatorToken);
            this.Visit(syntax.Expression);
        }

        public virtual void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.VisitToken(syntax.OpenSquare);
            this.Visit(syntax.IndexExpression);
            this.VisitToken(syntax.CloseSquare);
        }

        public virtual void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.VisitToken(syntax.Dot);
            this.Visit(syntax.PropertyName);
        }

        public virtual void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
        {
            this.VisitToken(syntax.OpenParen);
            this.Visit(syntax.Expression);
            this.VisitToken(syntax.CloseParen);
        }

        public virtual void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            this.Visit(syntax.Name);
            this.VisitToken(syntax.OpenParen);
            this.VisitNodes(syntax.Arguments);
            this.VisitToken(syntax.CloseParen);
        }

        public virtual void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax)
        {
            this.Visit(syntax.Expression);
            this.VisitToken(syntax.Comma);
        }

        public virtual void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            this.Visit(syntax.Name);
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