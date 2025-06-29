// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.Text
{
    public readonly record struct TextPosition
    {
        public TextPosition(int line, int character)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(line, nameof(line));
            ArgumentOutOfRangeException.ThrowIfNegative(character, nameof(character));

            this.Line = line;
            this.Character = character;
        }

        public TextPosition((int line, int character) input)
            : this(input.line, input.character)
        {
        }

        public int Line { get; }

        public int Character { get; }

        public static TextPosition? TryParse(string? value)
        {
            // Current format: "[line:character]"
            // Future format: "[line,character]"
            // The current format must be supported for backwards compatibility.
            if (string.IsNullOrEmpty(value) || value.Length < 5 || value[0] != '[' || value[^1] != ']')
            {
                return null;
            }

            var separatorIndex = value.IndexOf(',');

            if (separatorIndex < 2 || separatorIndex >= value.Length - 2)
            {
                // Handle both , and : as separators to allow for future migration.
                separatorIndex = value.IndexOf(':');
            }

            if (separatorIndex < 2 || separatorIndex >= value.Length - 2)
            {
                return null;
            }

            if (int.TryParse(value.AsSpan(1, separatorIndex - 1), out var line) &&
                int.TryParse(value.AsSpan(separatorIndex + 1, value.Length - separatorIndex - 2), out var character))
            {
                return new TextPosition(line, character);
            }

            return null;
        }

        public override string ToString() => $"[{this.Line}:{this.Character}]";
    }
}
