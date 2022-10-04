// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Text;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.UnitTests.Utils
{
    public static class OutputHelper
    {
        public static string EscapeWhitespace(string input)
            => input
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t");

        private static int CountDigits(int number)
        {
            if (number == 0)
            {
                return 1;
            }

            var count = 0;
            while (number != 0)
            {
                number /= 10;
                ++count;
            }

            return count;
        }

        public static string AddDiagsToSourceText<T>(string bicepOutput, string newlineSequence, IEnumerable<T> items, Func<T, TextSpan> getSpanFunc, Func<T, string> diagsFunc)
        {
            var lineStarts = TextCoordinateConverter.GetLineStarts(bicepOutput);

            var diagsByLine = items
                .Select(item =>
                {
                    var span = getSpanFunc(item);
                    var (line, startChar) = TextCoordinateConverter.GetPosition(lineStarts, span.Position);
                    var endChar = startChar + span.Length;

                    var escapedText = EscapeWhitespace(diagsFunc(item));

                    return (line, startChar, endChar, escapedText);
                })
                .ToLookup(t => t.line);

            var diags = diagsByLine.SelectMany(x => x);

            var startCharPadding = diags.Any() ? CountDigits(diags.Max(x => x.startChar)) : 0;
            var endCharPadding = diags.Any() ? CountDigits(diags.Max(x => x.endChar)) : 0;

            var sourceTextLines = bicepOutput.Split(newlineSequence);
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < sourceTextLines.Length; i++)
            {
                stringBuilder.Append(sourceTextLines[i]);
                stringBuilder.Append(newlineSequence);
                foreach (var diag in diagsByLine[i])
                {
                    var startCharPadded = diag.startChar.ToString().PadLeft(startCharPadding, '0');
                    var endCharPadded = diag.endChar.ToString().PadLeft(endCharPadding, '0');

                    // Pad the start & end char with zeros to ensure that the escaped text always starts at the same place
                    // This makes it easier to compare lines visually
                    stringBuilder.Append($"//@[{startCharPadded}:{endCharPadded}) {diag.escapedText}");
                    stringBuilder.Append(newlineSequence);
                }
            }

            return stringBuilder.ToString();
        }

        public static string AddDiagsToSourceText<TPositionable>(string bicepOutput, string newlineSequence, IEnumerable<TPositionable> items, Func<TPositionable, string> diagsFunc)
            where TPositionable : IPositionable
            => AddDiagsToSourceText(bicepOutput, newlineSequence, items, item => item.Span, diagsFunc);

        public static string AddSourceMapToSourceText(string bicepOutput, string bicepFilePath, string newlineSequence, SourceMap sourceMap, string[] jsonLines)
        {
            var sourceTextLines = bicepOutput.Split(newlineSequence);
            var mappingsStartLines = new int[sourceTextLines.Length];
            var mappingsEndLines = new int[sourceTextLines.Length];
            Array.Fill(mappingsStartLines, int.MaxValue);
            Array.Fill(mappingsEndLines, 0);

            // get source map entries for bicep file to annotate
            var fileEntry = sourceMap.Entries.FirstOrDefault(entry => string.Compare(entry.FilePath, bicepFilePath) == 0);
            if (fileEntry is null)
            {
                return bicepOutput;
            }

            // traverse the source map to determine the JSON line range of each mapped bicep line
            foreach (var entry in fileEntry.SourceMap)
            {
                int bicepLine = entry.SourceLine;
                int armLine = entry.TargetLine;

                if (armLine < mappingsStartLines[bicepLine])
                {
                    mappingsStartLines[bicepLine] = armLine;
                }

                if (armLine > mappingsEndLines[bicepLine])
                {
                    mappingsEndLines[bicepLine] = armLine;
                }
            }

            var sourceTextWithSourceMap = new StringBuilder();

            for (var i = 0; i < sourceTextLines.Length; i++)
            {
                sourceTextWithSourceMap.Append(sourceTextLines[i]);
                sourceTextWithSourceMap.Append(newlineSequence);

                // only annotate lines that have both a mapping and content
                if (mappingsEndLines[i] != 0 && sourceTextLines[i].Any(char.IsLetterOrDigit))
                {
                    // show first line of mapped JSON with content in annotation
                    var jsonLineText = string.Empty;
                    var jsonStartLine = mappingsStartLines[i];
                    var jsonEndLine = mappingsEndLines[i];
                    var offset = 0;
                    var maxOffset = jsonEndLine - jsonStartLine;

                    do
                    {
                        jsonLineText = OutputHelper.EscapeWhitespace(jsonLines[jsonStartLine + offset]);
                        offset++;
                    }
                    while (!jsonLineText.Any(char.IsLetterOrDigit) && offset <= maxOffset);

                    if (jsonLineText != string.Empty)
                    {
                        sourceTextWithSourceMap.Append($"//@[{jsonStartLine}:{jsonEndLine}] {jsonLineText}");
                        sourceTextWithSourceMap.Append(newlineSequence);
                    }
                }
            }

            return sourceTextWithSourceMap.ToString();
        }

        public static string GetSpanText(string sourceText, IPositionable positionable)
        {
            var spanText = sourceText[new Range(positionable.Span.Position, positionable.GetEndPosition())];

            return EscapeWhitespace(spanText);
        }

        public static string GetDiagLoggingString(string sourceText, string outputDirectory, IDiagnostic diagnostic)
        {
            var spanText = GetSpanText(sourceText, diagnostic);
            var message = diagnostic.Message.Replace($"{outputDirectory}{Path.DirectorySeparatorChar}", "${TEST_OUTPUT_DIR}/");
            // Normalize file path seperators across OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                message = Regex.Replace(message, @"(""|')\${TEST_OUTPUT_DIR}.*?(""|')", new MatchEvaluator((match) => match.Value.Replace('\\', '/')));
            }

            var docLink = diagnostic.Uri == null
                ? "none"
                : $"{diagnostic.Source}({diagnostic.Uri.AbsoluteUri})";


            return $"[{diagnostic.Code} ({diagnostic.Level})] {message} (CodeDescription: {docLink}) |{spanText}|";
        }
    }
}
