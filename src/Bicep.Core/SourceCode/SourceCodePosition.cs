// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.SourceCode
{
    [JsonConverter(typeof(SourceCodePositionConverter))]
    public record SourceCodePosition
    {
        private int line, column;

        public int Line
        {
            get => line;
            set
            {
                line = value >= 0 ? value : throw new ArgumentException($"{nameof(Line)} must be non-negative");
            }
        }

        public int Column
        {
            get => column;
            set
            {
                column = value >= 0 ? value : throw new ArgumentException($"{nameof(Column)} must be non-negative");
            }
        }

        public SourceCodePosition(int Line, int Column)
        {
            this.Line = Line;
            this.Column = Column;
        }

        public SourceCodePosition((int line, int column) input)
            : this(input.line, input.column)
        { }

        public static bool TryParse(string s, [NotNullWhen(true)] out SourceCodePosition? sourceCodePosition)
        {
            var parts = s?.TrimStart('"').TrimStart('[').TrimEnd(']').TrimEnd('"').Split(":");
            if (parts?.Length == 2 && int.TryParse(parts[0], out int line) && int.TryParse(parts[1], out int column))
            {
                sourceCodePosition = new SourceCodePosition(line, column);
                return true;
            }
            else
            {
                sourceCodePosition = null;
                return false;
            }
        }

        public override string ToString()
        {
            return $"[{Line}:{Column}]";
        }
    }

    public class SourceCodePositionConverter : JsonConverter<SourceCodePosition>
    {
        public override SourceCodePosition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (s is { } && SourceCodePosition.TryParse(s, out SourceCodePosition? sourceCodePosition))
            {
                return sourceCodePosition;
            }
            else
            {
                throw new ArgumentException($"Invalid input format for deserialization of {nameof(SourceCodePosition)}");
            }
        }

        public override void Write(Utf8JsonWriter writer, SourceCodePosition value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
