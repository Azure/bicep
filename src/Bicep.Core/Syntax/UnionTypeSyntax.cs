// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class UnionTypeSyntax : TypeSyntax
{
    public UnionTypeSyntax(IEnumerable<SyntaxBase> children)
    {
        Children = [.. children];
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
