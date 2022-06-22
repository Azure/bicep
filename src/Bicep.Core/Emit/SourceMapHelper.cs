// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    // TODO: how to reduce duplication
    public record SourceMap(
        string Entrypoint,
        ImmutableArray<SourceMapFileEntry> Entries);

    public record SourceMapFileEntry(
        string FilePath,
        ImmutableArray<SourceMapEntry> SourceMap);

    public record SourceMapEntry(
        int SourceLine,
        int TargetLine);

    public record RawSourceMap(
        IList<RawSourceMapFileEntry> Entries);

    public record RawSourceMapFileEntry(
        string FilePath,
        IList<SourceMapRawEntry> SourceMap);

    public record SourceMapRawEntry(
        TextSpan SourcePosition,
        IList<TextSpan> TargetPositions);

    public static class SourceMapHelper
    {
        private static readonly Regex JsonWhitespaceStrippingRegex = new(@"(""(?:[^""\\]|\\.)*"")|\s+", RegexOptions.Compiled);

        public static void AddMapping(
            RawSourceMap rawSourceMap,
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
            TextSpan bicepLocation = bicepSyntax.Span;

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
            var jsonPos = new TextSpan(jsonStartPos, jsonEndPos - jsonStartPos);

            rawSourceMap.AddMapping(
                bicepFilePath,
                bicepLocation,
                jsonPos);
        }

        public static void AddNestedRawSourceMap(
            this RawSourceMap parentSourceMap,
            RawSourceMap nestedSourceMap,
            int offset)
        {
            foreach (var fileEntry in nestedSourceMap.Entries)
            {
                foreach (var sourceMapEntry in fileEntry.SourceMap)
                {
                    foreach (var position in sourceMapEntry.TargetPositions)
                    {
                        var positionWithOffset = new TextSpan(
                            position.Position + offset,
                            position.Length);

                        parentSourceMap.AddMapping(
                            fileEntry.FilePath,
                            sourceMapEntry.SourcePosition,
                            positionWithOffset);
                    }
                }
            }
        }

        public static SourceMap ProcessRawSourceMap(
            RawSourceMap rawSourceMap,
            JToken rawTemplate,
            BicepFile sourceFile)
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


            var weights = new int[unformattedLineStarts.Count];
            Array.Fill(weights, int.MaxValue);

            var sourceMapFileEntries = new List<SourceMapFileEntry>();
            var entrypointFileName = Path.GetFileName(sourceFile.FileUri.AbsolutePath);
            var entrypointAbsolutePath = Path.GetDirectoryName(sourceFile.FileUri.AbsolutePath)!;
            var fileResolver = new FileResolver();

            foreach (var bicepFileEntry in rawSourceMap.Entries)
            {
                var bicepFilePath = fileResolver.GetRelativePath(entrypointAbsolutePath, bicepFileEntry.FilePath);
                var sourceMapEntries = new List<SourceMapEntry>();

                foreach (var sourceMapEntry in bicepFileEntry.SourceMap)
                {
                    var bicepLine = TextCoordinateConverter.GetPosition(
                        sourceFile.LineStarts, sourceMapEntry.SourcePosition.GetPosition()).line;

                    foreach (var jsonPosition in sourceMapEntry.TargetPositions)
                    {
                        var jsonStartPos = jsonPosition.Position;
                        var jsonEndPos = jsonStartPos + jsonPosition.Length;

                        // TODO remove once filler hash is added
                        // increment positions by templateHashLength that occur after hash start position
                        if (jsonStartPos >= templateHashStartPosition)
                        {
                            jsonStartPos += templateHashLength;
                            jsonEndPos += templateHashLength;
                        }

                        // transform offsets in rawSourceMap to line numbers for formatted JSON using unformattedLineStarts
                        var jsonStartLine = TextCoordinateConverter.GetPosition(unformattedLineStarts, jsonStartPos).line;
                        var jsonEndLine = TextCoordinateConverter.GetPosition(unformattedLineStarts, jsonEndPos).line;

                        // write most specific mapping available for each json line (less lines => stronger weight)
                        int weight = jsonEndLine - jsonStartLine;
                        for (int jsonLine = jsonStartLine; jsonLine <= jsonEndLine; jsonLine++)
                        {
                            // write new mapping if weight is stronger than existing mapping
                            if (weight < weights[jsonLine])
                            {
                                sourceMapEntries.RemoveAll(i => i.TargetLine == jsonLine);
                                sourceMapEntries.Add(new SourceMapEntry(bicepLine, jsonLine));
                                weights[jsonLine] = weight;
                            }
                        }
                    }
                }

                var fileEntry = new SourceMapFileEntry(
                    bicepFilePath,
                    sourceMapEntries.ToImmutableArray());
                sourceMapFileEntries.Add(fileEntry);
            }

            return new SourceMap(
                entrypointFileName,
                sourceMapFileEntries.ToImmutableArray());
        }

        private static void AddMapping(
            this RawSourceMap rawSourceMap,
            string bicepFileName,
            TextSpan bicepPosition,
            TextSpan jsonPosition)
        {
            var fileEntry = rawSourceMap.Entries.FirstOrDefault(
                i => i.FilePath.Equals(bicepFileName, StringComparison.OrdinalIgnoreCase));
            if (fileEntry is null)
            {
                fileEntry = new RawSourceMapFileEntry(bicepFileName, new List<SourceMapRawEntry>());

                rawSourceMap.Entries.Add(fileEntry);
            }

            var sourceMapEntry = fileEntry.SourceMap.FirstOrDefault(i => i.SourcePosition == bicepPosition);
            if (sourceMapEntry is null)
            {
                sourceMapEntry = new SourceMapRawEntry(bicepPosition, new List<TextSpan>());

                fileEntry.SourceMap.Add(sourceMapEntry);
            }

            sourceMapEntry.TargetPositions.Add(jsonPosition);
        }
    }
}
