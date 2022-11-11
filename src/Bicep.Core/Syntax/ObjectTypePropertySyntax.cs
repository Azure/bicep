// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class ObjectTypePropertySyntax : DecorableSyntax
{
    public ObjectTypePropertySyntax(IEnumerable<SyntaxBase> leadingNodes, SyntaxBase key, SyntaxBase? optionalityMarker, SyntaxBase colon, SyntaxBase value) : base(leadingNodes)
    {
        AssertSyntaxType(key, nameof(key), typeof(IdentifierSyntax), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
        AssertSyntaxType(optionalityMarker, nameof(optionalityMarker), typeof(Token));
        AssertTokenType(optionalityMarker as Token, nameof(optionalityMarker), TokenType.Question);
        AssertSyntaxType(colon, nameof(colon), typeof(Token), typeof(SkippedTriviaSyntax));
        AssertTokenType(colon as Token, nameof(colon), TokenType.Colon);

        Key = key;
        OptionalityMarker = optionalityMarker;
        Colon = colon;
        Value = value;
    }

    public string? TryGetKeyText() => Key switch
    {
        IdentifierSyntax identifier => identifier.IdentifierName,
        StringSyntax @string => @string.TryGetLiteralValue(),
        SkippedTriviaSyntax _ => null,
        // this should not be possible as we assert the type in the constructor
        _ => throw new InvalidOperationException($"Unexpected key syntax {Key.GetType()}"),
    };

    public SyntaxBase Key { get; }

    public SyntaxBase? OptionalityMarker { get; }

    public SyntaxBase Colon { get; }

    public SyntaxBase Value { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitObjectTypePropertySyntax(this);

    public override TextSpan Span => TextSpan.Between(LeadingNodes.FirstOrDefault() ?? Key, Value);
}
