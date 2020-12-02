// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Newtonsoft.Json;

namespace Bicep.Core.UnitTests.Serialization
{
    public class TokenItem
    {
        [JsonConstructor]
        public TokenItem(TokenType type, string text, TextSpan span)
        {
            this.Type = type;
            this.Text = text;
            this.Span = span;
        }

        public TokenItem(Token token) : this(token.Type, token.Text, token.Span)
        {
        }

        public TokenType? Type { get; set; }

        public string? Text { get; set; }

        [JsonConverter(typeof(TextSpanConverter))]
        public TextSpan? Span { get; set; }
    }
}
