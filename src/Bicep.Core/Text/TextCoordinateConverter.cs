// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.Text
{
    public static class TextCoordinateConverter
    {
        public static ImmutableArray<int> GetLineStarts(string text)
        {
            var lineStarts = new List<int> { 0 };

            for (int i = 0; i < text.Length; i++)
            {
                char character = text[i];

                if (character == '\r')
                {
                    if (i < text.Length - 1 && text[i + 1] == '\n')
                    {
                        continue;
                    }

                    lineStarts.Add(i + 1);
                }

                if (text[i] == '\n')
                {
                    lineStarts.Add(i + 1);
                }
            }

            return [.. lineStarts];
        }

        public static (int line, int character) GetPosition(IReadOnlyList<int> lineStarts, int offset)
        {
            if (lineStarts.Count == 0)
            {
                throw new ArgumentException($"{nameof(lineStarts)} must not be empty.");
            }

            if (lineStarts[0] != 0)
            {
                throw new ArgumentException($"The first element of {nameof(lineStarts)} must be 0, but got {lineStarts[0]}.");
            }

            if (offset < 0)
            {
                throw new ArgumentException($"{nameof(offset)} must not be a negative number.");
            }

            int line = BinarySearch(lineStarts, offset);

            if (line < 0)
            {
                // If the actual line start was not found,
                // the binary search returns the 2's-complement of the next line start, so subtracting 1.
                line = ~line - 1;
            }

            return (line, offset - lineStarts[line]);
        }

        public static int GetOffset(IReadOnlyList<int> lineStarts, int line, int character)
        {
            if (line < 0 || line >= lineStarts.Count)
            {
                throw new ArgumentException("The specified line number is not valid.");
            }

            return lineStarts[line] + character;
        }

        public static TextSpan GetLineSpan(IReadOnlyList<int> lineStarts, int programLength, int line)
        {
            int lineStart = GetOffset(lineStarts, line, 0);
            if (line == lineStarts.Count - 1)
            {
                return new TextSpan(lineStart, programLength - lineStart);
            }
            else
            {
                int nextLineStart = GetOffset(lineStarts, line + 1, 0);
                return new TextSpan(lineStart, nextLineStart - lineStart);
            }
        }

        // If the actual line start was not found, returns the 2's-complement of the next line start
        private static int BinarySearch(IReadOnlyList<int> values, int target)
        {
            int start = 0;
            int end = values.Count - 1;

            while (start <= end)
            {
                int mid = start + (end - start) / 2;

                if (values[mid] == target)
                {
                    return mid;
                }
                else if (values[mid] < target)
                {
                    start = mid + 1;
                }
                else
                {
                    end = mid - 1;
                }
            }

            return ~start;
        }
    }
}

