// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class ObjectTypeAdditionalPropertiesSyntax : DecorableSyntax
{
    public ObjectTypeAdditionalPropertiesSyntax(IEnumerable<SyntaxBase> leadingNodes, SyntaxBase asterisk, SyntaxBase colon, SyntaxBase value) : base(leadingNodes)
    {
        AssertSyntaxType(asterisk, nameof(asterisk), typeof(Token), typeof(SkippedTriviaSyntax));
        AssertTokenType(asterisk as Token, nameof(asterisk), TokenType.Asterisk);
        AssertSyntaxType(colon, nameof(colon), typeof(Token), typeof(SkippedTriviaSyntax));
        AssertTokenType(colon as Token, nameof(colon), TokenType.Colon);

        Asterisk = asterisk;
        Colon = colon;
        Value = value;
    }

    public SyntaxBase Asterisk { get; }

    public SyntaxBase Colon { get; }

    public SyntaxBase Value { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitObjectTypeAdditionalPropertiesSyntax(this);

    public override TextSpan Span => TextSpan.Between(LeadingNodes.FirstOrDefault() ?? Asterisk, Value);
}
