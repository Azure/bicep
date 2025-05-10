// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public abstract class ParameterizedTypeInstantiationSyntaxBase : TypeSyntax, ISymbolReference
{
    public ParameterizedTypeInstantiationSyntaxBase(IdentifierSyntax name, Token openChevron, IEnumerable<SyntaxBase> children, SyntaxBase closeChevron)
    {
        AssertTokenType(openChevron, nameof(openChevron), TokenType.LeftChevron);
        AssertSyntaxType(closeChevron, nameof(closeChevron), typeof(Token), typeof(SkippedTriviaSyntax));
        AssertTokenType(closeChevron as Token, nameof(closeChevron), TokenType.RightChevron);

        this.Name = name;
        this.OpenChevron = openChevron;
        this.Children = [.. children];
        this.CloseChevron = closeChevron;
        this.Arguments = [.. this.Children.OfType<ParameterizedTypeArgumentSyntax>()];
    }

    public IdentifierSyntax Name { get; }

    public Token OpenChevron { get; }

    public ImmutableArray<SyntaxBase> Children { get; }

    public ImmutableArray<ParameterizedTypeArgumentSyntax> Arguments { get; }

    public SyntaxBase CloseChevron { get; }

    public ParameterizedTypeArgumentSyntax GetArgumentByPosition(int index) => Arguments[index];

    public override TextSpan Span => TextSpan.Between(Name, CloseChevron);
}
