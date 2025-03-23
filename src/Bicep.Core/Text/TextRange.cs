// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.Text
{
    [JsonConverter(typeof(SourceCodeRangeConverter))]
    public readonly record struct TextRange(TextPosition Start, TextPosition End)
    {
        public TextRange(int startLine, int startColumn, int endLine, int endColumn)
            : this(new TextPosition(startLine, startColumn), new TextPosition(endLine, endColumn))
        {
        }

        public static TextRange? TryParse(string? value)
        {
            // Format: "[x1:y1]-[x2:y2]" ("[x1,y1]-[x2,y2]" makes more sense...but it's expensive to migrate)
            // where
            //   x1: start line
            //   x2: start character
            //   y1: end line
            //   y2: end character
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var parts = value.Split('-');

            if (parts.Length == 2 &&
                TextPosition.TryParse(parts[0]) is { } start &&
                TextPosition.TryParse(parts[1]) is { } end)
            {
                return new TextRange(start, end);
            }

            return null;
        }

        public override string ToString() => $"{this.Start}-{this.End}";
    }

    public class SourceCodeRangeConverter : JsonConverter<TextRange>
    {
        public override TextRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (TextRange.TryParse(reader.GetString()) is { } textRange)
            {
                return textRange;
            }

            throw new InvalidOperationException($"Invalid input format for deserialization of {nameof(textRange)}");
        }

        public override void Write(Utf8JsonWriter writer, TextRange value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}
