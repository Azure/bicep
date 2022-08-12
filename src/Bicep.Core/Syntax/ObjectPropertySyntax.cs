// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ObjectPropertySyntax : ExpressionSyntax
    {
        public ObjectPropertySyntax(SyntaxBase key, SyntaxBase colon, SyntaxBase value)
        {
            AssertSyntaxType(key, nameof(key), typeof(IdentifierSyntax), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
            AssertSyntaxType(colon, nameof(colon), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(colon as Token, nameof(colon), TokenType.Colon);

            this.Key = key;
            this.Colon = colon;
            this.Value = value;
        }

        public string? TryGetKeyText()
            => Key switch
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

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitObjectPropertySyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Key, this.Value);
    }
}
