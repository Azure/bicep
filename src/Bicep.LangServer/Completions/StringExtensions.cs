// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Class that implements string extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Finds all indices of a given string ignoring case, skipping overlapping values.
        /// </summary>
        /// <param name="str">The string we are searching in.</param>
        /// <param name="value">The value we are finding indices for.</param>
        /// <returns>Enumeration of indices of the given string.</returns>
        public static IEnumerable<long> IndexesOfInsensitive(this string str, string value)
        {
            if (str == null)
            {
                yield break;
            }

            int start = 0;
            while (start < value.Length)
            {
                var index = str.IndexOf(value, start++, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    yield return index;
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Return the index of the first occurrence of value found in the string
        /// using case insensitive match.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>Index of the first match or -1 if no match is found.</returns>
        public static int IndexOfInsensitive(this string str, string value)
        {
            return str?.IndexOf(value, StringComparison.OrdinalIgnoreCase) ?? -1;
        }

        /// <summary>
        /// Finds the first occurrence of value in the string and returns
        /// the start and end columns of the match.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>Start and end columns of the match or (-1, -1) if not found.</returns>
        public static (int Start, int End) SpanOf(this string str, string value)
        {
            if (str == null || value == null)
            {
                return (-1, -1);
            }

            var startCol = str.IndexOf(value, StringComparison.OrdinalIgnoreCase);
            var endCol = startCol >= 0 ? startCol + value.Length : -1;
            return (startCol, endCol);
        }

        /// <summary>
        /// Shorten the string to target length by replacing middle with ...
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="targetLength">The target length.</param>
        /// <param name="clipLeft">Indicates extra length should be clipped from the left instead of the middle.</param>
        /// <returns>The short form of the string.</returns>
        public static string Tiny(this string str, int targetLength = 40, bool clipLeft = true)
        {
            if (str == null)
            {
                return string.Empty;
            }

            if (str.Length < targetLength)
            {
                return str;
            }

            int leftLen = 0;
            int rightStart = targetLength - 3;
            if (!clipLeft)
            {
                leftLen = (targetLength - 2) / 2;
                rightStart = str.Length - leftLen;
            }

            var left = str.Substring(0, leftLen);
            var right = str.Substring(rightStart);
            return $"{left}...{right}";
        }

        /// <summary>
        /// Return the offset of the first character matching the given predicate, or -1 if there is none.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The index.</returns>
        public static int First(this string str, Func<char, bool> predicate)
        {
            if (predicate != null)
            {
                foreach (var pair in str.Select((c, i) => (c, i)))
                {
                    if (predicate(pair.c))
                    {
                        return pair.i;
                    }
                }
            }

            return -1;
        }
    }
}
