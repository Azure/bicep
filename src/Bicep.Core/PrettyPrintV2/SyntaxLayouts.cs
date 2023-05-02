// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2.Documents;
using Bicep.Core.Syntax;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Bicep.Core.PrettyPrintV2.Documents.DocumentOperators;

namespace Bicep.Core.PrettyPrintV2
{
    public partial class SyntaxLayouts
    {
        private IEnumerable<Document> LayoutArrayAccessSyntax(ArrayAccessSyntax syntax) =>
            this.Concat(
                syntax.BaseExpression,
                syntax.OpenSquare,
                syntax.IndexExpression,
                syntax.CloseSquare);

        private IEnumerable<Document> LayoutArraySyntax(ArraySyntax syntax) =>
            this.Bracket(
                syntax.OpenBracket,
                syntax.Children,
                separator: LineOrCommaSpace,
                padding: LineOrSpace,
                syntax.CloseBracket);

        private IEnumerable<Document> LayoutArrayTypeSyntax(ArrayTypeSyntax syntax) =>
            this.Concat(
                syntax.OpenBracket,
                syntax.Item,
                syntax.CloseBracket);

        private IEnumerable<Document> LayoutBinaryOperationSyntax(BinaryOperationSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.LeftExpression,
                syntax.OperatorToken,
                syntax.RightExpression);

        private IEnumerable<Document> LayoutDecoratorSyntax(DecoratorSyntax syntax) =>
            this.Concat(syntax.At, syntax.Expression);

        private IEnumerable<Document> LayoutForSyntax(ForSyntax syntax) =>
            this.Concat(
                syntax.OpenSquare,
                this.SeparateWithSpace(
                    syntax.ForKeyword,
                    syntax.VariableSection,
                    syntax.InKeyword,
                    this.Concat(
                        syntax.Expression,
                        syntax.Colon),
                    syntax.Body),
                syntax.CloseSquare);

        private IEnumerable<Document> LayoutFunctionCallSyntax(FunctionCallSyntax syntax) =>
            this.Concat(
                syntax.Name,
                this.Bracket(
                    syntax.OpenParen,
                    syntax.Children,
                    separator: CommaLineOrCommaSpace,
                    padding: LineOrEmpty,
                    syntax.CloseParen));

        private IEnumerable<Document> LayoutIfConditionSyntax(IfConditionSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.Keyword,
                syntax.ConditionExpression,
                syntax.Body);

        private IEnumerable<Document> LayoutImportAsClauseSyntax(ImportAsClauseSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.Keyword,
                syntax.Alias);

        private IEnumerable<Document> LayoutImportDeclarationSyntax(ImportDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.SeparateWithSpace(
                    syntax.Keyword,
                    syntax.SpecificationString,
                    syntax.WithClause,
                    syntax.AsClause));

        private IEnumerable<Document> LayoutImportWithClauseSyntax(ImportWithClauseSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.Keyword,
                syntax.Config);

        private IEnumerable<Document> LayoutIntanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax) =>
            this.Concat(
                syntax.BaseExpression,
                syntax.Name,
                syntax.Dot,
                this.Bracket(
                    syntax.OpenParen,
                    syntax.Children,
                    separator: CommaLineOrCommaSpace,
                    padding: LineOrEmpty,
                    syntax.CloseParen));

        private IEnumerable<Document> LayoutLambdaSyntax(LambdaSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.VariableSection,
                syntax.Arrow,
                syntax.Body);

        private IEnumerable<Document> LayoutMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.SeparateWithSpace(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Assignment,
                    syntax.Value));

        private IEnumerable<Document> LayoutMissingDeclarationSyntax(MissingDeclarationSyntax syntax) =>
            this.LayoutMany(syntax.LeadingNodes);

        private IEnumerable<Document> LayoutModuleDeclarationSyntax(ModuleDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.SeparateWithSpace(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Path,
                    syntax.Assignment,
                    syntax.Value));

        private IEnumerable<Document> LayoutNonNullAssertionSyntax(NonNullAssertionSyntax syntax) =>
            this.Concat(syntax.BaseExpression, syntax.AssertionOperator);

        private IEnumerable<Document> LayoutNullableTypeSyntax(NullableTypeSyntax syntax) =>
            this.Concat(syntax.Base, syntax.NullabilityMarker);

        private IEnumerable<Document> LayoutObjectPropertySyntax(ObjectPropertySyntax syntax) =>
            this.SeparateWithSpace(
                this.Concat(syntax.Key, syntax.Colon),
                syntax.Value);

        private IEnumerable<Document> LayoutObjectSyntax(ObjectSyntax syntax)
        {
            // Special case for objects: if the object contains a newline before
            // the first property, always break the group.
            if (syntax.Children.First() is Token token &&
                token.IsOf(TokenType.NewLine) &&
                syntax.HasProperties())
            {
                this.ForceBreak();
            }

            return this.Bracket(
                syntax.OpenBrace,
                syntax.Children,
                separator: LineOrCommaSpace,
                padding: LineOrSpace,
                syntax.CloseBrace);
        }

        private IEnumerable<Document> LayoutObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.SeparateWithSpace(
                    this.Concat(syntax.Asterisk, syntax.Colon),
                    syntax.Value));

        private IEnumerable<Document> LayoutObjectTypePropertySyntax(ObjectTypePropertySyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.SeparateWithSpace(
                    this.Concat(syntax.Key, syntax.Colon),
                    syntax.Value));

        private IEnumerable<Document> LayoutObjectTypeSyntax(ObjectTypeSyntax syntax) =>
            this.Bracket(
                syntax.OpenBrace,
                syntax.Children,
                separator: LineOrCommaSpace,
                padding: LineOrSpace,
                syntax.CloseBrace);

        private IEnumerable<Document> LayoutOutputDeclarationSyntax(OutputDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.SeparateWithSpace(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Type,
                    syntax.Assignment,
                    syntax.Value));

        private IEnumerable<Document> LayoutParameterAssignmentSyntax(ParameterAssignmentSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.Keyword,
                syntax.Name,
                syntax.Assignment,
                syntax.Value);

        private IEnumerable<Document> LayoutParameterDeclarationSyntax(ParameterDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(syntax.Modifier is not null
                    ? this.SeparateWithSpace(syntax.Keyword, syntax.Name, syntax.Type, syntax.Modifier)
                    : this.SeparateWithSpace( syntax.Keyword, syntax.Name, syntax.Type));

        private IEnumerable<Document> LayoutParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.AssignmentToken,
                syntax.DefaultValue);

        private IEnumerable<Document> LayoutParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax) =>
            this.Concat(
                syntax.OpenParen,
                syntax.Expression,
                syntax.CloseParen);

        private IEnumerable<Document> LayoutProgramSyntax(ProgramSyntax syntax) =>
            this.LayoutMany(syntax.Children)
                .Collapse(x => x == LiteralLine)
                .Trim(x => x == LiteralLine)
                .SeparatedBy(LiteralLine);

        private IEnumerable<Document> LayoutPropertyAccessSyntax(PropertyAccessSyntax syntax) =>
            this.Concat(
                syntax.BaseExpression,
                syntax.Dot,
                syntax.PropertyName);

        private IEnumerable<Document> LayoutResourceAccessSyntax(ResourceAccessSyntax syntax) =>
            this.Concat(
                syntax.BaseExpression,
                syntax.DoubleColon,
                syntax.ResourceName);

        private IEnumerable<Document> LayoutResourceDeclarationSyntax(ResourceDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.SeparateWithSpace(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Type,
                    syntax.Assignment,
                    syntax.Value));

        private IEnumerable<Document> LayoutResourceTypeSyntax(ResourceTypeSyntax syntax) =>
            syntax.Type is not null
                ? this.Concat(syntax.Keyword, syntax.Type)
                : this.LayoutSingle(syntax.Keyword);

        private IEnumerable<Document> LayoutSkippedTriviaSyntax(SkippedTriviaSyntax syntax) =>
            this.LayoutMany(syntax.Elements);

        private IEnumerable<Document> LayoutStringSyntax(StringSyntax syntax) =>
            this.Concat(syntax.StringTokens
                    .Zip(syntax.Expressions, (stringToken, expression) => new SyntaxBase[] { stringToken, expression })
                    .SelectMany(x => x)
                    .Append(syntax.StringTokens[^1]));

        private IEnumerable<Document> LayoutTargetScopeSyntax(TargetScopeSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.Keyword,
                syntax.Assignment,
                syntax.Value);

        private IEnumerable<Document> LayoutTernaryOperationSyntax(TernaryOperationSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.ConditionExpression,
                syntax.Question,
                syntax.TrueExpression,
                syntax.Colon,
                syntax.FalseExpression);

        private IEnumerable<Document> LayoutTupleTypeItemSyntax(TupleTypeItemSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.LayoutSingle(syntax.Value));

        private IEnumerable<Document> LayoutTupleTypeSyntax(TupleTypeSyntax syntax) =>
            this.Bracket(
                syntax.OpenBracket,
                syntax.Children,
                separator: LineOrCommaSpace,
                padding: LineOrSpace,
                syntax.CloseBracket);

        private IEnumerable<Document> LayoutTypeDeclarationSyntax(TypeDeclarationSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.Keyword,
                syntax.Name,
                syntax.Assignment,
                syntax.Value);

        private IEnumerable<Document> LayoutUnaryOperationSyntax(UnaryOperationSyntax syntax) =>
            this.Concat(
                syntax.OperatorToken,
                syntax.Expression);

        private IEnumerable<Document> LayoutUnionTypeSyntax(UnionTypeSyntax syntax) =>
            this.SeparateWithSpace(syntax.Children);

        private IEnumerable<Document> LayoutUsingDeclarationSyntax(UsingDeclarationSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.Keyword,
                syntax.Path);

        private IEnumerable<Document> LayoutVariableBlockSyntax(VariableBlockSyntax syntax) =>
            this.Concat(
                syntax.OpenParen,
                this.LayoutMany(syntax.Children)
                    .SeparatedBy(", "),
                syntax.CloseParen);

        private IEnumerable<Document> LayoutVariableDeclarationSyntax(VariableDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.SeparateWithSpace(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Assignment,
                    syntax.Value));
        
        private IEnumerable<Document> LayoutTypedVariableBlockSyntax(TypedVariableBlockSyntax syntax) =>
            this.Bracket(
                syntax.OpenParen,
                syntax.Children,
                separator: CommaLineOrCommaSpace,
                padding: LineOrEmpty,
                syntax.CloseParen);

        public IEnumerable<Document> LayoutTypedLocalVariableSyntax(TypedLocalVariableSyntax syntax) =>
            this.Concat(
                syntax.Name,
                syntax.Type);

        public IEnumerable<Document> LayoutTypedLambdaSyntax(TypedLambdaSyntax syntax) =>
            this.SeparateWithSpace(
                syntax.VariableSection,
                syntax.ReturnType,
                syntax.Arrow,
                syntax.Body);

        public IEnumerable<Document> LayoutFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.SeparateWithSpace(
                    syntax.Keyword,
                    this.Concat(
                        syntax.Name,
                        syntax.Lambda)));


        private IEnumerable<Document> LayoutToken(Token token)
        {
            var leadingComments = this.LayoutComments(token.LeadingComments);
            var trailingComments = this.LayoutComments(token.TrailingComments);
            var commentStickiness = token.GetCommentStickiness();

            switch (commentStickiness)
            {
                case CommentStickiness.None:
                    Debug.Assert(token.IsOneOf(TokenType.NewLine, TokenType.Comma));
                    Debug.Assert(!trailingComments.Any());

                    if (leadingComments.Any())
                    {
                        this.ForceBreak();
                    }

                    return token.IsMultiLineNewLine() ? leadingComments.Append(LiteralLine) : leadingComments;

                case CommentStickiness.Leading:
                    Debug.Assert(!trailingComments.Any());

                    return DocumentOperators.SeparateWithSpace(leadingComments.Append(token.Text));

                case CommentStickiness.Trailing:
                    if (leadingComments.Any())
                    {
                        this.ForceBreak();
                    }

                    return leadingComments.Concat(DocumentOperators.SeparateWithSpace(trailingComments.Prepend(token.Text)));

                default:
                    Debug.Assert(commentStickiness == CommentStickiness.Bidirectional);
                    
                    return DocumentOperators.SeparateWithSpace(leadingComments.Append(token.Text).Concat(trailingComments));
            }
        }

        private IEnumerable<Document> LayoutComments(IEnumerable<SyntaxTrivia> comments)
        {
            foreach (var comment in comments)
            {
                if (comment.IsSingleLineComment())
                {
                    this.ForceBreak();
                }

                yield return comment.Text;
            }
        }

        // Remove empty lines between decorators.
        private IEnumerable<Document> LayoutLeadingNodes(IEnumerable<SyntaxBase> leadingNodes) =>
            this.LayoutMany(leadingNodes)
                .Where(x => x != LiteralLine);
    }
}
