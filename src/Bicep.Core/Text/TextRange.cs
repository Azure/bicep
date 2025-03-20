// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.Text
{
    [JsonConverter(typeof(TextRangeConverter))]
    public readonly record struct TextRange(TextPosition Start, TextPosition End)
    {
        public TextRange(int startLine, int startCharacter, int endLine, int endCharacter)
            : this(new(startLine, startCharacter), new(endLine, endCharacter))
        {
        }

        public static bool TryParse(string? value, [NotNullWhen(true)] out TextRange? textRange)
        {
            // Format: "[x1,y1]-[x2,y2]"
            // where
            //   x1: start line
            //   x2: start character
            //   y1: end line
            //   y2: end character
            if (string.IsNullOrEmpty(value))
            {
                textRange = null;
                return false;
            }

            var parts = value.Split('-');

            if (parts.Length == 2 &&
                TextPosition.TryParse(parts[0], out var start) &&
                TextPosition.TryParse(parts[1], out var end))
            {
                textRange = new TextRange(start.Value, end.Value);
                return true;
            }

            textRange = null;
            return false;
        }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }
    }

    public class TextRangeConverter : JsonConverter<TextRange>
    {
        public override TextRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (TextRange.TryParse(value, out TextRange? textRange))
            {
                return textRange.Value;
            }

            throw new ArgumentException($"Invalid input format for deserialization of {nameof(textRange)}");
        }

        public override void Write(Utf8JsonWriter writer, TextRange value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}
