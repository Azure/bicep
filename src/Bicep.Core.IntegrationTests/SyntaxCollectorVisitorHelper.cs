// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.IntegrationTests
{
    public class SyntaxCollectorVisitorHelper
    {
        public class SyntaxCollectorVisitor : SyntaxVisitor
        {
            public record SyntaxItem(SyntaxBase Syntax, SyntaxBase? Parent, int Depth);

            private readonly IList<SyntaxItem> syntaxList = new List<SyntaxItem>();
            private SyntaxBase? parent = null;
            private int depth = 0;

            private SyntaxCollectorVisitor()
            {
            }

            public static ImmutableArray<SyntaxItem> Build(ProgramSyntax syntax)
            {
                var visitor = new SyntaxCollectorVisitor();
                visitor.Visit(syntax);

                return visitor.syntaxList.ToImmutableArray();
            }

            protected override void VisitInternal(SyntaxBase syntax)
            {
                syntaxList.Add(new(Syntax: syntax, Parent: parent, Depth: depth));

                var prevParent = parent;
                parent = syntax;
                depth++;
                base.VisitInternal(syntax);
                depth--;
                parent = prevParent;
            }
        }
    }
}
