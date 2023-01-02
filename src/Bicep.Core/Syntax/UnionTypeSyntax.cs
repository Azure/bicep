// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
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
        if (!Members.Any())
        {
            throw new ArgumentException("Union types must contain at least one member");
        }
    }

    public ImmutableArray<SyntaxBase> Children { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitUnionTypeSyntax(this);

    public override TextSpan Span => TextSpan.Between(Children.First(), Children.Last());

    public IEnumerable<UnionTypeMemberSyntax> Members => Children.OfType<UnionTypeMemberSyntax>();
}
