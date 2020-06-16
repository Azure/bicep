﻿using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Utils
{
    public static class PositionHelper
    {
        public static IReadOnlyList<int> GetLineStarts(string contents)
        {
            var lineStarts = new List<int>() { 0 };

            for (int i = 0; i < contents.Length; i++)
            {
                char character = contents[i];

                if (character == '\r')
                {
                    if (i < contents.Length - 1 && contents[i + 1] == '\n')
                    {
                        continue;
                    }

                    lineStarts.Add(i + 1);
                }

                if (contents[i] == '\n')
                {
                    lineStarts.Add(i + 1);
                }
            }

            return lineStarts;
        }

        public static Position GetPosition(IReadOnlyList<int> lineStarts, int offset)
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
                throw new ArgumentOutOfRangeException($"{nameof(offset)} must not be a negative number.");
            }

            int line = BinarySearch(lineStarts, offset);

            if (line < 0)
            {
                // If the actual line start was not found,
                // the binary search returns the 2's-complement of the next line start, so substracting 1.
                line = ~line - 1;
            }

            return new Position(line, offset - lineStarts[line]);
        }

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
                } else
                {
                    end = mid - 1;
                }
            }

            return ~start;
        }
    }
}
