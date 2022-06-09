// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.CodeAction.Fixes
{
    public class MultilineObjectsAndArraysCodeFixProvider : ICodeFixProvider
    {
        public const string ConvertToMultiLineDescription = "Convert to multi line";
        public const string ConvertToSingleLineDescription = "Convert to single line";

        public IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
        {
            var objectOrArray = matchingNodes.Where(x => x is ArraySyntax or ObjectSyntax).LastOrDefault();
            if (objectOrArray is null || objectOrArray.HasParseErrors())
            {
                yield break;
            }

            var children = objectOrArray switch {
                ObjectSyntax x => x.Children,
                ArraySyntax x => x.Children,
                _ => throw new NotImplementedException($"{nameof(objectOrArray)} is unexpected type {objectOrArray?.GetType()}"),
            };

            if (children.Any(x => x is Token { Type: TokenType.Comma }) ||
                !children.Any(x => x is Token { Type: TokenType.NewLine }))
            {
                // expression has multiple or all items on one line
                var updatedChildren = children
                    .Where(x => x is not Token { Type: TokenType.Comma } and not Token { Type: TokenType.NewLine })
                    .SelectMany(x => new [] { SyntaxFactory.NewlineToken, x })
                    .Concat(new [] { SyntaxFactory.NewlineToken });

                SyntaxBase newItem = objectOrArray switch {
                    ObjectSyntax x => new ObjectSyntax(x.OpenBrace, updatedChildren, x.CloseBrace),
                    ArraySyntax x => new ArraySyntax(x.OpenBracket, updatedChildren, x.CloseBracket),
                    _ => throw new NotImplementedException($"{nameof(objectOrArray)} is unexpected type {objectOrArray?.GetType()}"),
                };

                var codeReplacement = new CodeReplacement(objectOrArray.Span, newItem.ToText(indent: "  "));
                yield return new CodeFix(ConvertToMultiLineDescription, false, CodeFixKind.Refactor, codeReplacement);
            }

            if (children.Any(x => x is Token { Type: TokenType.NewLine }))
            {
                // expression has items across multiple lines
                var updatedChildren = children
                    .Where(x => x is not Token { Type: TokenType.Comma } and not Token { Type: TokenType.NewLine })
                    .SelectMany((x, i) => i == 0 ? new [] { x } : new[] { SyntaxFactory.CommaToken, x });

                SyntaxBase newItem = objectOrArray switch {
                    ObjectSyntax x => new ObjectSyntax(x.OpenBrace, updatedChildren, x.CloseBrace),
                    ArraySyntax x => new ArraySyntax(x.OpenBracket, updatedChildren, x.CloseBracket),
                    _ => throw new NotImplementedException($"{nameof(objectOrArray)} is unexpected type {objectOrArray?.GetType()}"),
                };

                var codeReplacement = new CodeReplacement(objectOrArray.Span, newItem.ToText(indent: "  "));
                yield return new CodeFix(ConvertToSingleLineDescription, false, CodeFixKind.Refactor, codeReplacement);
            }
        }
    }
}
