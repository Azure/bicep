// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Newtonsoft.Json;

namespace Bicep.Core.UnitTests.Serialization
{
    [method: JsonConstructor]
    public class TokenItem(TokenType type, string text, TextSpan span)
    {
        public TokenItem(Token token) : this(token.Type, token.Text, token.Span)
        {
        }

        public TokenType? Type { get; set; } = type;

        public string? Text { get; set; } = text;

        [JsonConverter(typeof(TextSpanConverter))]
        public TextSpan? Span { get; set; } = span;
    }
}
