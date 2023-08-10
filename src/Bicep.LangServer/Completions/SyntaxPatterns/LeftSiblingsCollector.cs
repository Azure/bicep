// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using System.Collections.Generic;

namespace Bicep.LanguageServer.Completions.SyntaxPatterns
{
    public class LeftSiblingsCollector : CstVisitor
    {
        private readonly int offset;

        private readonly IList<SyntaxBase> leftSiblings = new List<SyntaxBase>();

        private SyntaxBase? overlappingNode = null;

        private LeftSiblingsCollector(int offset)
        {
            this.offset = offset;
        }

        public static (IList<SyntaxBase> LeftSiblings, SyntaxBase? overlappingNode) CollectLeftSiblings(SyntaxBase parent, int offset)
        {
            var collector = new LeftSiblingsCollector(offset);

            collector.Visit(parent);

            return (collector.leftSiblings, collector.overlappingNode);
        }

        public override void VisitProviderDeclarationSyntax(ProviderDeclarationSyntax syntax) =>
            this.AddLeftSiblings(syntax.Keyword, syntax.SpecificationString, syntax.WithClause);

        public override void VisitProviderWithClauseSyntax(ProviderWithClauseSyntax syntax) =>
            this.AddLeftSiblings(syntax.Keyword, syntax.Config);

        public override void VisitAliasAsClauseSyntax(AliasAsClauseSyntax syntax) =>
            this.AddLeftSiblings(syntax.Keyword, syntax.Alias);

        private void AddLeftSiblings(params SyntaxBase[] candidates)
        {
            foreach (var candidate in candidates)
            {
                if (candidate.IsOverlapping(this.offset))
                {
                    this.overlappingNode = candidate;

                    return;
                }

                if (candidate.GetEndPosition() < this.offset)
                {
                    this.leftSiblings.Add(candidate);
                }
            }
        }
    }
}
