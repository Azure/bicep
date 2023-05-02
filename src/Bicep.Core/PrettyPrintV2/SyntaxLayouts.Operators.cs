// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2.Documents;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Bicep.Core.PrettyPrintV2.Documents.DocumentOperators;

namespace Bicep.Core.PrettyPrintV2
{
    public partial class SyntaxLayouts
    {
        private void ForceBreak() => this.lineBreakerTracker.AddOne();

        private ConcatDocument Concat(SyntaxBase first, SyntaxBase second, params SyntaxBase[] tail) =>
            this.Concat(tail.Prepend(second).Prepend(first));

        private ConcatDocument Concat(IEnumerable<SyntaxBase> syntaxes) =>
            DocumentOperators.Concat(syntaxes.Select(this.LayoutSingle));

        private ConcatDocument Concat(object first, object second, params object[] tail) =>
            DocumentOperators.Concat(tail.Prepend(second).Prepend(first).Select(this.ConvertToDocument));

        private ConcatDocument SeparateWithSpace(SyntaxBase first, SyntaxBase second, params SyntaxBase[] tail) =>
            this.SeparateWithSpace(tail.Prepend(second).Prepend(first));

        private ConcatDocument SeparateWithSpace(IEnumerable<SyntaxBase> syntaxes) =>
            DocumentOperators.SeparateWithSpace(syntaxes.Select(this.LayoutSingle));

        private ConcatDocument SeparateWithSpace(object first, object second, params object[] tail) =>
            DocumentOperators.SeparateWithSpace(tail.Prepend(second).Prepend(first).Select(this.ConvertToDocument));

        private Document Bracket(Token openToken, IEnumerable<SyntaxBase> syntaxes, Document separator, Document padding, Token closeToken)
        {
            var openBracket = this.LayoutSingle(openToken);
            var items = this.LayoutMany(syntaxes);
            var closeParts = this.Layout(closeToken).ToArray();
            var danglingComments = closeParts[..^1]; // Can be empty.
            var closeBracket = closeParts[^1];

            items = items
                .Concat(danglingComments)
                .SeparatedBy(separator)
                .Collapse(x => x == LiteralLine)
                .Trim(x => x == LiteralLine)
                .ToList();

            if (!items.Any())
            {
                return DocumentOperators.Concat(openBracket, closeBracket);
            }

            if (items.Contains(LiteralLine))
            {
                this.lineBreakerTracker.AddOne();
            }

            return Group(
                openBracket,
                Indent(
                    padding.Concat(items)),
                padding,
                closeBracket);
        }

        private Document ConvertToDocument(object? documentOrSyntax) => documentOrSyntax switch
        {
            Document document => document,
            SyntaxBase syntax => this.LayoutSingle(syntax),
            _ => throw new ArgumentOutOfRangeException(nameof(documentOrSyntax)),
        };

    }
}
