// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class UnionTypeSyntax : TypeSyntax
{
    public UnionTypeSyntax(IEnumerable<SyntaxBase> children)
    {
        Children = children.ToImmutableArray();
    }

    public IEnumerable<SyntaxBase> Children { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitUnionTypeSyntax(this);

    public override TextSpan Span => TextSpan.Between(Children.First(), Children.Last());

    public IEnumerable<UnionTypeMemberSyntax> Members => Children.OfType<UnionTypeMemberSyntax>();
}
