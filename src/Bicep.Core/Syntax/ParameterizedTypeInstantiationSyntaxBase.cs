// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public abstract class ParameterizedTypeInstantiationSyntaxBase : TypeSyntax, ISymbolReference
{
    public ParameterizedTypeInstantiationSyntaxBase(IdentifierSyntax name, Token openChevron, IEnumerable<SyntaxBase> children, Token closeChevron)
    {
        AssertTokenType(openChevron, nameof(openChevron), TokenType.LeftChevron);
        AssertTokenType(closeChevron, nameof(closeChevron), TokenType.RightChevron);

        this.Name = name;
        this.OpenChevron = openChevron;
        this.Children = children.ToImmutableArray();
        this.CloseChevron = closeChevron;
        this.Arguments = this.Children.OfType<ParameterizedTypeArgumentSyntax>().ToImmutableArray();
    }

    public IdentifierSyntax Name { get; }

    public Token OpenChevron { get; }

    public ImmutableArray<SyntaxBase> Children { get; }

    public ImmutableArray<ParameterizedTypeArgumentSyntax> Arguments { get; }

    public Token CloseChevron { get; }

    public ParameterizedTypeArgumentSyntax GetArgumentByPosition(int index) => Arguments[index];

    public override TextSpan Span => TextSpan.Between(Name, CloseChevron);
}
