// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
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

        private static Func<int, string> GetPaddingFunc(IEnumerable<int> integers)
        {
            var padding = CountDigits(integers.DefaultIfEmpty(0).Max());

            return x => x.ToInvariantString().PadLeft(padding, '0');
        }

        public static string AddDiagsToSourceText<T>(string bicepOutput, string newlineSequence, IEnumerable<T> items, Func<T, TextSpan> getSpanFunc, Func<T, string> diagsFunc, bool isLinePreformatted = false)
        {
            if (bicepOutput.Equals("\r", StringComparison.Ordinal) ||
                bicepOutput.Equals("\n", StringComparison.Ordinal) ||
                bicepOutput.Equals("\r\n", StringComparison.Ordinal))
            {
                return bicepOutput;
            }

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

            var padStartChar = GetPaddingFunc(diags.Select(x => x.startChar));
            var padEndChar = GetPaddingFunc(diags.Select(x => x.endChar));

            var sourceTextLines = bicepOutput.Split(newlineSequence);
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < sourceTextLines.Length; i++)
            {
                stringBuilder.Append(sourceTextLines[i]);
                stringBuilder.Append(newlineSequence);
                foreach (var diag in diagsByLine[i])
                {
                    // Pad the start & end char with zeros to ensure that the escaped text always starts at the same place
                    // This makes it easier to compare lines visually
                    if (isLinePreformatted)
                    {
                        stringBuilder.Append(diag.escapedText);
                    }
                    else
                    {
                        var startCharPadded = padStartChar(diag.startChar);
                        var endCharPadded = padEndChar(diag.endChar);
                        stringBuilder.Append($"//@[{startCharPadded}:{endCharPadded}) {diag.escapedText}");
                    }
                    stringBuilder.Append(newlineSequence);
                }
            }

            return stringBuilder.ToString();
        }

        public static string AddDiagsToSourceText<TPositionable>(string bicepOutput, string newlineSequence, IEnumerable<TPositionable> items, Func<TPositionable, string> diagsFunc, bool isLinePreformatted = false)
            where TPositionable : IPositionable
            => AddDiagsToSourceText(bicepOutput, newlineSequence, items, item => item.Span, diagsFunc, isLinePreformatted);

        private record SourceMapDiags(TextSpan Span, string JsonLine) : IPositionable;

        public static string AddSourceMapToSourceText(string bicepOutput, string bicepFilePath, string newlineSequence, SourceMap sourceMap, string[] jsonLines)
        {
            // get source map entries for bicep file to annotate
            var fileEntry = sourceMap.Entries.FirstOrDefault(entry => string.Compare(entry.FilePath, bicepFilePath) == 0);
            if (fileEntry is null)
            {
                return bicepOutput;
            }

            var lineStarts = TextCoordinateConverter.GetLineStarts(bicepOutput);
            var sourceMapDiags = fileEntry.SourceMap.Select(entry =>
            {
                var offset = TextCoordinateConverter.GetOffset(lineStarts, entry.SourceLine, 0);
                var jsonLine = jsonLines[entry.TargetLine];

                return new SourceMapDiags(new(offset, 0), jsonLine);
            });

            return AddDiagsToSourceText(
                bicepOutput,
                newlineSequence,
                sourceMapDiags,
                e => $"//@{e.JsonLine}",
                isLinePreformatted: true);
        }

        public static string GetSpanText(string sourceText, IPositionable positionable)
        {
            var spanText = sourceText[new Range(positionable.Span.Position, positionable.GetEndPosition())];

            return EscapeWhitespace(spanText);
        }

        public static string GetDiagLoggingString(string sourceText, string outputDirectory, IDiagnostic diagnostic)
        {
            var spanText = GetSpanText(sourceText, diagnostic);
            var message = NormalizeOutputPath(outputDirectory, diagnostic.Message);
            var source = diagnostic.Source.ToSourceString();

            return $"[{diagnostic.Code} ({diagnostic.Level})] {message} ({source} {diagnostic.Uri}) |{spanText}|";
        }

        public static string NormalizeOutputPath(string outputDirectory, string message)
        {
            message = message.Replace($"{outputDirectory}{Path.DirectorySeparatorChar}", "${TEST_OUTPUT_DIR}/");
            // Normalize file path separators across OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                message = Regex.Replace(message, @"(""|')\${TEST_OUTPUT_DIR}.*?(""|')", new MatchEvaluator((match) => match.Value.Replace('\\', '/')));
            }

            return message;
        }
    }
}
