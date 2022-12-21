// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.UnitTests.Utils
{
    public static class StringTestUtils
    {
        private static Regex LineIndentRegex { get; } = new Regex("^\\s*", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// Shifts all lines to the left by removing whitespace until the line with the least amount of indent is
        /// flush left (finds the minimum indent of any line and removes that amount of indent from all lines)
        /// </summary>
        public static string Unindent(string s)
        {
            s = StringUtils.ReplaceNewlines(s, "\n");
            var lines = s.Split('\n');
            var nonEmptyLines = lines.Where(l => !string.IsNullOrWhiteSpace(l));
            int minIndent = nonEmptyLines.Any() ? nonEmptyLines.Min(l => LineIndentRegex.Match(l).Value.Length) : 0;
            var minIndentationRegex = new Regex($"^\\s{{{minIndent}}}");
            var unindentedLines = lines.Select(l => minIndentationRegex.Replace(l, "")).ToArray();
            return string.Join("\n", unindentedLines);
        }

        /// <summary>
        /// Removes whitespace from the beginning and ending of all lines
        /// </summary>
        public static string TrimAllLines(string s)
        {
            s = StringUtils.ReplaceNewlines(s, "\n");
            var lines = s.Split('\n');
            return string.Join("\n", lines.Select(l => l.Trim()));
        }
    }
}
