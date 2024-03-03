// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.PrettyPrintV2.Documents;
using Bicep.Core.Syntax;

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

        private Document IndentGroup(IEnumerable<SyntaxBase> syntaxes) =>
            this.IndentGroup(() => this.LayoutMany(syntaxes));

        private Document IndentGroup(Func<IEnumerable<Document>> layoutSpecifier)
        {
            var lineBreakerCountBefore = this.lineBreakerCount;
            var indented = layoutSpecifier()
                .Where(x => x is not LineDocument)
                .SeparateBy(DocumentOperators.LineOrSpace)
                .Prepend(DocumentOperators.LineOrEmpty)
                .Indent();

            if (indented.HasSuffix())
            {
                this.lineBreakerCount--;
            }

            return this.lineBreakerCount > lineBreakerCountBefore
                ? DocumentOperators.Glue(indented)
                : DocumentOperators.Group(indented);
        }

        private Document IndentTail(Func<IEnumerable<Document>> layoutSpecifier)
        {
            var lineBreakerCountBefore = this.lineBreakerCount;
            var indented = layoutSpecifier()
                .Where(x => x is not LineDocument)
                .SeparateBy(DocumentOperators.LineOrSpace)
                .Indent();

            if (indented.HasSuffix())
            {
                this.lineBreakerCount--;
            }

            return this.lineBreakerCount > lineBreakerCountBefore
                ? DocumentOperators.Glue(indented)
                : DocumentOperators.Group(indented);
        }

        private Document IndentTail(IEnumerable<SyntaxBase> syntaxes) =>
            this.IndentTail(() => this.LayoutMany(syntaxes));

        private Document Bracket(SyntaxBase openSyntax, Func<IEnumerable<Document>> itemsLayoutSpecifier, SyntaxBase closeSyntax, Document separator, Document padding, bool forceBreak = false, bool indentSingleItem = true)
        {
            var openBracket = this.LayoutSingle(openSyntax);
            var closeParts = this.Layout(closeSyntax).ToArray();
            var danglingComments = closeParts[..^1];
            var closeBracket = closeParts[^1];

            var lineBreakerCountBefore = this.lineBreakerCount;
            var items = itemsLayoutSpecifier()
                .TrimNewlines()
                .CollapseNewlines(onHardLine: this.ForceBreak)
                .Concat(danglingComments)
                .SeparateBy(separator)
                .ToArray();

            if (items.Length == 0)
            {
                return DocumentOperators.Glue(openBracket, closeBracket);
            }

            if (items.Length == 1 && !indentSingleItem)
            {
                if (items[0] is not ContainerDocument container || !container.HasSuffix())
                {
                    return DocumentOperators.Glue(
                        openBracket,
                        padding.Flatten().Glue(),
                        items[0],
                        padding.Flatten().Glue(),
                        closeBracket);
                }
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

        private Document Bracket(SyntaxBase openSyntax, IEnumerable<SyntaxBase> syntaxes, SyntaxBase closeSyntax, Document separator, Document padding, bool forceBreak = false, bool indentSingleItem = true) =>
            this.Bracket(openSyntax, () => this.LayoutMany(syntaxes), closeSyntax, separator, padding, forceBreak, indentSingleItem);


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
