// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Parsing;

namespace Bicep.Core.Parsing
{
    public class SlidingTextWindow
    {
        public const char InvalidCharacter = char.MaxValue;

        private readonly string text;

        public SlidingTextWindow(string text)
        {
            this.text = text;
            this.position = 0;
            this.offset = 0;
        }

        /// <summary>
        /// Gets the span of the current window.
        /// </summary>
        public TextSpan GetSpan()
            => new TextSpan(position, offset);

        /// <summary>
        /// Gets the span from n characters behind up to current position.
        /// </summary>
        /// <param name="charCount">Number of characters to look behind. Must be zero or positive.</param>
        public TextSpan GetLookbehindSpan(int charCount = 1)
        {
            if (charCount < 0)
            {
                throw new ArgumentException($"{nameof(charCount)} must be zero or positive.");
            }

            int effectivePosition = GetAbsolutePosition() - charCount;
            if (effectivePosition < 0)
            {
                throw new ArgumentException("Unable to look behind the beginning of the file.");
            }

            return new TextSpan(effectivePosition, charCount);
        }

        public TextSpan GetSpanFromPosition(int referencePosition)
        {
            int currentPosition = GetAbsolutePosition();
            int delta = currentPosition - referencePosition;
            if (delta >= 0)
            {
                return new TextSpan(referencePosition, delta);
            }

            return new TextSpan(currentPosition, -delta);
        }

        /// <summary>
        /// Returns the current absolute position within the text.
        /// </summary>
        public int GetAbsolutePosition() => position + offset;

        public string GetText()
            => text.Substring(position, offset);

        private int position;

        private int offset;

        public char Peek(int numChars = 0)
        {
            if (position + offset + numChars >= text.Length)
            {
                return InvalidCharacter;
            }

            return text[position + offset + numChars];
        }

        public bool IsAtEnd()
            => position + offset >= text.Length;

        public char Next()
        {
            var nextChar = Peek(0);
            if (nextChar != InvalidCharacter)
            {
                Advance(1);
            }

            return nextChar;
        }

        public void Advance(int numChars = 1)
        {
            offset += numChars;
        }

        public void Rewind(int numChars = 1)
        {
            offset -= numChars;
        }

        public void Reset()
        {
            position += offset;
            offset = 0;
        }
    }
}

