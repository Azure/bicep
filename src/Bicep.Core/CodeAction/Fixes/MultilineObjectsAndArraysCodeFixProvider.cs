// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.CodeAction.Fixes
{
    public class MultilineObjectsAndArraysCodeFixProvider : ICodeFixProvider
    {
        public const string ConvertToMultiLineDescription = "Convert to multiline";
        public const string ConvertToSingleLineDescription = "Convert to single line";
        private const int IndentSpaces = 2;

        private static ImmutableArray<SyntaxBase> GetChildren(SyntaxBase syntax)
            => syntax switch
            {
                ObjectSyntax x => x.Children,
                ArraySyntax x => x.Children,
                _ => throw new NotImplementedException($"{nameof(syntax)} is unexpected type {syntax?.GetType()}"),
            };

        private static SyntaxBase ReplaceChildren(SyntaxBase syntax, IEnumerable<SyntaxBase> children)
            => syntax switch
            {
                ObjectSyntax x => new ObjectSyntax(x.OpenBrace, children, x.CloseBrace),
                ArraySyntax x => new ArraySyntax(x.OpenBracket, children, x.CloseBracket),
                _ => throw new NotImplementedException($"{nameof(syntax)} is unexpected type {syntax?.GetType()}"),
            };

        public IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
        {
            var objectOrArray = matchingNodes.Where(x => x is ArraySyntax or ObjectSyntax).LastOrDefault();
            if (objectOrArray is null || semanticModel.HasParsingError(objectOrArray))
            {
                yield break;
            }

            var children = GetChildren(objectOrArray);

            if (children.Any(x => x is Token { Type: TokenType.Comma }) ||
                !children.Any(x => x is Token { Type: TokenType.NewLine }))
            {
                // Array/object has some items on a single line. Let's offer to convert to a multi-line array/object
                var updatedChildren = children
                    .Where(x => x is not Token { Type: TokenType.Comma } and not Token { Type: TokenType.NewLine })
                    .SelectMany(x => new[] { SyntaxFactory.NewlineToken, x })
                    .Concat(new[] { SyntaxFactory.NewlineToken });

                var newItem = ReplaceChildren(objectOrArray, updatedChildren);

                var replacementText = FormatHelper.ToFormattedTextWithNewIndentation(semanticModel, objectOrArray, newItem, IndentSpaces);

                var codeReplacement = new CodeReplacement(objectOrArray.Span, replacementText);
                yield return new CodeFix(ConvertToMultiLineDescription, false, CodeFixKind.Refactor, codeReplacement);
            }

            if (children.Any(x => x is Token { Type: TokenType.NewLine }))
            {
                // Array/object has some items on multiple lines. Let's offer to convert to a single-line array/object
                var updatedChildren = children
                    .Where(x => x is not Token { Type: TokenType.Comma } and not Token { Type: TokenType.NewLine })
                    .SelectMany((x, i) => i == 0 ? new[] { x } : new[] { SyntaxFactory.CommaToken, x });

                var newItem = ReplaceChildren(objectOrArray, updatedChildren);

                var replacementText = FormatHelper.ToFormattedTextWithNewIndentation(semanticModel, objectOrArray, newItem, IndentSpaces);

                var codeReplacement = new CodeReplacement(objectOrArray.Span, replacementText);
                yield return new CodeFix(ConvertToSingleLineDescription, false, CodeFixKind.Refactor, codeReplacement);
            }
        }
    }
}
