// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.Syntax;

public abstract class DecorableSyntax : SyntaxBase
{
    protected DecorableSyntax(IEnumerable<SyntaxBase> leadingNodes)
    {
        this.LeadingNodes = [.. leadingNodes];
    }

    public ImmutableArray<SyntaxBase> LeadingNodes { get; }

    public IEnumerable<DecoratorSyntax> Decorators => this.LeadingNodes.OfType<DecoratorSyntax>();
}
