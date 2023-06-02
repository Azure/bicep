// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2.Documents;
using Bicep.Core.Syntax;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
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
                syntax.Item,
                syntax.OpenBracket,
                syntax.CloseBracket);

        private IEnumerable<Document> LayoutBinaryOperationSyntax(BinaryOperationSyntax syntax) =>
            this.Spread(
                syntax.LeftExpression,
                syntax.OperatorToken,
                syntax.RightExpression);

        private IEnumerable<Document> LayoutDecoratorSyntax(DecoratorSyntax syntax) =>
            this.Glue(syntax.At, syntax.Expression);

        private IEnumerable<Document> LayoutForSyntax(ForSyntax syntax)
        {
            var variableSection = syntax.VariableSection switch
            {
                LocalVariableSyntax localVariable => this.LayoutSingle(localVariable),
                VariableBlockSyntax variableBlock => this.LayoutSingle(variableBlock) switch
                {
                    // The parser does not support multi-line VariableBlockSyntax, so flattening it.
                    GroupDocument group => group.Flatten().Glue(),
                    var document => document,
                },

                _ => throw new NotImplementedException()
            };

            return this.Glue(
                syntax.OpenSquare,
                this.Spread(
                    syntax.ForKeyword,
                    variableSection,
                    syntax.InKeyword,
                    this.Glue(
                        syntax.Expression,
                        syntax.Colon),
                    syntax.Body),
                syntax.CloseSquare);
        }

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
                syntax.Dot,
                syntax.Name,
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

        private IEnumerable<Document> LayoutObjectSyntax(ObjectSyntax syntax) =>
            this.Bracket(
                syntax.OpenBrace,
                syntax.Children,
                syntax.CloseBrace,
                separator: LineOrCommaSpace,
                padding: LineOrSpace,
                forceBreak:
                    // Special case for objects: if the object contains a newline before
                    // the first property, always break the the object.
                    StartsWithNewline(syntax.Children) &&
                    syntax.Properties.Any());

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
                padding: LineOrSpace,
                forceBreak:
                    // Special case for object types: if it contains a newline before
                    // the first property, always break the the object.
                    StartsWithNewline(syntax.Children) &&
                    syntax.Children.Any(x => x is ObjectTypePropertySyntax or ObjectTypeAdditionalPropertiesSyntax));

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
            this.LayoutMany(syntax.Children.Append(syntax.EndOfFile))
                .TrimNewlines()
                .CollapseNewlines()
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

        private IEnumerable<Document> LayoutSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            var text = SyntaxStringifier.Stringify(syntax, this.context.Newline).Trim().AsSpan();
            var trailingNewlineCount = 0;

            while (text.EndsWith(this.context.Newline))
            {
                text = text[0..(text.Length - this.context.Newline.Length)];
                trailingNewlineCount++;
            }

            yield return text.ToString();

            if (trailingNewlineCount > 1)
            {
                yield return HardLine;
            }
        }

        private IEnumerable<Document> LayoutStringSyntax(StringSyntax syntax)
        {
            var leadingTrivia = this.LayoutLeadingTrivia(syntax.StringTokens[0].LeadingTrivia);
            var trailingTrivia = this.LayoutTrailingTrivia(syntax.StringTokens[^1].TrailingTrivia, out var suffix);

            var writer = new StringWriter();

            for (var i = 0; i < syntax.Expressions.Length; i++)
            {
                writer.Write(syntax.StringTokens[i].Text);
                SyntaxStringifier.StringifyTo(writer, syntax.Expressions[i], this.context.Newline);
            }

            writer.Write(syntax.StringTokens[^1].Text);

            return LayoutWithLeadingAndTrailingTrivia(writer.ToString(), leadingTrivia, trailingTrivia, suffix);
        }

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
                padding: LineOrEmpty);

        private IEnumerable<Document> LayoutTypeDeclarationSyntax(TypeDeclarationSyntax syntax) =>
            this.LayoutLeadingNodes(syntax.LeadingNodes)
                .Concat(this.Spread(
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Assignment,
                    syntax.Value));

        private IEnumerable<Document> LayoutUnaryOperationSyntax(UnaryOperationSyntax syntax) =>
            this.Glue(
                syntax.OperatorToken,
                syntax.Expression);

        private IEnumerable<Document> LayoutUnionTypeSyntax(UnionTypeSyntax syntax) =>
            this.LayoutMany(syntax.Children)
                .Spread();

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
            this.Spread(
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

        private IEnumerable<Document> LayoutLeadingNodes(IEnumerable<SyntaxBase> leadingNodes) =>
            this.LayoutMany(leadingNodes)
                .Where(x => x != HardLine); // Remove empty lines between decorators.

        private IEnumerable<Document> LayoutToken(Token token)
        {
            var commentStickiness = SyntaxFacts.GetCommentStickiness(token.Type);

            if (commentStickiness == CommentStickiness.None)
            {
                return token.IsOf(TokenType.Comma) ? Empty : TextDocument.From(token.Text);
            }

            var leadingTrivia = this.LayoutLeadingTrivia(token.LeadingTrivia);

            if (commentStickiness == CommentStickiness.Leading)
            {
                return this.LayoutWithLeadingTrivia(token, leadingTrivia);
            }

            var trailingTrivia = LayoutTrailingTrivia(token.TrailingTrivia, out var suffix);

            if (commentStickiness == CommentStickiness.Trailing)
            {
                return LayoutWithTrailingTrivia(token, leadingTrivia, trailingTrivia, suffix);
            }

            if (commentStickiness == CommentStickiness.Bidirectional)
            {
                return LayoutWithLeadingAndTrailingTrivia(token.Text, leadingTrivia, trailingTrivia, suffix);
            }

            throw new NotImplementedException($"Cannot handle {commentStickiness}");
        }

        private IEnumerable<Document> LayoutWithLeadingTrivia(Token token, IEnumerable<Document> leadingTrivia)
        {
            var hasLeadingTrivia = leadingTrivia.Any();

            if (token.IsOf(TokenType.EndOfFile))
            {
                return hasLeadingTrivia ? leadingTrivia.Spread() : Empty;
            }

            if (token.IsOf(TokenType.NewLine))
            {
                var printHardLine = StringUtils.CountNewlines(token.Text) > 1;

                if (hasLeadingTrivia)
                {
                    this.ForceBreak();

                    leadingTrivia = leadingTrivia.Spread();

                    return printHardLine ? leadingTrivia.Append(HardLine) : leadingTrivia.Append(SoftLine);
                }

                return printHardLine ? HardLine : SoftLine;
            }

            return hasLeadingTrivia ? leadingTrivia.Append(token.Text).Spread() : token.Text;
        }

        private static IEnumerable<Document> LayoutWithTrailingTrivia(Token token, IEnumerable<Document> danglingLeadingTrivia, IEnumerable<Document> trailingTrivia, SuffixDocument? suffix)
        {
            var text = trailingTrivia.Any() ? trailingTrivia.Prepend(token.Text).Spread() : token.Text;

            if (suffix is not null)
            {
                text = DocumentOperators.Glue(text, suffix);
            }

            // Tokens such as ), ], and } may have dangling leading comments attached to them.
            return danglingLeadingTrivia.Any() ? danglingLeadingTrivia.Append(text) : text;
        }

        private static IEnumerable<Document> LayoutWithLeadingAndTrailingTrivia(Document text, IEnumerable<Document> leadingTrivia, IEnumerable<Document> trailingTrivia, SuffixDocument? suffix)
        {
            if (leadingTrivia.Any() || trailingTrivia.Any())
            {
                text = leadingTrivia
                    .Append(text)
                    .Concat(trailingTrivia)
                    .Spread();
            }

            return suffix is not null ? DocumentOperators.Glue(text, suffix) : text;
        }

        private IEnumerable<Document> LayoutLeadingTrivia(ImmutableArray<SyntaxTrivia> trivia)
        {
            foreach (var triviaItem in trivia)
            {
                if (triviaItem.IsOf(SyntaxTriviaType.Whitespace))
                {
                    continue;
                }

                if (triviaItem.IsOf(SyntaxTriviaType.SingleLineComment))
                {
                    this.ForceBreak();
                }

                if (triviaItem is DisableNextLineDiagnosticsSyntaxTrivia disableNextLineDirective)
                {
                    var diagnosticCodes = string.Join(" ", disableNextLineDirective.DiagnosticCodes.Select(x => x.Text));
                    yield return $"#disable-next-line {diagnosticCodes}";
                    continue;
                }

                // Trim newlines to handle unterminated multi-line comments.
                yield return triviaItem.Text.TrimEnd('\r', '\n');
            }
        }

        private IEnumerable<Document> LayoutTrailingTrivia(ImmutableArray<SyntaxTrivia> trivia, out SuffixDocument? suffix)
        {
            suffix = null;

            if (trivia.Length == 0)
            {
                return Empty;
            }

            var trailingTrivia = new List<Document>();

            foreach (var triviaItem in trivia)
            {
                if (triviaItem.IsOf(SyntaxTriviaType.Whitespace))
                {
                    continue;
                }

                if (triviaItem.IsOf(SyntaxTriviaType.SingleLineComment))
                {
                    this.ForceBreak();

                    // Trailing single-line comment should be ignored
                    // when calculating occupied width for the current line,
                    // so making it a zero-length suffix.
                    suffix = new($" {triviaItem.Text}");

                    // There cannot exist any trivia item after a
                    // single-line comment that is not a whitespace.
                    break;
                }

                // Trim newlines to handle unterminated multi-line comments.
                trailingTrivia.Add(triviaItem.Text.TrimEnd('\r', '\n'));
            }

            return trailingTrivia;
        }

        private static bool StartsWithNewline(IEnumerable<SyntaxBase> syntaxes) =>
            syntaxes.FirstOrDefault() is Token { Type: TokenType.NewLine };
    }
}
