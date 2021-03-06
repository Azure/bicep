// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Syntax
{
    public abstract class StatementSyntax : SyntaxBase
    {
        protected StatementSyntax(IEnumerable<SyntaxBase> leadingNodes)
        {
            this.LeadingNodes = leadingNodes.ToImmutableArray();
        }

        public ImmutableArray<SyntaxBase> LeadingNodes { get; }

        public IEnumerable<DecoratorSyntax> Decorators => this.LeadingNodes.OfType<DecoratorSyntax>();
    }
}
