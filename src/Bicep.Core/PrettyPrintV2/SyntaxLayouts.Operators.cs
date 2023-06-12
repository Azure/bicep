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

        private Document Spread(IEnumerable<SyntaxBase> syntaxes) => syntaxes.Select(this.LayoutSingle).Spread();

        private Document Spread(params object[] syntaxesOrDocuments) => syntaxesOrDocuments.Select(this.ConvertToDocument).Spread();

        private Document Bracket(SyntaxBase openSyntax, IEnumerable<SyntaxBase> syntaxes, SyntaxBase closeSyntax, Document separator, Document padding, bool forceBreak = false)
        {
            var openBracket = this.LayoutSingle(openSyntax);
            var closeParts = this.Layout(closeSyntax).ToArray();
            var danglingComments = closeParts[..^1];
            var closeBracket = closeParts[^1];

            var lineBreakerCountBefore = this.lineBreakerCount;
            var items = this.LayoutMany(syntaxes)
                .TrimNewlines()
                .CollapseNewlines(onHardLine: this.ForceBreak)
                .Concat(danglingComments)
                .SeparateBy(separator);

            if (!items.Any())
            {
                return DocumentOperators.Glue(openBracket, closeBracket);
            }

            if (forceBreak || danglingComments.Length > 0)
            {
                this.ForceBreak();
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
            return this.lineBreakerCount > lineBreakerCountBefore
                ? DocumentOperators.Glue(documents)
                : DocumentOperators.Group(documents);
        }

        /// <summary>
        /// Breaks the enclosing parent groups.
        /// </summary>
        private void ForceBreak() => this.lineBreakerCount++;

        private Document ConvertToDocument(object? syntaxOrDocument) => syntaxOrDocument switch
        {
            SyntaxBase syntax => this.LayoutSingle(syntax),
            Document document => document,
            _ => throw new ArgumentOutOfRangeException(nameof(syntaxOrDocument)),
        };

    }
}
