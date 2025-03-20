// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Bicep.Core.Text
{
    [JsonConverter(typeof(TextPositionConverter))]
    public readonly record struct TextPosition
    {
        public TextPosition(int line, int character)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(line);
            ArgumentOutOfRangeException.ThrowIfNegative(character);

            this.Line = line;
            this.Character = character;
        }

        public int Line { get; }

        public int Character { get; }

        public TextPosition((int line, int character) position)
            : this(position.line, position.character)
        { }

        public static implicit operator (int line, int character)(TextPosition position) => (position.Line, position.Character);

        public static implicit operator TextPosition((int line, int character) position) => new(position.line, position.character);

        public void Deconstruct(out int line, out int character) => (line, character) = (this.Line, this.Character);

        public static bool TryParse(string? value, [NotNullWhen(true)] out TextPosition? textPosition)
        {
            // Format: "[line:character]"
            if (string.IsNullOrEmpty(value) || value.Length < 5 || value[0] != '[' || value[^1] != ']')
            {
                textPosition = null;
                return false;
            }

            var colonIndex = value.IndexOf(':');
            if (colonIndex < 2 || colonIndex >= value.Length - 2)
            {
                textPosition = null;
                return false;
            }

            if (int.TryParse(value.AsSpan(1, colonIndex - 1), out var line) &&
                int.TryParse(value.AsSpan(colonIndex + 1, value.Length - colonIndex - 2), out var character))
            {
                textPosition = new TextPosition(line, character);
                return true;
            }

            textPosition = null;
            return false;
        }

        public override string ToString() => $"[{this.Line}:{this.Character}]";

        public class TextPositionConverter : JsonConverter<TextPosition>
        {
            public override TextPosition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var s = reader.GetString();
                if (s is { } && TextPosition.TryParse(s, out TextPosition? sourceCodePosition))
                {
                    return sourceCodePosition.Value;
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
}
