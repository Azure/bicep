// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class MissingDeclarationSyntax : StatementSyntax
    {
        public MissingDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes)
            : base(leadingNodes)
        {
            Assert(leadingNodes.Any(), "Expect at least one leading node.");
        }

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes[0], this.LeadingNodes[^1]);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitMissingDeclarationSyntax(this);
    }
}
