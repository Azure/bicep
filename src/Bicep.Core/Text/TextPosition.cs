// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Bicep.Core.Text
{
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
            // Current format: "[line:character]"
            // Future format: "[line,character]"
            // The current format must be supported for backwards compatibility.
            if (string.IsNullOrEmpty(value) || value.Length < 5 || value[0] != '[' || value[^1] != ']')
            {
                textPosition = null;
                return false;
            }

            var separatorIndex = value.IndexOf(',');

            if (separatorIndex < 2 || separatorIndex >= value.Length - 2)
            {
                // Handle both , and : as separators to allow for future migration.
                separatorIndex = value.IndexOf(':');
            }

            if (separatorIndex < 2 || separatorIndex >= value.Length - 2)
            {
                textPosition = null;
                return false;
            }

            if (int.TryParse(value.AsSpan(1, separatorIndex - 1), out var line) &&
                int.TryParse(value.AsSpan(separatorIndex + 1, value.Length - separatorIndex - 2), out var character))
            {
                textPosition = new TextPosition(line, character);
                return true;
            }

            textPosition = null;
            return false;
        }

        // TODO: Use [line,character] instead of [line:character].
        // The current format must be supported for an extended period before transitioning to the new one.
        public override string ToString() => $"[{this.Line}:{this.Character}]";
    }
}
