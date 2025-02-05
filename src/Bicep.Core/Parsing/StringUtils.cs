// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.ComponentModel.Composition;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Parsing
{
    public static partial class StringUtils
    {
        [GeneratedRegex(@"(\r\n|\r|\n)")]
        private static partial Regex NewLineRegex();

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

        public static bool IsPropertyNameEscapingRequired(string propertyName) =>
            !Lexer.IsValidIdentifier(propertyName) || LanguageConstants.NonContextualKeywords.ContainsKey(propertyName);

        public static string EscapeBicepPropertyName(string propertyName)
        {
            return IsPropertyNameEscapingRequired(propertyName)
                ? EscapeBicepString(propertyName)
                : propertyName;
        }

        public static int CountNewlines(string value) => NewLineRegex().Matches(value).Count;

        public static string MatchNewline(string value) => NewLineRegex().Match(value).Value;

        public static string ReplaceNewlines(this string value, string newlineReplacement) =>
            NewLineRegex().Replace(value, newlineReplacement);

        public static string NormalizeNewlines(this string value) =>
            ReplaceNewlines(value, "\n");

        public static IEnumerable<string> SplitOnNewLine(string value) =>
            value.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

        public static string NormalizeIndent(string value)
        {
            var allLines = SplitOnNewLine(value).ToArray();
            var firstLine = allLines[0];
            var linesToNormalize = allLines[1..];

            var commonPrefixLength = 0;
            if (linesToNormalize.Any(x => x.Length > 0))
            {
                var minLength = linesToNormalize.Where(x => x.Length > 0).Select(x => x.Length).Min();
                for (var i = 0; i < minLength; i++)
                {
                    if (linesToNormalize.Any(x => x.Length > 0 && !Lexer.IsWhiteSpace(x[i])))
                    {
                        break;
                    }

                    commonPrefixLength = i + 1;
                }
            }

            return string.Join(
                '\n',
                linesToNormalize
                    .Select(x => x.Length == 0 ? x : x[commonPrefixLength..])
                    .Prepend(firstLine));
        }

        public static string ToCamelCase(string name) => char.ToLowerInvariant(name[0]) + name[1..];
    }
}
