// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class ParameterizedTypeInstantiationSyntax : TypeSyntax
{
    public ParameterizedTypeInstantiationSyntax(IdentifierSyntax name, Token openChevron, IEnumerable<SyntaxBase> children, Token closeChevron)
    {
        AssertTokenType(openChevron, nameof(openChevron), TokenType.LeftChevron);
        AssertTokenType(closeChevron, nameof(closeChevron), TokenType.RightChevron);

        this.Name = name;
        this.OpenChevron = openChevron;
        this.Children = children.ToImmutableArray();
        this.CloseChevron = closeChevron;
        this.Arguments = children.OfType<ParameterizedTypeArgumentSyntax>().ToImmutableArray();
    }

    public IdentifierSyntax Name { get; }

    public Token OpenChevron { get; }

    public ImmutableArray<SyntaxBase> Children { get; }

    public ImmutableArray<ParameterizedTypeArgumentSyntax> Arguments { get; }

    public Token CloseChevron { get; }

    public ParameterizedTypeArgumentSyntax GetArgumentByPosition(int index) => Arguments[index];

    public override TextSpan Span => TextSpan.Between(Name, CloseChevron);

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitParameterizedTypeInstantiationSyntax(this);
}
