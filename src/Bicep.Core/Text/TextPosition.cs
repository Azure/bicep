// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.Text
{
    [JsonConverter(typeof(SourceCodePositionConverter))]
    public record TextPosition
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

        public TextPosition(int Line, int Column)
        {
            this.Line = Line;
            this.Column = Column;
        }

        public TextPosition((int line, int column) input)
            : this(input.line, input.column)
        { }

        public static bool TryParse(string s, [NotNullWhen(true)] out TextPosition? sourceCodePosition)
        {
            var parts = s?.TrimStart('"').TrimStart('[').TrimEnd(']').TrimEnd('"').Split(":");
            if (parts?.Length == 2 && int.TryParse(parts[0], out int line) && int.TryParse(parts[1], out int column))
            {
                sourceCodePosition = new TextPosition(line, column);
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

    public class SourceCodePositionConverter : JsonConverter<TextPosition>
    {
        public override TextPosition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (s is { } && TextPosition.TryParse(s, out TextPosition? sourceCodePosition))
            {
                return sourceCodePosition;
            }
            else
            {
                throw new ArgumentException($"Invalid input format for deserialization of {nameof(TextPosition)}");
            }
        }

        public override void Write(Utf8JsonWriter writer, TextPosition value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
