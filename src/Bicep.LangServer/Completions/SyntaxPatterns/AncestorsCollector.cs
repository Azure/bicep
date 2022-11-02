// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Completions.SyntaxPatterns
{
    public class AncestorsCollector : SyntaxVisitor
    {
        private readonly int offset;

        private readonly IList<SyntaxBase> ancestors = new List<SyntaxBase>();

        private AncestorsCollector(int offset)
        {
            this.offset = offset;
        }

        public static IList<SyntaxBase> CollectAncestors(SyntaxBase root, int offset)
        {
            var collector = new AncestorsCollector(offset);

            collector.Visit(root);

            return collector.ancestors;
        }

        protected override void VisitInternal(SyntaxBase node)
        {
            if (node.Span.ContainsInclusive(this.offset) && CanBeAncestor(node))
            {
                this.ancestors.Add(node);

                base.VisitInternal(node);
            }
        }

        private static bool CanBeAncestor(SyntaxBase node) => !node.IsSkipped && node is not Token or IdentifierSyntax;
    }
}
