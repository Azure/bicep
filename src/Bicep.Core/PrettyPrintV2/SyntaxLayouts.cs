// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
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
            this.Glue(
                syntax.BaseExpression,
                syntax.OpenSquare,
                syntax.IndexExpression,
                syntax.CloseSquare);

        private IEnumerable<Document> LayoutArraySyntax(ArraySyntax syntax) =>
            this.Bracket(
                syntax.OpenBracket,
                syntax.Children,
                syntax.CloseBracket,
                separator: LineOrCommaSpace,
                padding: LineOrEmpty);

        private IEnumerable<Document> LayoutArrayTypeSyntax(ArrayTypeSyntax syntax) =>
            this.Glue(
                syntax.OpenBracket,
                syntax.Item,
                syntax.CloseBracket);

        private IEnumerable<Document> LayoutBinaryOperationSyntax(BinaryOperationSyntax syntax) =>
            this.Spread(
                syntax.LeftExpression,
                syntax.OperatorToken,
                syntax.RightExpression);

        private IEnumerable<Document> LayoutDecoratorSyntax(DecoratorSyntax syntax) =>
            this.Glue(syntax.At, syntax.Expression);

        private IEnumerable<Document> LayoutForSyntax(ForSyntax syntax) =>
            this.Glue(
                syntax.OpenSquare,
                this.Spread(
                    syntax.ForKeyword,
                    syntax.VariableSection,
                    syntax.InKeyword,
                    this.Glue(
                        syntax.Expression,
                        syntax.Colon),
                    syntax.Body),
                syntax.CloseSquare);

        private IEnumerable<Document> LayoutFunctionCallSyntax(FunctionCallSyntax syntax) =>
            this.Glue(
                syntax.Name,
                this.Bracket(
                    syntax.OpenParen,
                    syntax.Children,
                    syntax.CloseParen,
                    separator: CommaLineOrCommaSpace,
                    padding: LineOrEmpty));

        private IEnumerable<Document> LayoutIfConditionSyntax(IfConditionSyntax syntax) =>
            this.Spread(
                syntax.Keyword,
                syntax.ConditionExpression,
                syntax.Body);

        private IEnumerable<Document> LayoutImportAsClauseSyntax(ImportAsClauseSyntax syntax) =>
            this.Spread(
                syntax.Keyword,
                syntax.Alias);

        private IEnumerable<Document> LayoutImportDeclarationSyntax(ImportDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.Spread(
                    syntax.Keyword,
                    syntax.SpecificationString,
                    syntax.WithClause,
                    syntax.AsClause));

        private IEnumerable<Document> LayoutImportWithClauseSyntax(ImportWithClauseSyntax syntax) =>
            this.Spread(
                syntax.Keyword,
                syntax.Config);

        private IEnumerable<Document> LayoutIntanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax) =>
            this.Glue(
                syntax.BaseExpression,
                syntax.Name,
                syntax.Dot,
                this.Bracket(
                    syntax.OpenParen,
                    syntax.Children,
                    syntax.CloseParen,
                    separator: CommaLineOrCommaSpace,
                    padding: LineOrEmpty));

        private IEnumerable<Document> LayoutLambdaSyntax(LambdaSyntax syntax) =>
            this.Spread(
                syntax.VariableSection,
                syntax.Arrow,
                syntax.Body);

        private IEnumerable<Document> LayoutMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.Spread(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Assignment,
                    syntax.Value));

        private IEnumerable<Document> LayoutMissingDeclarationSyntax(MissingDeclarationSyntax syntax) =>
            this.LayoutMany(syntax.LeadingNodes);

        private IEnumerable<Document> LayoutModuleDeclarationSyntax(ModuleDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.Spread(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Path,
                    syntax.Assignment,
                    syntax.Value));

        private IEnumerable<Document> LayoutNonNullAssertionSyntax(NonNullAssertionSyntax syntax) =>
            this.Glue(syntax.BaseExpression, syntax.AssertionOperator);

        private IEnumerable<Document> LayoutNullableTypeSyntax(NullableTypeSyntax syntax) =>
            this.Glue(syntax.Base, syntax.NullabilityMarker);

        private IEnumerable<Document> LayoutObjectPropertySyntax(ObjectPropertySyntax syntax) =>
            this.Spread(
                this.Glue(syntax.Key, syntax.Colon),
                syntax.Value);

        private IEnumerable<Document> LayoutObjectSyntax(ObjectSyntax syntax)
        {
            // Special case for objects: if the object contains a newline before
            // the first property, always break the the object.
            if (syntax.Children.First() is Token token &&
                token.IsOf(TokenType.NewLine) &&
                syntax.HasProperties())
            {
                this.BreakEnclosingGroups();
            }

            return this.Bracket(
                syntax.OpenBrace,
                syntax.Children,
                syntax.CloseBrace,
                separator: LineOrCommaSpace,
                padding: LineOrSpace);
        }

        private IEnumerable<Document> LayoutObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.Spread(
                    this.Glue(syntax.Asterisk, syntax.Colon),
                    syntax.Value));

        private IEnumerable<Document> LayoutObjectTypePropertySyntax(ObjectTypePropertySyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.Spread(
                    this.Glue(syntax.Key, syntax.Colon),
                    syntax.Value));

        private IEnumerable<Document> LayoutObjectTypeSyntax(ObjectTypeSyntax syntax) =>
            this.Bracket(
                syntax.OpenBrace,
                syntax.Children,
                syntax.CloseBrace,
                separator: LineOrCommaSpace,
                padding: LineOrSpace);

        private IEnumerable<Document> LayoutOutputDeclarationSyntax(OutputDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.Spread(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Type,
                    syntax.Assignment,
                    syntax.Value));

        private IEnumerable<Document> LayoutParameterAssignmentSyntax(ParameterAssignmentSyntax syntax) =>
            this.Spread(
                syntax.Keyword,
                syntax.Name,
                syntax.Assignment,
                syntax.Value);

        private IEnumerable<Document> LayoutParameterDeclarationSyntax(ParameterDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(syntax.Modifier is not null
                    ? this.Spread(syntax.Keyword, syntax.Name, syntax.Type, syntax.Modifier)
                    : this.Spread(syntax.Keyword, syntax.Name, syntax.Type));

        private IEnumerable<Document> LayoutParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax) =>
            this.Spread(
                syntax.AssignmentToken,
                syntax.DefaultValue);

        private IEnumerable<Document> LayoutParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax) =>
            this.Glue(
                syntax.OpenParen,
                syntax.Expression,
                syntax.CloseParen);

        private IEnumerable<Document> LayoutProgramSyntax(ProgramSyntax syntax) =>
            this.LayoutMany(syntax.Children)
                .TrimNewline()
                .CollapseNewline()
                .SeparatedByNewline();

        private IEnumerable<Document> LayoutPropertyAccessSyntax(PropertyAccessSyntax syntax) =>
            this.Glue(
                syntax.BaseExpression,
                syntax.Dot,
                syntax.PropertyName);

        private IEnumerable<Document> LayoutResourceAccessSyntax(ResourceAccessSyntax syntax) =>
            this.Glue(
                syntax.BaseExpression,
                syntax.DoubleColon,
                syntax.ResourceName);

        private IEnumerable<Document> LayoutResourceDeclarationSyntax(ResourceDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.Spread(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Type,
                    syntax.Assignment,
                    syntax.Value));

        private IEnumerable<Document> LayoutResourceTypeSyntax(ResourceTypeSyntax syntax) =>
            syntax.Type is not null
                ? this.Glue(syntax.Keyword, syntax.Type)
                : this.LayoutSingle(syntax.Keyword);

        private IEnumerable<Document> LayoutSkippedTriviaSyntax(SkippedTriviaSyntax syntax) =>
            TextDocument.From(syntax
                .ToTextPreserveFormatting()
                .ReplaceLineEndings(this.options.Newline));

        private IEnumerable<Document> LayoutStringSyntax(StringSyntax syntax) =>
            this.Glue(syntax.StringTokens
                    .Zip(syntax.Expressions, (stringToken, expression) => new SyntaxBase[] { stringToken, expression })
                    .SelectMany(x => x)
                    .Append(syntax.StringTokens[^1]));

        private IEnumerable<Document> LayoutTargetScopeSyntax(TargetScopeSyntax syntax) =>
            this.Spread(
                syntax.Keyword,
                syntax.Assignment,
                syntax.Value);

        private IEnumerable<Document> LayoutTernaryOperationSyntax(TernaryOperationSyntax syntax) =>
            this.Spread(
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
                syntax.CloseBracket,
                separator: LineOrCommaSpace,
                padding: LineOrSpace);

        private IEnumerable<Document> LayoutTypeDeclarationSyntax(TypeDeclarationSyntax syntax) =>
            this.Spread(
                syntax.Keyword,
                syntax.Name,
                syntax.Assignment,
                syntax.Value);

        private IEnumerable<Document> LayoutUnaryOperationSyntax(UnaryOperationSyntax syntax) =>
            this.Glue(
                syntax.OperatorToken,
                syntax.Expression);

        private IEnumerable<Document> LayoutUnionTypeSyntax(UnionTypeSyntax syntax) =>
            this.Spread(syntax.Children);

        private IEnumerable<Document> LayoutUsingDeclarationSyntax(UsingDeclarationSyntax syntax) =>
            this.Spread(
                syntax.Keyword,
                syntax.Path);

        private IEnumerable<Document> LayoutVariableBlockSyntax(VariableBlockSyntax syntax) =>
            this.Bracket(
                syntax.OpenParen,
                syntax.Children,
                syntax.CloseParen,
                separator: CommaLineOrCommaSpace,
                padding: LineOrEmpty);

        private IEnumerable<Document> LayoutVariableDeclarationSyntax(VariableDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.Spread(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Assignment,
                    syntax.Value));

        private IEnumerable<Document> LayoutTypedVariableBlockSyntax(TypedVariableBlockSyntax syntax) =>
            this.Bracket(
                syntax.OpenParen,
                syntax.Children,
                syntax.CloseParen,
                separator: CommaLineOrCommaSpace,
                padding: LineOrEmpty);

        public IEnumerable<Document> LayoutTypedLocalVariableSyntax(TypedLocalVariableSyntax syntax) =>
            this.Glue(
                syntax.Name,
                syntax.Type);

        public IEnumerable<Document> LayoutTypedLambdaSyntax(TypedLambdaSyntax syntax) =>
            this.Spread(
                syntax.VariableSection,
                syntax.ReturnType,
                syntax.Arrow,
                syntax.Body);

        public IEnumerable<Document> LayoutFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.Spread(
                    syntax.Keyword,
                    this.Glue(
                        syntax.Name,
                        syntax.Lambda)));

        private IEnumerable<Document> LayoutToken(Token token)
        {
            var leadingComments = this.LayoutComments(token.LeadingComments);
            var trailingComments = this.LayoutComments(token.TrailingComments);
            var commentStickiness = token.GetCommentStickiness();

            switch (commentStickiness)
            {
                case CommentStickiness.Bidirectional:
                    if (token.IsOf(TokenType.NewLine))
                    {
                        if (leadingComments.Any() || trailingComments.Any())
                        {
                            this.BreakEnclosingGroups();
                        }

                        return token.IsMultiLineNewLine()
                            ? leadingComments.Append(LiteralLine).Concat(trailingComments)
                            : leadingComments.Concat(trailingComments);
                    }

                    var text = token.IsOf(TokenType.MultilineString)
                        ? token.Text.ReplaceLineEndings(this.options.Newline)
                        : token.Text;

                    return leadingComments.Any() || trailingComments.Any()
                        ? leadingComments.Append(text).Concat(trailingComments).SeparateBySpace().Glue()
                        : text;

                case CommentStickiness.Leading:
                    return leadingComments.Any()
                        ? leadingComments.Append(token.Text).SeparateBySpace().Glue()
                        : token.Text;

                case CommentStickiness.Trailing:
                    return trailingComments.Any()
                        ? trailingComments.Prepend(token.Text).SeparateBySpace().Glue()
                        : token.Text;

                case CommentStickiness.None:
                    return token.IsOf(TokenType.Comma)
                        ? Enumerable.Empty<Document>()
                        : TextDocument.From(token.Text);

                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerable<Document> LayoutComments(IEnumerable<SyntaxTrivia> comments)
        {
            foreach (var comment in comments)
            {
                if (comment.IsSingleLineComment())
                {
                    this.BreakEnclosingGroups();
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
