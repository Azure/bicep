using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bicep.Core.IntegrationTests.Extensons;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Json;
using Bicep.Core.UnitTests.Serialization;
using Bicep.Core.UnitTests.Utils;
using DiffPlex.DiffBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    public static class OutputHelper
    {
        public static string AddDiagsToSourceText<T>(string sourceText, IEnumerable<T> items, Func<T, TextSpan> getSpanFunc, Func<T, string> diagsFunc)
        {
            var lineStarts = TextCoordinateConverter.GetLineStarts(sourceText);

            var orderedItems = items.OrderBy(t => getSpanFunc(t).Position).ThenBy(t => getSpanFunc(t).Length);
            var itemsByLine = orderedItems
                .Select(item => {
                    var (line, character) = TextCoordinateConverter.GetPosition(lineStarts, getSpanFunc(item).Position);
                    return (line, character, item);
                })
                .ToLookup(t => t.line);

            var sourceTextLines = sourceText.Replace("\r\n", "\n").Split("\n");
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < sourceTextLines.Length; i++)
            {
                stringBuilder.AppendLine(sourceTextLines[i]);
                foreach (var (line, character, item) in itemsByLine[i])
                {
                    var escapedDiagsText = diagsFunc(item).Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
                    stringBuilder.AppendLine($"//@[{character}:{character + getSpanFunc(item).Length}] {escapedDiagsText}");
                }
            }

            return stringBuilder.ToString();
        }

        public static string AddDiagsToSourceText<TPositionable>(string sourceText, IEnumerable<TPositionable> items, Func<TPositionable, string> diagsFunc)
            where TPositionable : IPositionable
            => AddDiagsToSourceText(sourceText, items, item => item.Span, diagsFunc);
    }
}
