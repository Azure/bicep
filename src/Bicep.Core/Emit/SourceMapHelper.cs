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
            IDictionary<string, IDictionary<int, IList<(int start, int end)>>> rawSourceMap,
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
            (int bicepLine, _) = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, bicepSyntax.GetPosition());

            // account for leading nodes (decorators)
            if (bicepSyntax is StatementSyntax syntax)
            {
                bicepLine += syntax.LeadingNodes.Count(node => node is Token token && token.Type is TokenType.NewLine);
            }

            // increment start position if starting on a comma (happens when outputting successive items in objects and arrays)
            if (jsonWriter.CommaPositions.Contains(jsonStartPos))
            {
                jsonStartPos++;
            }

            var jsonEndPos = jsonWriter.CurrentPos - 1;

            rawSourceMap.AddMapping(
                bicepFilePath,
                bicepLine,
                jsonStartPos,
                jsonEndPos);
        }

        public static void AddNestedSourceMap(
            this IDictionary<string, IDictionary<int, IList<(int start, int end)>>> parentSourceMap,
            IDictionary<string, IDictionary<int, IList<(int start, int end)>>> nestedSourceMap,
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
            IDictionary<string, IDictionary<int, IList<(int start, int end)>>> rawSourceMap,
            JToken rawTemplate,
            string sourceFileAbsolutePath,
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

            // increment all positions in mappings by templateHashLength that occur after hash start position
            foreach (var file in rawSourceMap.Keys)
            {
                foreach (var line in rawSourceMap[file].Keys)
                {
                    for (int i = 0; i < rawSourceMap[file][line].Count; i++)
                    {
                        var (start, end) = rawSourceMap[file][line][i];

                        if (start >= templateHashStartPosition)
                        {
                            rawSourceMap[file][line][i] =
                                (start + templateHashLength, end + templateHashLength);
                        }
                    }
                }
            }

            // transform offsets in rawSourceMap to line numbers for formatted JSON using unformattedLineStarts
            // add 1 to all line numbers to convert to 1-indexing
            // strip full path from main bicep source file
            string getFileName(string file) => (file == sourceFileAbsolutePath) ? Path.GetFileName(file) : file;
            var formattedSourceMap = rawSourceMap.ToDictionary(
                kvp => getFileName(kvp.Key),
                kvp => kvp.Value.ToDictionary(
                    kvp => kvp.Key + 1,
                    kvp => kvp.Value.Select(mapping => (
                        TextCoordinateConverter.GetPosition(unformattedLineStarts, mapping.start).line + 1,
                        TextCoordinateConverter.GetPosition(unformattedLineStarts, mapping.end).line + 1))));

            // unfold key-values in bicep-to-json map to convert to json-to-bicep map
            var weights = new int[unformattedLineStarts.Count];
            Array.Fill(weights, int.MaxValue);

            foreach (var fileKvp in formattedSourceMap)
            {
                foreach (var lineKvp in fileKvp.Value)
                {
                    foreach (var (start, end) in lineKvp.Value)
                    {
                        // write most specific mapping available for each json line (less lines => stronger weight)
                        int weight = end - start;
                        for (int i = start; i <= end; i++)
                        {
                            // write new mapping if weight is stronger than existing mapping
                            if (weight < weights[i])
                            {
                                sourceMap![i] = (fileKvp.Key, lineKvp.Key);
                                weights[i] = weight;
                            }
                        }
                    }
                }
            }
        }

        private static void AddMapping(
            this IDictionary<string, IDictionary<int, IList<(int start, int end)>>> rawSourceMap,
            string bicepFileName,
            int bicepLine,
            int jsonStartPos,
            int jsonEndPos)
        {
            if (!rawSourceMap.TryGetValue(bicepFileName, out var bicepFileDict))
            {
                rawSourceMap[bicepFileName] = bicepFileDict = new Dictionary<int, IList<(int, int)>>();
            }

            if (!bicepFileDict.TryGetValue(bicepLine, out var mappingList))
            {
                bicepFileDict[bicepLine] = mappingList = new List<(int, int)>();
            }

            mappingList.Add((jsonStartPos, jsonEndPos));
        }
    }
}
