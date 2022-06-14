// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Text;
using System.Collections.Immutable;
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

        public static string AddDiagsToSourceText<T>(string bicepOutput, string newlineSequence, IEnumerable<T> items, Func<T, TextSpan> getSpanFunc, Func<T, string> diagsFunc)
        {
            var lineStarts = TextCoordinateConverter.GetLineStarts(bicepOutput);

            var itemsByLine = items
                .Select(item =>
                {
                    var (line, character) = TextCoordinateConverter.GetPosition(lineStarts, getSpanFunc(item).Position);
                    return (line, character, item);
                })
                .ToLookup(t => t.line);

            var sourceTextLines = bicepOutput.Split(newlineSequence);
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < sourceTextLines.Length; i++)
            {
                stringBuilder.Append(sourceTextLines[i]);
                stringBuilder.Append(newlineSequence);
                foreach (var (line, character, item) in itemsByLine[i])
                {
                    var escapedDiagsText = EscapeWhitespace(diagsFunc(item));
                    stringBuilder.Append($"//@[{character}:{character + getSpanFunc(item).Length}) {escapedDiagsText}");
                    stringBuilder.Append(newlineSequence);
                }
            }

            return stringBuilder.ToString();
        }

        public static string AddDiagsToSourceText<TPositionable>(string bicepOutput, string newlineSequence, IEnumerable<TPositionable> items, Func<TPositionable, string> diagsFunc)
            where TPositionable : IPositionable
            => AddDiagsToSourceText(bicepOutput, newlineSequence, items, item => item.Span, diagsFunc);

        public static string AddSourceMapToSourceText(string bicepOutput, string jsonOutput, ImmutableDictionary<int, (string, int)> sourceMap, string newlineSequence)
        {
            var sourceTextLines = bicepOutput.Split(newlineSequence);
            var mappingsStartLines = new int[sourceTextLines.Length];
            var mappingsEndLines = new int[sourceTextLines.Length];
            Array.Fill(mappingsStartLines, int.MaxValue);
            Array.Fill(mappingsEndLines, 0);

            // traverse the source map to determine the JSON line range of each mapped bicep line
            sourceMap.ForEach(kvp =>
            {
                int armLine = kvp.Key;
                (string bicepFile, int bicepLine) = kvp.Value;

                // convert line numbers from 1-indexing to 0-indexing
                armLine--;
                bicepLine--;

                if (armLine < mappingsStartLines[bicepLine])
                {
                    mappingsStartLines[bicepLine] = armLine;
                }

                if (armLine > mappingsEndLines[bicepLine])
                {
                    mappingsEndLines[bicepLine] = armLine;
                }
            });

            var sourceTextWithSourceMap = new StringBuilder();
            var compiledJsonLines = jsonOutput.Split(newlineSequence);
            // "Content" is a line that contains word character that is not part of escape sequence
            var HasContentRegex = new Regex("(?<!\\\\)\\w", RegexOptions.Compiled);

            for (var i = 0; i < sourceTextLines.Length; i++)
            {
                sourceTextWithSourceMap.Append(sourceTextLines[i]);
                sourceTextWithSourceMap.Append(newlineSequence);

                // only annotate lines that have both a mapping and content
                if (mappingsEndLines[i] != 0 && HasContentRegex.IsMatch(sourceTextLines[i]))
                {
                    // show first line of mapped JSON with content in annotation
                    var jsonLine = string.Empty;
                    var jsonStartLine = mappingsStartLines[i];
                    var jsonEndLine = mappingsEndLines[i];
                    var offset = 0;
                    var maxOffset = jsonEndLine - jsonStartLine;

                    do
                    {
                        jsonLine = OutputHelper.EscapeWhitespace(compiledJsonLines[jsonStartLine + offset]);
                        offset++;
                    }
                    while (!HasContentRegex.IsMatch(jsonLine) && offset <= maxOffset);

                    if (jsonLine != string.Empty)
                    {
                        sourceTextWithSourceMap.Append($"//@[{jsonStartLine}:{jsonEndLine}] {jsonLine}");
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
            message = Regex.Replace(message, @"'\${TEST_OUTPUT_DIR}.*?'", new MatchEvaluator((match) => match.Value.Replace('\\', '/')));
        }

        var docLink = diagnostic.Uri == null
            ? "none"
            : $"{diagnostic.Source}({diagnostic.Uri.AbsoluteUri})";


        return $"[{diagnostic.Code} ({diagnostic.Level})] {message} (CodeDescription: {docLink}) |{spanText}|";
    }
}
}
