// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class ObjectTypePropertySyntax : DecorableSyntax
{
    public ObjectTypePropertySyntax(IEnumerable<SyntaxBase> leadingNodes, SyntaxBase key, SyntaxBase colon, SyntaxBase value) : base(leadingNodes)
    {
        AssertSyntaxType(key, nameof(key), typeof(IdentifierSyntax), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
        AssertSyntaxType(colon, nameof(colon), typeof(Token), typeof(SkippedTriviaSyntax));
        AssertTokenType(colon as Token, nameof(colon), TokenType.Colon);

        Key = key;
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

    public SyntaxBase Colon { get; }

    public SyntaxBase Value { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitObjectTypePropertySyntax(this);

    public override TextSpan Span => TextSpan.Between(LeadingNodes.FirstOrDefault() ?? Key, Value);
}
