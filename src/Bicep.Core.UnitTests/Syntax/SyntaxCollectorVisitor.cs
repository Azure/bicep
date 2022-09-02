// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Syntax
{
    public class SyntaxCollectorVisitor : SyntaxVisitor
    {
        public record SyntaxItem(SyntaxBase Syntax, SyntaxItem? Parent, int Depth)
        {
            public IEnumerable<SyntaxCollectorVisitor.SyntaxItem> GetAncestors()
            {
                var data = this;
                while (data.Parent is {} parent)
                {
                    yield return parent;
                    data = parent;
                }
            }
        }

        private readonly IList<SyntaxItem> syntaxList = new List<SyntaxItem>();
        private SyntaxItem? parent = null;
        private int depth = 0;

        private SyntaxCollectorVisitor()
        {
        }

        public static ImmutableArray<SyntaxItem> Build(SyntaxBase syntax)
        {
            var visitor = new SyntaxCollectorVisitor();
            visitor.Visit(syntax);

            return visitor.syntaxList.ToImmutableArray();
        }

        protected override void VisitInternal(SyntaxBase syntax)
        {
            var syntaxItem = new SyntaxItem(Syntax: syntax, Parent: parent, Depth: depth);
            syntaxList.Add(syntaxItem);

            var prevParent = parent;
            parent = syntaxItem;
            depth++;
            base.VisitInternal(syntax);
            depth--;
            parent = prevParent;
        }
    }
}
