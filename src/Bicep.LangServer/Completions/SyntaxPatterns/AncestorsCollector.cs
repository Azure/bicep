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
    public class AncestorsCollector : CstVisitor
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
            if (node.Span.ContainsInclusive(this.offset) && !IsLeafNode(node))
            {
                this.ancestors.Add(node);

                base.VisitInternal(node);
            }
        }

        // Ancestors does not include leaf nodes matching the offset in the syntax tree.
        private static bool IsLeafNode(SyntaxBase node) => node.IsSkipped || node is Token or IdentifierSyntax;
    }
}
