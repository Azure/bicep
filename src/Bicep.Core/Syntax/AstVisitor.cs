// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Visits an <see href="https://en.wikipedia.org/wiki/Abstract_syntax_tree">abstract syntax tree (AST)</see>.
    /// </summary>
    /// <remarks>
    /// The Bicep syntax tree is always a <see href="https://en.wikipedia.org/wiki/Parse_tree">concret syntax tree</see>.
    /// The visitor visits syntax nodes except for terminal symbols (tokens) so that the Bicep syntax tree is traversed as an AST.
    /// </remarks>
    public abstract class AstVisitor : SyntaxVisitor
    {
        public override void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia)
        {
        }

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
        }

        public override void VisitToken(Token token)
        {
        }

        public override void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
        }

        public override void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax)
        {
        }

        public override void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
        {
        }

        public override void VisitSeparatedSyntaxList(SeparatedSyntaxList syntax)
        {
            foreach (var element in syntax.Elements)
            {
                this.Visit(element);
            }
        }

        public override void VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Value);
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Modifier);
        }

        public override void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
        {
            this.Visit(syntax.DefaultValue);
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Value);
        }

        public override void VisitLocalVariableSyntax(LocalVariableSyntax syntax)
        {
            this.Visit(syntax.Name);
        }

        public override void VisitTargetScopeSyntax(TargetScopeSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Value);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Value);
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Path);
            this.Visit(syntax.Value);
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Value);
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            this.Visit(syntax.Child);
        }

        public override void VisitResourceTypeSyntax(ResourceTypeSyntax syntax)
        {
            this.Visit(syntax.Type);
        }

        public override void VisitObjectTypeSyntax(ObjectTypeSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public override void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Key);
            this.Visit(syntax.Value);
        }

        public override void VisitObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Value);
        }

        public override void VisitTupleTypeSyntax(TupleTypeSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public override void VisitTupleTypeItemSyntax(TupleTypeItemSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Value);
        }

        public override void VisitArrayTypeSyntax(ArrayTypeSyntax syntax)
        {
            this.Visit(syntax.Item);
        }

        public override void VisitArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax)
        {
            this.Visit(syntax.Value);
        }

        public override void VisitUnionTypeSyntax(UnionTypeSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public override void VisitUnionTypeMemberSyntax(UnionTypeMemberSyntax syntax)
        {
            this.Visit(syntax.Value);
        }

        public override void VisitTypeDeclarationSyntax(TypeDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Value);
        }

        public override void VisitNullableTypeSyntax(NullableTypeSyntax syntax)
        {
            this.Visit(syntax.Base);
        }

        public override void VisitStringSyntax(StringSyntax syntax)
        {
            for (var i = 0; i < syntax.Expressions.Length; i++)
            {
                this.Visit(syntax.StringTokens[i]);
                this.Visit(syntax.Expressions[i]);
            }
            this.Visit(syntax.StringTokens.Last());
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            this.Visit(syntax.Key);
            this.Visit(syntax.Value);
        }

        public override void VisitArraySyntax(ArraySyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public override void VisitArrayItemSyntax(ArrayItemSyntax syntax)
        {
            this.Visit(syntax.Value);
        }

        public override void VisitIfConditionSyntax(IfConditionSyntax syntax)
        {
            this.Visit(syntax.ConditionExpression);
            this.Visit(syntax.Body);
        }

        public override void VisitForSyntax(ForSyntax syntax)
        {
            this.Visit(syntax.VariableSection);
            this.Visit(syntax.Expression);
            this.Visit(syntax.Body);
        }

        public override void VisitVariableBlockSyntax(VariableBlockSyntax syntax)
        {
            this.VisitNodes(syntax.Children);
        }

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
        {
            this.Visit(syntax.ConditionExpression);
            this.Visit(syntax.TrueExpression);
            this.Visit(syntax.FalseExpression);
        }

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            this.Visit(syntax.LeftExpression);
            this.Visit(syntax.RightExpression);
        }

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            this.Visit(syntax.Expression);
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.IndexExpression);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.PropertyName);
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.ResourceName);
        }

        public override void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
        {
            this.Visit(syntax.Expression);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            this.Visit(syntax.Name);
            this.VisitNodes(syntax.Children);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
            this.Visit(syntax.Name);
            this.VisitNodes(syntax.Children);
        }

        public override void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax)
        {
            this.Visit(syntax.Expression);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            this.Visit(syntax.Name);
        }

        public override void VisitDecoratorSyntax(DecoratorSyntax syntax)
        {
            this.Visit(syntax.Expression);
        }

        public override void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
        }

        public override void VisitImportDeclarationSyntax(ImportDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.SpecificationString);
            this.Visit(syntax.WithClause);
            this.Visit(syntax.AsClause);
        }

        public override void VisitImportWithClauseSyntax(ImportWithClauseSyntax syntax)
        {
            this.Visit(syntax.Config);
        }

        public override void VisitImportAsClauseSyntax(ImportAsClauseSyntax syntax)
        {
            this.Visit(syntax.Alias);
        }

        public override void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Name);
            this.Visit(syntax.Value);
        }

        public override void VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax)
        {
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Path);
        }

        public override void VisitLambdaSyntax(LambdaSyntax syntax)
        {
            this.Visit(syntax.VariableSection);
            this.Visit(syntax.Body);
        }

        public override void VisitNonNullAssertionSyntax(NonNullAssertionSyntax syntax)
        {
            this.Visit(syntax.BaseExpression);
        }
    }
}
