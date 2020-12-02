// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bicep.Core.Parsing
{
    public static class StringUtils
    {
        private static Regex NewLineRegex { get; } = new Regex("(\r\n|\r|\n)");

        public static string EscapeBicepString(string value)
            => EscapeBicepString(value, "'", "'");

        public static string EscapeBicepString(string value, string startString, string endString)
        {
            var buffer = new StringBuilder(value.Length + 2);

            buffer.Append(startString);
            for (var i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '\\':
                        buffer.Append("\\\\");
                        break;
                    case '\'':
                        buffer.Append("\\'");
                        break;
                    case '\r':
                        buffer.Append("\\r");
                        break;
                    case '\n':
                        buffer.Append("\\n");
                        break;
                    case '\t':
                        buffer.Append("\\t");
                        break;
                    case '$' when value.Length > i + 1 && value[i + 1] == '{':
                        buffer.Append("\\$");
                        break;
                    default:
                        buffer.Append(value[i]);
                        break;
                }
            }
            buffer.Append(endString);

            return buffer.ToString();
        }

        public static int CountNewlines(string value) => NewLineRegex.Matches(value).Count;

        public static string MatchNewline(string value) => NewLineRegex.Match(value).Value;
        
        public static string ReplaceNewlines(string value, string newlineReplacement) =>
            NewLineRegex.Replace(value, newlineReplacement);
    }
}