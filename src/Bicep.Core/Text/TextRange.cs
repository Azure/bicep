// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.Text
{
    [JsonConverter(typeof(SourceCodeRangeConverter))]
    public record TextRange(TextPosition Start, TextPosition End)
    {
        public TextRange(int startLine, int startColumn, int endLine, int endColumn)
            : this(new TextPosition(startLine, startColumn), new TextPosition(endLine, endColumn))
        { }

        public static bool TryParse(string s, [NotNullWhen(true)] out TextRange? sourceCodeRange)
        {
            // Format: "[x1,y1]-[x2,y2]"
            // where x1,x2,y1,y2 are non-negative integers

            var parts = s?.TrimStart('"').TrimStart('[').TrimEnd(']').TrimEnd('"').Split("-");
            if (parts?.Length == 2 && TextPosition.TryParse(parts[0], out var start) && TextPosition.TryParse(parts[1], out var end))
            {
                sourceCodeRange = new TextRange(start, end);
                return true;
            }

            sourceCodeRange = null;
            return false;
        }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }
    }

    public class SourceCodeRangeConverter : JsonConverter<TextRange>
    {
        public override TextRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (s is { } && TextRange.TryParse(s, out TextRange? sourceCodeRange))
            {
                return sourceCodeRange;
            }
            else
            {
                throw new ArgumentException($"Invalid input format for deserialization of {nameof(sourceCodeRange)}");
            }
        }

        public override void Write(Utf8JsonWriter writer, TextRange value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}
