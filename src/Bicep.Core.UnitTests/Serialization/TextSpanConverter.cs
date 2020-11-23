// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Parsing;
using Newtonsoft.Json;

namespace Bicep.Core.UnitTests.Serialization
{
    public class TextSpanConverter : JsonConverter<TextSpan>
    {
        public override void WriteJson(JsonWriter writer, TextSpan? value, JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteToken(JsonToken.String, value.ToString());
            }
        }

        public override TextSpan ReadJson(JsonReader reader, Type objectType, TextSpan? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonException($"Expected a {nameof(TextSpan)} string, but encountered a JSON token of type '{reader.TokenType}'.");
            }

            var value = reader.ReadAsString();
            TextSpan.TryParse(value, out var span);
            if (span == null)
            {
                throw new JsonException($"The string '{value}' is not a valid {nameof(TextSpan)}.");
            }

            return span;
        }
    }
}

