// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Visits an <see href="https://en.wikipedia.org/wiki/Abstract_syntax_tree">abstract syntax tree (AST)</see>.
    /// </summary>
    public abstract class AstVisitor : ISyntaxVisitor
    {
        public void Visit(SyntaxBase? node)
        {
            if (node == null)
            {
                return;
            }

            RuntimeHelpers.EnsureSufficientExecutionStack();

            VisitInternal(node);
        }

        protected virtual void VisitInternal(SyntaxBase node)
        {
            node.Accept(this);
        }

        public virtual void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia)
        {
        }

        public virtual void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
        }

        public virtual void VisitToken(Token token)
        {
        }

        public virtual void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
        }

        public virtual void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax)
        {
        }

        public virtual void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
        {
        }

        public virtual void VisitSeparatedSyntaxList(SeparatedSyntaxList syntax)
        {
            foreach (var element in syntax.Elements)
            {
                this.Visit(element);
            }
        }

        public virtual void VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Value);
        }

        public virtual void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Modifier);
        }

        public virtual void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
        {
            this.Visit(syntax.DefaultValue);
        }

        public virtual void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Value);
        }

        public virtual void VisitLocalVariableSyntax(LocalVariableSyntax syntax)
        {
            this.Visit(syntax.Name);
        }

        public virtual void VisitTargetScopeSyntax(TargetScopeSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Value);
        }

        public virtual void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Value);
        }

        public virtual void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Path);
            this.Visit(syntax.Value);
        }

        public virtual void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Value);
        }

        public virtual void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            this.Visit(syntax.Child);
        }

        public virtual void VisitResourceTypeSyntax(ResourceTypeSyntax syntax)
        {
            this.Visit(syntax.Type);
        }

        public virtual void VisitObjectTypeSyntax(ObjectTypeSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public virtual void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Key);
            this.Visit(syntax.Value);
        }

        public virtual void VisitArrayTypeSyntax(ArrayTypeSyntax syntax)
        {
            this.Visit(syntax.Item);
        }

        public virtual void VisitArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax)
        {
            this.Visit(syntax.Value);
        }

        public virtual void VisitUnionTypeSyntax(UnionTypeSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public virtual void VisitUnionTypeMemberSyntax(UnionTypeMemberSyntax syntax)
        {
            this.Visit(syntax.Value);
        }

        public virtual void VisitTypeDeclarationSyntax(TypeDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Keyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Value);
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
        }

        public virtual void VisitObjectSyntax(ObjectSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public virtual void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            this.Visit(syntax.Key);
            this.Visit(syntax.Value);
        }

        public virtual void VisitArraySyntax(ArraySyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public virtual void VisitArrayItemSyntax(ArrayItemSyntax syntax)
        {
            this.Visit(syntax.Value);
        }

        public virtual void VisitIfConditionSyntax(IfConditionSyntax syntax)
        {
            this.Visit(syntax.ConditionExpression);
            this.Visit(syntax.Body);
        }

        public virtual void VisitForSyntax(ForSyntax syntax)
        {
            this.Visit(syntax.VariableSection);
            this.Visit(syntax.Expression);
            this.Visit(syntax.Body);
        }

        public virtual void VisitVariableBlockSyntax(VariableBlockSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public virtual void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
        {
            this.Visit(syntax.ConditionExpression);
            this.Visit(syntax.TrueExpression);
            this.Visit(syntax.FalseExpression);
        }

        public virtual void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            this.Visit(syntax.LeftExpression);
            this.Visit(syntax.RightExpression);
        }

        public virtual void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            this.Visit(syntax.Expression);
        }

        public virtual void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.IndexExpression);
        }

        public virtual void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.PropertyName);
        }

        public virtual void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.ResourceName);
        }

        public virtual void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
        {
            this.Visit(syntax.Expression);
        }

        public virtual void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            this.Visit(syntax.Name);
            this.VisitNodes(syntax.Children);
        }

        public virtual void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.Name);
            this.VisitNodes(syntax.Children);
        }

        public virtual void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax)
        {
            this.Visit(syntax.Expression);
        }

        public virtual void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            this.Visit(syntax.Name);
        }

        public virtual void VisitDecoratorSyntax(DecoratorSyntax syntax)
        {
            this.Visit(syntax.Expression);
        }

        public virtual void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
        }

        public virtual void VisitImportDeclarationSyntax(ImportDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.SpecificationString);
            this.Visit(syntax.WithClause);
            this.Visit(syntax.AsClause);
        }

        public virtual void VisitImportWithClauseSyntax(ImportWithClauseSyntax syntax)
        {
            this.Visit(syntax.Config);
        }

        public virtual void VisitImportAsClauseSyntax(ImportAsClauseSyntax syntax)
        {
            this.Visit(syntax.Alias);
        }

        public virtual void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Value);
        }

        public virtual void VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Path);
        }

        public virtual void VisitLambdaSyntax(LambdaSyntax syntax)
        {
            this.Visit(syntax.VariableSection);
            this.Visit(syntax.Body);
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
