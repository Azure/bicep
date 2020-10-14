// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using Bicep.Core.Text;

namespace Bicep.Core.IntegrationTests
{
    public static class OutputHelper
    {
        public static string EscapeWhitespace(string input)
            => input
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t");

        public static string AddDiagsToSourceText<T>(DataSet dataSet, IEnumerable<T> items, Func<T, TextSpan> getSpanFunc, Func<T, string> diagsFunc)
        {
            var newlineSequence = dataSet.HasCrLfNewlines() ? "\r\n" : "\n";
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);

            var itemsByLine = items
                .Select(item => {
                    var (line, character) = TextCoordinateConverter.GetPosition(lineStarts, getSpanFunc(item).Position);
                    return (line, character, item);
                })
                .ToLookup(t => t.line);

            var sourceTextLines = dataSet.Bicep.Split(newlineSequence);
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

        public static string AddDiagsToSourceText<TPositionable>(DataSet dataSet, IEnumerable<TPositionable> items, Func<TPositionable, string> diagsFunc)
            where TPositionable : IPositionable
            => AddDiagsToSourceText(dataSet, items, item => item.Span, diagsFunc);

        public static string GetSpanText(string sourceText, IPositionable positionable)
        {
            var spanText = sourceText[new Range(positionable.Span.Position, positionable.Span.Position + positionable.Span.Length)];

            return EscapeWhitespace(spanText);
        }

        public static string GetBaselineUpdatePath(DataSet dataSet, string fileName)
            => Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, fileName);
    }
}
