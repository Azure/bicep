// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2.Documents;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2
{
    public partial class SyntaxLayouts
    {
        private int lineBreakerCount = 0;

        private Document Glue(params SyntaxBase[] syntaxes) => this.Glue(syntaxes.AsEnumerable());

        private Document Glue(IEnumerable<SyntaxBase> syntaxes) => syntaxes.Select(this.LayoutSingle).Glue();

        private Document Glue(params object[] syntaxesOrDocuments) => syntaxesOrDocuments.Select(this.ConvertToDocument).Glue();

        private Document Spread(params SyntaxBase[] syntaxes) => this.Spread(syntaxes.AsEnumerable());

        private Document Spread(IEnumerable<SyntaxBase> syntaxes) => syntaxes.Select(this.LayoutSingle).SeparateBySpace().Glue();

        private Document Spread(params object[] syntaxesOrDocuments) => syntaxesOrDocuments.Select(this.ConvertToDocument).SeparateBySpace().Glue();

        private Document Bracket(SyntaxBase openSyntax, IEnumerable<SyntaxBase> syntaxes, SyntaxBase closeSyntax, Document separator, Document padding)
        {
            var openBracket = this.LayoutSingle(openSyntax);
            var closeBracket = this.LayoutSingle(closeSyntax);

            var lineBreakerCountBefore = this.lineBreakerCount;
            var items = this.LayoutMany(syntaxes)
                .TrimNewline()
                .CollapseNewline()
                .Select(document =>
                {
                    if (document == DocumentOperators.LiteralLine)
                    {
                        this.lineBreakerCount++;
                    }

                    return document;
                })
                .SeparateBy(separator);

            if (!items.Any())
            {
                return DocumentOperators.Glue(openBracket, closeBracket);
            }

            var documents = new[]
            {
                openBracket,
                padding
                    .Concat(items)
                    .Indent(),
                padding,
                closeBracket
            };

            // The GroupDocument degenerates into a GlueDocument if it contains line breakers
            // that prevent the group from being flattened.
            return this.lineBreakerCount != lineBreakerCountBefore
                ? DocumentOperators.Glue(documents)
                : DocumentOperators.Group(documents);
        }

        private void BreakEnclosingGroups() => this.lineBreakerCount++;

        private Document ConvertToDocument(object? syntaxOrDocument) => syntaxOrDocument switch
        {
            SyntaxBase syntax => this.LayoutSingle(syntax),
            Document document => document,
            _ => throw new ArgumentOutOfRangeException(nameof(syntaxOrDocument)),
        };

    }
}
