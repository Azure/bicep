// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Emit
{
    public static class SourceMapHelper
    {
        private static readonly Regex JsonWhitespaceStrippingRegex = new(@"(""(?:[^""\\]|\\.)*"")|\s+", RegexOptions.Compiled);

        public static void AddMapping(
            Dictionary<string, Dictionary<IPositionable, IList<(int start, int end)>>> rawSourceMap,
            BicepFile bicepFile,
            SyntaxBase bicepSyntax,
            PositionTrackingJsonTextWriter jsonWriter,
            int jsonStartPos)
        {
            if (rawSourceMap is null)
            {
                throw new ArgumentNullException(nameof(rawSourceMap));
            }

            var bicepFilePath = bicepFile.FileUri.AbsolutePath;
            IPositionable bicepLocation = bicepSyntax;

            // account for leading nodes (decorators)
            if (bicepSyntax is StatementSyntax syntax)
            {
                var lastLeadingNode = syntax.LeadingNodes.LastOrDefault();
                if (lastLeadingNode is not null)
                {
                    bicepLocation = new TextSpan(
                        lastLeadingNode.Span.Position + lastLeadingNode.Span.Length,
                        syntax.Span.Length - lastLeadingNode.Span.Length);
                }
            }

            // increment start position if starting on a comma (happens when outputting successive items in objects and arrays)
            if (jsonWriter.CommaPositions.Contains(jsonStartPos))
            {
                jsonStartPos++;
            }

            var jsonEndPos = jsonWriter.CurrentPos - 1;

            rawSourceMap.AddMapping(
                bicepFilePath,
                bicepLocation,
                jsonStartPos,
                jsonEndPos);
        }

        public static void AddNestedSourceMap(
            this Dictionary<string, Dictionary<IPositionable, IList<(int start, int end)>>> parentSourceMap,
            Dictionary<string, Dictionary<IPositionable, IList<(int start, int end)>>> nestedSourceMap,
            Func<string,string> toRelativePath,
            int offset)
        {
            foreach (var fileKvp in nestedSourceMap)
            {
                foreach (var lineKvp in fileKvp.Value)
                {
                    foreach (var (start, end) in lineKvp.Value)
                    {
                        parentSourceMap.AddMapping(
                            toRelativePath(fileKvp.Key),
                            lineKvp.Key,
                            start + offset,
                            end + offset);
                    }
                }
            }
        }

        public static void ProcessRawSourceMap(
            Dictionary<string, Dictionary<IPositionable, IList<(int start, int end)>>> rawSourceMap,
            JToken rawTemplate,
            BicepFile sourceFile,
            IDictionary<int,(string,int)> sourceMap)
        {
            var formattedTemplateLines = rawTemplate
                .ToString(Formatting.Indented)
                .Split(Environment.NewLine, StringSplitOptions.None);

            // get line starts of unformatted JSON by stripping formatting from each line of formatted JSON
            var unformattedLineStarts = formattedTemplateLines
                .Aggregate(
                    new List<int>() { 0 }, // first line starts at position 0
                    (lineStarts, line) =>
                    {
                        var unformattedLine = JsonWhitespaceStrippingRegex.Replace(line, "$1");
                        lineStarts.Add(lineStarts.Last() + unformattedLine.Length);
                        return lineStarts;
                    });

            // TODO remove soon
            // get position and length of template hash (relying on the first occurence)
            (var templateHashStartPosition, var templateHashLength) = formattedTemplateLines
                .Select((value, index) => new { lineNumber = index, lineValue = value })
                .Where(item => item.lineValue.Contains(TemplateWriter.TemplateHashPropertyName))
                .Select(item =>
                {
                    var startPosition = unformattedLineStarts[item.lineNumber];
                    var unformattedLineLength = unformattedLineStarts[item.lineNumber + 1] - startPosition;
                    return (startPosition, unformattedLineLength + 1); // account for comma by adding 1 to length
                })
                .FirstOrDefault();

            // strip full path from main bicep source file
            // TODO: better name for this
            string getRelativeFileName(string file) => (file == sourceFile.FileUri.AbsolutePath) ? Path.GetFileName(file) : file;
            var formattedSourceMap = new Dictionary<string, Dictionary<int, IList<(int start, int end)>>>();

            // unfold key-values in bicep-to-json map to convert to json-to-bicep map
            var weights = new int[unformattedLineStarts.Count];
            Array.Fill(weights, int.MaxValue);

            foreach (var bicepFileName in rawSourceMap.Keys)
            {
                var bicepRelativeFileName = getRelativeFileName(bicepFileName);

                foreach (var bicepPosition in rawSourceMap[bicepFileName].Keys)
                {
                    // add 1 to all line numbers to convert to 1-indexing TODO REMOVE
                    var bicepLine = TextCoordinateConverter.GetPosition(sourceFile.LineStarts, bicepPosition.GetPosition()).line + 1;

                    foreach(var mapping in rawSourceMap[bicepFileName][bicepPosition])
                    {
                        var (jsonStartPos, jsonEndPos) = mapping;

                        // TODO remove once filler hash is added
                        // increment positions by templateHashLength that occur after hash start position
                        if (jsonStartPos >= templateHashStartPosition)
                        {
                            jsonStartPos += templateHashLength;
                            jsonEndPos += templateHashLength;
                        }

                        // add 1 to all line numbers to convert to 1-indexing TODO REMOVE
                        // transform offsets in rawSourceMap to line numbers for formatted JSON using unformattedLineStarts
                        var jsonStartLine = TextCoordinateConverter.GetPosition(unformattedLineStarts, jsonStartPos).line + 1;
                        var jsonEndLine = TextCoordinateConverter.GetPosition(unformattedLineStarts, jsonEndPos).line + 1;

                        // write most specific mapping available for each json line (less lines => stronger weight)
                        int weight = jsonEndLine - jsonStartLine;
                        for (int i = jsonStartLine; i <= jsonEndLine; i++)
                        {
                            // write new mapping if weight is stronger than existing mapping
                            if (weight < weights[i])
                            {
                                sourceMap![i] = (bicepRelativeFileName, bicepLine);
                                weights[i] = weight;
                            }
                        }
                    }
                }
            }
        }

        private static void AddMapping<T>(
            this Dictionary<string, Dictionary<T, IList<(int start, int end)>>> rawSourceMap,
            string bicepFileName,
            T bicepLocation,
            int jsonStartPos,
            int jsonEndPos) where T : notnull
        {
            if (!rawSourceMap.TryGetValue(bicepFileName, out var bicepFileDict))
            {
                rawSourceMap[bicepFileName] = bicepFileDict = new Dictionary<T, IList<(int, int)>>();
            }

            if (!bicepFileDict.TryGetValue(bicepLocation, out var mappingList))
            {
                bicepFileDict[bicepLocation] = mappingList = new List<(int, int)>();
            }

            mappingList.Add((jsonStartPos, jsonEndPos));
        }
    }
}
