// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.SourceCode
{
    [JsonConverter(typeof(SourceCodeRangeConverter))]
    public record SourceCodeRange(SourceCodePosition Start, SourceCodePosition End)
    {
        public SourceCodeRange(int startLine, int startColumn, int endLine, int endColumn)
            : this(new SourceCodePosition(startLine, startColumn), new SourceCodePosition(endLine, endColumn))
        { }

        public static bool TryParse(string s, [NotNullWhen(true)] out SourceCodeRange? sourceCodeRange)
        {
            // Format: "[x1,y1]-[x2,y2]"
            // where x1,x2,y1,y2 are non-negative integers

            var parts = s?.TrimStart('"').TrimStart('[').TrimEnd(']').TrimEnd('"').Split("-");
            if (parts?.Length == 2 && SourceCodePosition.TryParse(parts[0], out var start) && SourceCodePosition.TryParse(parts[1], out var end))
            {
                sourceCodeRange = new SourceCodeRange(start, end);
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

    public class SourceCodeRangeConverter : JsonConverter<SourceCodeRange>
    {
        public override SourceCodeRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (s is { } && SourceCodeRange.TryParse(s, out SourceCodeRange? sourceCodeRange))
            {
                return sourceCodeRange;
            }
            else
            {
                throw new ArgumentException($"Invalid input format for deserialization of {nameof(sourceCodeRange)}");
            }
        }

        public override void Write(Utf8JsonWriter writer, SourceCodeRange value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}
