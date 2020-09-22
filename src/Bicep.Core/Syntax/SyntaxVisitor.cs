// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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

        public virtual void VisitToken(Token token)
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
        public virtual void VisitSeparatedSyntaxList(SeparatedSyntaxList syntax)
        {
            // visit paired elements in order
            foreach (var (item, token) in syntax.GetPairedElements())
            {
                this.Visit(item);
                this.Visit(token);
            }
        }

        public virtual void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            this.Visit(syntax.ParameterKeyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Modifier);
        }

        public virtual void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
        {
            this.Visit(syntax.AssignmentToken);
            this.Visit(syntax.DefaultValue);
        }

        public virtual void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            this.Visit(syntax.VariableKeyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Assignment);
            this.Visit(syntax.Value);
        }

        public virtual void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            this.Visit(syntax.ResourceKeyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Assignment);
            this.Visit(syntax.Body);
        }

        public virtual void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            this.Visit(syntax.OutputKeyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Assignment);
            this.Visit(syntax.Value);
        }

        public virtual void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            this.Visit(syntax.Identifier);
        }

        public virtual void VisitTypeSyntax(TypeSyntax syntax)
        {
            this.Visit(syntax.Identifier);
        }

        public virtual void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
            this.Visit(syntax.Literal);
        }

        public virtual void VisitStringSyntax(StringSyntax syntax)
        {
            for (var i = 0; i < syntax.Expressions.Length; i++)
            {
                this.Visit(syntax.StringTokens[i]);
                this.Visit(syntax.Expressions[i]);
            }
            this.Visit(syntax.StringTokens.Last());
        }

        public virtual void VisitProgramSyntax(ProgramSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
            this.Visit(syntax.EndOfFile);
        }

        public virtual void VisitNumericLiteralSyntax(NumericLiteralSyntax syntax)
        {
            this.Visit(syntax.Literal);
        }

        public virtual void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
        {
            this.Visit(syntax.NullKeyword);
        }

        public virtual void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            foreach (var element in syntax.Elements)
            {
                this.Visit(element);
            }
        }

        public virtual void VisitObjectSyntax(ObjectSyntax syntax)
        {
            this.Visit(syntax.OpenBrace);
            this.VisitTokens(syntax.NewLines);
            this.VisitNodes(syntax.Children);
            this.Visit(syntax.CloseBrace);
        }

        public virtual void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            this.Visit(syntax.Key);
            this.Visit(syntax.Colon);
            this.Visit(syntax.Value);
            this.VisitTokens(syntax.NewLines);
        }

        public virtual void VisitArraySyntax(ArraySyntax syntax)
        {
            this.Visit(syntax.OpenBracket);
            this.VisitTokens(syntax.NewLines);
            this.VisitNodes(syntax.Children);
            this.Visit(syntax.CloseBracket);
        }

        public virtual void VisitArrayItemSyntax(ArrayItemSyntax syntax)
        {
            this.Visit(syntax.Value);
            this.VisitTokens(syntax.NewLines);
        }

        public virtual void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
        {
            this.Visit(syntax.ConditionExpression);
            this.Visit(syntax.Question);
            this.Visit(syntax.TrueExpression);
            this.Visit(syntax.Colon);
            this.Visit(syntax.FalseExpression);
        }

        public virtual void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            this.Visit(syntax.LeftExpression);
            this.Visit(syntax.OperatorToken);
            this.Visit(syntax.RightExpression);
        }

        public virtual void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            this.Visit(syntax.OperatorToken);
            this.Visit(syntax.Expression);
        }

        public virtual void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.OpenSquare);
            this.Visit(syntax.IndexExpression);
            this.Visit(syntax.CloseSquare);
        }

        public virtual void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.Dot);
            this.Visit(syntax.PropertyName);
        }

        public virtual void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
        {
            this.Visit(syntax.OpenParen);
            this.Visit(syntax.Expression);
            this.Visit(syntax.CloseParen);
        }

        public virtual void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            this.Visit(syntax.Name);
            this.Visit(syntax.OpenParen);
            this.VisitNodes(syntax.Arguments);
            this.Visit(syntax.CloseParen);
        }

        public virtual void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.Dot);
            this.Visit(syntax.Name);
            this.Visit(syntax.OpenParen);
            this.VisitNodes(syntax.Arguments);
            this.Visit(syntax.CloseParen);
        }

        public virtual void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax)
        {
            this.Visit(syntax.Expression);
            this.Visit(syntax.Comma);
        }

        public virtual void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            this.Visit(syntax.Name);
        }

        protected void VisitTokens(IEnumerable<Token> tokens)
        {
            foreach (Token token in tokens)
            {
                this.Visit(token);
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
