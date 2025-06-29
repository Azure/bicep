// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class NullableTypeSyntax : TypeSyntax
{
    public NullableTypeSyntax(SyntaxBase @base, Token nullabilityMarker)
    {
        AssertTokenType(nullabilityMarker, nameof(nullabilityMarker), TokenType.Question);

        Base = @base;
        NullabilityMarker = nullabilityMarker;
    }

    public SyntaxBase Base { get; }

    public Token NullabilityMarker { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitNullableTypeSyntax(this);

    public override TextSpan Span => TextSpan.Between(Base, NullabilityMarker);
}
