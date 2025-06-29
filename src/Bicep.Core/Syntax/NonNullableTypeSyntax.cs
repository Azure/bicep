// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class NonNullableTypeSyntax : TypeSyntax
{
    public NonNullableTypeSyntax(SyntaxBase @base, Token nonNullabilityMarker)
    {
        AssertTokenType(nonNullabilityMarker, nameof(nonNullabilityMarker), TokenType.Exclamation);

        Base = @base;
        NonNullabilityMarker = nonNullabilityMarker;
    }

    public SyntaxBase Base { get; }

    public Token NonNullabilityMarker { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitNonNullableTypeSyntax(this);

    public override TextSpan Span => TextSpan.Between(Base, NonNullabilityMarker);
}
